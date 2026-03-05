using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

[ExecuteAlways]
public class RayTracingSphereManagerSimple : MonoBehaviour
{

	[Header("References")]
	[SerializeField] Shader rayTracingShader;
	[SerializeField, HideInInspector] Shader accumulateShader;

	// Lazy initialization of default shader
	private Shader GetRayTracingShader()
	{
		if (rayTracingShader == null)
		{
			rayTracingShader = Shader.Find("Custom/RayTracingSpheres");
		}
		return rayTracingShader;
	}

	[Header("Info")]
	[SerializeField] int numRenderedFrames;
	[SerializeField] int numSpheres;

	// Materials and render textures
	Material rayTracingMaterial;
	Material accumulateMaterial;
	RenderTexture resultTexture;

	// Buffers
	ComputeBuffer sphereBuffer;
	
	// Track if data needs updating
	private int lastSphereCount = -1;
	private bool needsDataUpdate = true;

	void Start()
	{
		numRenderedFrames = 0;
		needsDataUpdate = true;
	}

	void LateUpdate()
	{
		// Update camera parameters every frame
		if (Camera.current != null)
		{
			UpdateCameraParams(Camera.current);
		}
		
		// Check if spheres in scene have changed
		RayTracedSphereSimple[] sphereObjects = FindObjectsByType<RayTracedSphereSimple>(FindObjectsSortMode.None);
		if (sphereObjects.Length != lastSphereCount)
		{
			needsDataUpdate = true;
			lastSphereCount = sphereObjects.Length;
		}
	}

	// Called after any camera (e.g. game or scene camera) has finished rendering into the src texture
	void OnRenderImage(RenderTexture src, RenderTexture target)
	{
		Debug.Log("OnRenderImage");
		InitFrame();

		// Create copy of prev frame
		RenderTexture prevFrameCopy = RenderTexture.GetTemporary(src.width, src.height, 0, ShaderHelper.RGBA_SFloat);
		Graphics.Blit(resultTexture, prevFrameCopy);

		// Run the ray tracing shader and draw the result to a temp texture
		rayTracingMaterial.SetInt("Frame", numRenderedFrames);
		RenderTexture currentFrame = RenderTexture.GetTemporary(src.width, src.height, 0, ShaderHelper.RGBA_SFloat);
		Graphics.Blit(null, currentFrame, rayTracingMaterial);

		// Accumulate
		accumulateMaterial.SetInt("_Frame", numRenderedFrames);
		accumulateMaterial.SetTexture("_PrevFrame", prevFrameCopy);
		Graphics.Blit(currentFrame, resultTexture, accumulateMaterial);

		// Draw result to screen
		Graphics.Blit(resultTexture, target);

		// Release temps
		RenderTexture.ReleaseTemporary(currentFrame);
		RenderTexture.ReleaseTemporary(prevFrameCopy);

		numRenderedFrames += Application.isPlaying ? 1 : 0;
	}

	void InitFrame()
	{
		// Create materials used in blits
		ShaderHelper.InitMaterial(GetRayTracingShader(), ref rayTracingMaterial);
		ShaderHelper.InitMaterial(accumulateShader, ref accumulateMaterial);
		// Create result render texture
		ShaderHelper.CreateRenderTexture(ref resultTexture, Screen.width, Screen.height, FilterMode.Bilinear, ShaderHelper.RGBA_SFloat, "Result");

		// Update data only if needed
		if (needsDataUpdate)
		{
			CreateSpheres();
			needsDataUpdate = false;
		}
	}

	void UpdateCameraParams(Camera cam)
	{
		if (cam == null) return;
		
		float focusDistance = 1.0f; // Fixed distance
		float planeHeight = focusDistance * Tan(cam.fieldOfView * 0.5f * Deg2Rad) * 2;
		float planeWidth = planeHeight * cam.aspect;
		// Send data to shader
		if (rayTracingMaterial != null)
		{
			rayTracingMaterial.SetVector("ViewParams", new Vector3(planeWidth, planeHeight, focusDistance));
			rayTracingMaterial.SetMatrix("CamLocalToWorldMatrix", cam.transform.localToWorldMatrix);
		}
	}

	void CreateSpheres()
	{
		// Create sphere data from the sphere objects in the scene
		RayTracedSphereSimple[] sphereObjects = FindObjectsByType<RayTracedSphereSimple>(FindObjectsSortMode.None);
		
		SphereSimple[] spheres;
		
		if (sphereObjects.Length == 0)
		{
			// If no spheres, create a dummy sphere to avoid empty buffer
			spheres = new SphereSimple[1];
			spheres[0] = new SphereSimple()
			{
				position = Vector3.zero,
				radius = 0f,
				material = GetDefaultMaterial()
			};
		}
		else
		{
			spheres = new SphereSimple[sphereObjects.Length];
			for (int i = 0; i < sphereObjects.Length; i++)
			{
				RayTracingMaterialSimple mat = sphereObjects[i].material;
				// If material is null, use default material
				
				
				spheres[i] = new SphereSimple()
				{
					position = sphereObjects[i].transform.position,
					radius = sphereObjects[i].transform.localScale.x * 0.5f,
					material = mat
				};
				
				Debug.Log($"Sphere {i}: pos={spheres[i].position}, radius={spheres[i].radius}, color={mat.colour}");
			}
		}

		// Create buffer containing all sphere data, and send it to the shader
		ShaderHelper.CreateStructuredBuffer(ref sphereBuffer, spheres);
		if (rayTracingMaterial != null)
		{
			rayTracingMaterial.SetBuffer("Spheres", sphereBuffer);
			rayTracingMaterial.SetInt("NumSpheres", sphereObjects.Length);
			
			Debug.Log($"Set {sphereObjects.Length} spheres to shader buffer");
		}
		else
		{
			Debug.LogError("rayTracingMaterial is null - cannot set sphere buffer!");
		}

		numSpheres = sphereObjects.Length;
	}

	RayTracingMaterialSimple GetDefaultMaterial()
	{
		return new RayTracingMaterialSimple()
		{
			colour = Color.white
		};
	}

	void OnDisable()
	{
		ShaderHelper.Release(sphereBuffer);
		ShaderHelper.Release(resultTexture);
	}
}
