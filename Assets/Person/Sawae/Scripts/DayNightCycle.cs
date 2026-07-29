using UnityEngine;
using DG.Tweening;
/// <summary>
/// 制限時間経過で天球ピボットが時計回りに回転し、月が沈み太陽が昇る夜明け演出を行うクラス
/// 月と太陽のオブジェクトを、空の親オブジェクトの子として配置する
/// </summary>

public class DayNightCycle : MonoBehaviour
{
    [Header("天球ピボット")]
    [SerializeField] private Transform celestiaPivot;

    [Header("制限時間の取得元")]
    [SerializeField] private StageSettings _stageSettings;

    private Vector3 rotationAngle = new Vector3(0f, 0f, 180f);

    private Tween _rotateTween;

    private void Start()
    {
        if (_stageSettings == null)
        {
            Debug.Log("StageSettingsが設定されていません。", this);
            return;
        }

        Play(_stageSettings.StageTimeLimit);
    }
    /// <summary>
    /// 指定した時間で180度回転を完了させる
    /// </summary>
    /// <param name="duration">回転にかける秒数(StageTimeLimit)</param>
    private void Play(float duration)
    {
        _rotateTween?.Kill();

        //天球ピボットの回転を、初期状態にする
        celestiaPivot.localRotation = Quaternion.identity;

        // duration秒かけてStageTimeLimitちょうどで180度回転する
        _rotateTween = celestiaPivot
            .DOLocalRotate(rotationAngle, duration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }
}
