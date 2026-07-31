using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraAnimation : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform _target1;
    [SerializeField] private Transform _target2;
    [SerializeField] private Transform _target3;

    [Header("Settings")]
    [SerializeField] private CinemachineCamera _cinemachineCamera;
    [SerializeField] private float _moveDuration = 1f;
    [SerializeField] private float _zoomInOrthographicSize = 5f;
    [SerializeField] private float _zoomOutAdjustment = 10f;

    private Sequence _sequence;
    private float _initialOrthographicSize;

    // Lens が構造体でも安全に OrthographicSize を書き換えるためのプロパティ
    private float OrthographicSize
    {
        get => _cinemachineCamera.Lens.OrthographicSize;
        set
        {
            var lens = _cinemachineCamera.Lens;
            lens.OrthographicSize = value;
            _cinemachineCamera.Lens = lens;
        }
    }

    private void Awake()
    {
        _initialOrthographicSize = OrthographicSize;
    }

    public Tween PlayCameraMove()
    {
        // ボタン連打などで前回の演出が残らないようにする
        _sequence?.Kill();

        // Cinemachine の追従を一時的に外す
        _cinemachineCamera.Target.TrackingTarget = null;

        float cameraZ = _cinemachineCamera.transform.position.z;

        Vector3 target1Pos = new Vector3(
            _target1.position.x,
            _target1.position.y,
            cameraZ
        );

        Vector3 target2Pos = new Vector3(
            _target2.position.x,
            _target2.position.y,
            cameraZ
        );

        Vector3 centerPos = new Vector3(0f, 0f, cameraZ);

        Vector3 target3Pos = new Vector3(
            _target3.position.x,
            _target3.position.y,
            cameraZ
        );

        float distance = Vector2.Distance(_target1.position, _target2.position);

        // OrthographicSize は大きいほどズームアウト
        float zoomOutOrthographicSize = distance + _zoomOutAdjustment;

        _sequence = DOTween.Sequence();

        // target1 へ移動しながらズームイン
        _sequence.Append(
            _cinemachineCamera.transform.DOMove(target1Pos, _moveDuration)
        );

        _sequence.Join(
            DOTween.To(
                () => OrthographicSize,
                value => OrthographicSize = value,
                _zoomInOrthographicSize,
                _moveDuration
            )
        );

        // target2 へ移動
        _sequence.Append(
            _cinemachineCamera.transform.DOMove(target2Pos, _moveDuration)
        );

        // 中央へ移動しながらズームアウト
        _sequence.Append(
            _cinemachineCamera.transform.DOMove(centerPos, _moveDuration)
        );

        _sequence.Join(
            DOTween.To(
                () => OrthographicSize,
                value => OrthographicSize = value,
                zoomOutOrthographicSize,
                _moveDuration
            )
        );

        // target3 へ移動するだけ。
        // OrthographicSize は zoomOutOrthographicSize のまま維持される。
        _sequence.Append(
            _cinemachineCamera.transform.DOMove(target3Pos, _moveDuration)
        );

        _sequence.OnComplete(() =>
        {
            OrthographicSize = zoomOutOrthographicSize;
        });


        return _sequence;
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }
}