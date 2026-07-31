using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraAnimation : MonoBehaviour
{
    [SerializeField] private Transform _target1;
    [SerializeField] private Transform _target2;
    [SerializeField] private Transform _target3;
    [SerializeField] private float _moveDuration = 1f;

    [SerializeField] private float _zoomInOrthographicSize = 5;

    [SerializeField] private CinemachineCamera _cinemachineCamera;

    private Vector3 _target1Pos;
    private Vector3 _target2Pos;
    private Vector3 _target3Pos;
    private Vector3 _pos;    
    private float _orthographicSize
    {
        get => _cinemachineCamera.Lens.OrthographicSize;
        set
        {
            var lens = _cinemachineCamera.Lens;
            lens.OrthographicSize = value;
            _cinemachineCamera.Lens = lens;
        }
    }

    private void Start()
    {
        _orthographicSize = _cinemachineCamera.Lens.OrthographicSize;
        
        _target1Pos = new Vector3(_target1.position.x,_target1.position.y,transform.position.z);
        _target2Pos = new Vector3( _target2.position.x, _target2.position.y, transform.position.z);
        _target3Pos = new Vector3(_target3.position.x,_target3.position.y,transform.position.z);
        _pos = new Vector3(0, 0, -10);
    }

    public Tween PlayCameraMove()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            transform.DOMove(_target1Pos, _moveDuration)
        );

        sequence.Join(
            DOTween.To(
                () => _cinemachineCamera.Lens.OrthographicSize,
                x => _cinemachineCamera.Lens.OrthographicSize = x,
                _zoomInOrthographicSize,
                _moveDuration
            )
        );

        sequence.Append(
            transform.DOMove(_target2Pos, _moveDuration)
        );

        sequence.Append(
            transform.DOMove(_pos, _moveDuration)
        );

        sequence.Join(
        DOTween.To(
        () => _cinemachineCamera.Lens.OrthographicSize,
        x => _cinemachineCamera.Lens.OrthographicSize = x,
        _orthographicSize,
        _moveDuration
        )
        );

        sequence.Append(
            transform.DOMove(_target3Pos, _moveDuration)
        );
        return sequence;
    }
}