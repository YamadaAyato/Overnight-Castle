using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 時間制限を管理するクラス
/// </summary>
public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    /// <summary>
    /// 終わった時の通知
    /// </summary>
    public event Action OnTimeUp;

    public event Action<float> OnTimeAdded;
    public float CurrentTime => _currentTime;
    public bool IsRunning => _isRunning;

    [SerializeField] Image _timerImage;
    private float _maxTime;
    private float _currentTime;
    private bool _isRunning;
    

    /// <summary>
    /// 何秒間時間を回すのか
    /// </summary>
    /// <param name="time"></param>
    public void StartTimer(float time)
    {
        _maxTime = time;
        _currentTime = time;
        _isRunning = true;

        UpdateTimerImage();
    }

    public void AddTime(float time)
    {
        if(!_isRunning ||
            time <= 0) 
        {
            return;
        }

        _maxTime += time;
        _currentTime += time;

        UpdateTimerImage();
        OnTimeAdded?.Invoke(time);
        Debug.Log($"AddTime: {time}, CurrentTime: {_currentTime}");
    }

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (!_isRunning) 
        {
            return;
        }
            
        _currentTime -= Time.deltaTime;


        if (_currentTime <= 0)
        {
            TimerReset();
        }
        else
        {
            UpdateTimerImage();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void UpdateTimerImage()
    {
        if(_timerImage != null)
        {
            _timerImage.fillAmount = Mathf.Clamp01(_currentTime / _maxTime);
        }
    }

    private void TimerReset()
    {
        _isRunning = false;
        _currentTime = 0;

        UpdateTimerImage() ;

        OnTimeUp?.Invoke();
        Debug.Log("Time's up!");
    }
}