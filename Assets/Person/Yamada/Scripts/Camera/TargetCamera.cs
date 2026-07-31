using Unity.Cinemachine;
using UnityEngine;

public class TargetCamera : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private CinemachineCamera _camera;

    [SerializeField, Min(0.1f)] private float _heightStep = 5f;
    [SerializeField, Min(0f)] private float _zoomOutAmount = 0.5f;
    [SerializeField, Min(0.1f)] private float _maxOrthographicSize = 7f;
    [SerializeField, Min(0f)] private float _zoomSmoothTime = 0.5f;

    private float _spawnOffsetY;
    private float _initialTargetY;
    private float _initialOrthographicSize;
    private float _zoomVelocity;

    private void Start()
    {
        if(_targetTransform == null)
        {
            Debug.LogError("Target Transformが設定されていません。");
            enabled = false;
            return;
        }

        if (_camera == null)
        {
            Debug.LogError("Cinemachine Cameraが設定されていません。", this);
            enabled = false;
            return;
        }

        _spawnOffsetY = transform.position.y - _targetTransform.position.y;
        _initialTargetY = _targetTransform.position.y;
        _initialOrthographicSize = _camera.Lens.OrthographicSize;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = transform.position;
        targetPosition.y = _targetTransform.position.y + _spawnOffsetY;
        transform.position = targetPosition;

        float movedHeight = Mathf.Max(0f, _targetTransform.position.y - _initialTargetY);
        int currentStep = Mathf.FloorToInt(movedHeight / _heightStep);

        float targetOrthographicSize = Mathf.Min(
            _initialOrthographicSize + currentStep * _zoomOutAmount
            , _maxOrthographicSize);

        LensSettings lens = _camera.Lens;
        lens.OrthographicSize = Mathf.SmoothDamp(
            lens.OrthographicSize,
            targetOrthographicSize,
            ref _zoomVelocity,
            _zoomSmoothTime);

        _camera.Lens = lens;
    }
}
