using System;
using UnityEngine;

/// <summary>
/// 時間制限を管理するクラス
/// </summary>
public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    [Tooltip("時間が終わったかどうか")]
    public Action OnTimeUp;

    private float _currentTime;
    private bool _isRunning;
    
    /// <summary>
    /// 何秒間時間を回すのか
    /// </summary>
    /// <param name="time"></param>
    public void StartTimer(float time)
    {
        _currentTime = time;
        _isRunning = true;
    }

    /// <summary>
    /// 残り時間の取得
    /// </summary>
    /// <returns></returns>
    public float GetCurrentTime()
    {
        return _currentTime;
    }

    public void AddTime(int Time) 
    {
        _currentTime += Time;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!_isRunning) 
            return;
            

        _currentTime -= Time.deltaTime;

        if (_currentTime <= 0)
        {
            TimerReset();
        }
    }

    private void TimerReset() 
    {
        _isRunning = false;
        _currentTime = 0;

        OnTimeUp?.Invoke();
    }
}