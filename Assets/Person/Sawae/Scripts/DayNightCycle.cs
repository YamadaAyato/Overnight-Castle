using UnityEngine;
using DG.Tweening;
/// <summary>
/// 制限時間経過で天球ピボットが時計回りに回転し、月が沈み太陽が昇る夜明け演出を行うクラス
/// 月と太陽のオブジェクトを、空の親オブジェクトの子として配置する
/// </summary>

public class DayNightCycle : MonoBehaviour
{
    [Header("天球ピボット")]
    [SerializeField] private Transform _celestiaPivot;

    private Vector3 rotationAngle = new Vector3(0f, 0f, 180f);

    private Timer _timer;
    private Tween _rotateTween;

    public bool Initialize(Timer timer)
    {
        if(_celestiaPivot == null)
        {
            Debug.LogError("天球ピボットが設定されていません", this);
            return false;
        }

        if (timer == null)
        {
            Debug.LogError("Timerが設定されていません", this);
            return false;
        }

        _timer = timer;
        _timer.OnTimeAdded += HandleTimeAdded;
        Play(_timer.CurrentTime);
        return true;
    }

    private void HandleTimeAdded(float addedTime)
    {
        // タイマーの時間に応じて回転を再開
        Play(_timer.CurrentTime);
    }

    /// <summary>
    /// 指定した時間で180度回転を完了させる
    /// </summary>
    /// <param name="duration">回転にかける秒数(StageTimeLimit)</param>
    private void Play(float duration)
    {
        _rotateTween?.Kill();

        //天球ピボットの回転を、初期状態にする
        _celestiaPivot.localRotation = Quaternion.identity;

        // duration秒かけてStageTimeLimitちょうどで180度回転する
        _rotateTween = _celestiaPivot
            .DOLocalRotate(rotationAngle, duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLink(gameObject);
    }
}
