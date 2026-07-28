using UnityEngine;
using DG.Tweening;
/// <summary>
/// 制限時間経過後に天球ピボットを回転させ、月が沈み太陽が昇る夜明け演出を行うクラス
/// </summary>

public class DayNightCycle : MonoBehaviour
{
    [Header("天球ピボット")]
    [SerializeField] private Transform celestiaPivot;

    [Header("制限時間の取得元")]
    [SerializeField] private StageSettings _stageSettings;

    [Header("回転開始のタイミング(ゲーム開始からの経過秒数)")]
    [SerializeField] private float triggerTime = 60f;

    [Header("回転を完了させる秒数")]
    [SerializeField] private float RotateDuration = 1f; // 1秒で180度回転する

    private Vector3 rotationAngle = new Vector3(0f, 0f, 180f);

    private float _elapsedTime; // ゲーム開始から何秒経過したか
    private bool _hasTriggered; // 回転がすでに1回発動したかどうか

    private void Update()
    {
        //すでに回転していた場合中断する
        if (_hasTriggered || _stageSettings == null)
        {
            return;
        } 

        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= triggerTime)
        {
            _hasTriggered = true;
            StartRotation();
        }
    }
    /// <summary>
    /// 特定の時間が経過したとき１秒間で180度回転する
    /// </summary>
    private void StartRotation()
    {
        celestiaPivot
            .DOLocalRotate(rotationAngle, RotateDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear);
    }
}
