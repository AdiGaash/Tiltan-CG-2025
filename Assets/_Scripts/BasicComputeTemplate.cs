using UnityEngine;

public class BasicComputeTemplate : MonoBehaviour
{
    // Assign the compute shader in the Inspector
    public ComputeShader computeShader;

    // Number of elements we want to process
    public int elementCount = 256;

    // Example parameter sent to GPU
    public float multiplier = 2.0f;

    // GPU buffer
    ComputeBuffer resultBuffer;

    void Start()
    {
        if (SystemInfo.supportsComputeShaders)
        {
            RunComputeShader();
        }
        else
        {
            Debug.LogError("Compute shaders not supported on this platform.");
        }
    }
    
    void RunComputeShader()
    {
    // ================================
        // 1. FIND KERNEL
        // ================================
        int kernelHandle = computeShader.FindKernel("CSMain");

        // ================================
        // 2. CREATE BUFFER
        // ================================
        // elementCount = number of elements
        // sizeof(float) = size of one element in bytes
        resultBuffer = new ComputeBuffer(elementCount, sizeof(float));

        // ================================
        // 3. SEND DATA TO GPU
        // ================================
        computeShader.SetBuffer(kernelHandle, "ResultBuffer", resultBuffer);
        computeShader.SetFloat("Multiplier", multiplier);

        // ================================
        // 4. DISPATCH
        // ================================
        // THREADS PER GROUP = 64 (from numthreads)
        // GROUP COUNT = elementCount / 64
        int threadGroupsX = Mathf.CeilToInt(elementCount / 64.0f);

        computeShader.Dispatch(kernelHandle, threadGroupsX, 1, 1);

        // ================================
        // 5. READ BACK DATA (for debugging)
        // ================================
        float[] result = new float[elementCount];
        resultBuffer.GetData(result);

        Debug.Log("Result[10] = " + result[10]);
    }

    void OnDestroy()
    {
        // Always release GPU memory
        resultBuffer.Release();
    }
}