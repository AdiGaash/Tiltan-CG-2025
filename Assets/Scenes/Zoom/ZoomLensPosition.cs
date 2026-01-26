using UnityEngine;

public class ZoomLensPosition : MonoBehaviour
{
    [SerializeField] Material _zoomMaterial;
    Camera _camera;
    Vector2 _screenPos;

    private const string SHADER_OBJECT_SCREEN_POSITION = "_ObjectScreenPosition";

    private void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        _screenPos = _camera.WorldToScreenPoint(transform.position);
        _screenPos = new Vector2(_screenPos.x / Screen.width, _screenPos.y / Screen.height);
        _zoomMaterial.SetVector(SHADER_OBJECT_SCREEN_POSITION, _screenPos);
    }
}
