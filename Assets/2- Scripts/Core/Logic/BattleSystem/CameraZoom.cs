using UnityEngine;
using UnityEngine.InputSystem;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private float _zoom;
    [SerializeField] private float _zoomMultiplier = 4f;
    [SerializeField] private float _minZoom = 2f;
    [SerializeField] private float _maxZoom = 8f;
    [SerializeField] private float _velocity = 0f;
    [SerializeField] private float _smoothTime = 0.25f;
    [SerializeField] private Camera _camera;

    private void Start()
    {
        CameraBounds.Calculate(_camera, _maxZoom);
        _zoom = _camera.orthographicSize;
    }

    private void Update()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;

        _zoom -= scroll * _zoomMultiplier;
        _zoom = Mathf.Clamp(_zoom, _minZoom, _maxZoom);
        _camera.orthographicSize = Mathf.SmoothDamp(_camera.orthographicSize, _zoom, ref _velocity, _smoothTime);
    }
}
