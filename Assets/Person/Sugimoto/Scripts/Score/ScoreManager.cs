using DG.Tweening;
using GameFoundation.Runtime.Attributers;
using UnityEngine;

/// <summary>
/// スコアを管理するクラス
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>現在のキャッスルスコアの結果</summary>
    public CastleScoreResult ScoreResult { get; private set; }

    /// <summary>最高スコア</summary>
    public int MaxScore { get; private set; } = 0;

    [SerializeField] private ScoreSender _scoreSender;
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private CameraAnimation _cameraAnimation;
    [SerializeField] private TargetCamera _targetCamera;
    [SerializeField] private CharacterImageManager _characterImageManager;
    [SerializeField, SceneNameSelector] private string _returnSceneName;

    private bool _isReturnedToTitle = false;

    private const string MaxScoreKey = "MaxScore";

    public void SetCastleScoreResult(CastleScoreResult castleScoreResult)
    {
        _targetCamera.SetTrackingEnabled(false);
        ScoreResult = castleScoreResult;
        bool isMax = ScoreResult.TotalScore > MaxScore;

        if (isMax)
        {
            MaxScore = ScoreResult.TotalScore;

            PlayerPrefs.SetInt(MaxScoreKey, MaxScore);
            PlayerPrefs.Save();

            _characterImageManager.PlayAnimation(ImageType.HighScore);
            _scoreSender.SendHighScore(MaxScore);
        }

        AudioManager.Instance.PlaySE("ResultText");
        _cameraAnimation.PlayCameraMove()
        .OnComplete(() =>
        {
            _scoreUI.SetUI(ScoreResult, isMax);
        });
    }

    /// <summary>
    /// スコアをリセット
    /// </summary>
    public void ResetScore()
    {
        ScoreResult = default;
    }

    /// <summary>
    /// 最高スコアを送信する
    /// </summary>
    public void SendMaxScore()
    {
        _scoreSender.SendHighScore(MaxScore);
    }

    /// <summary>
    ///     タイトルに戻る
    /// </summary>
    public void ReturnToTitle()
    {
        if(_isReturnedToTitle)
        {
            return;
        }

        if(string.IsNullOrEmpty(_returnSceneName))
        {
            Debug.LogError("ReturnSceneNameが設定されていません", this);
            return;
        }

        _isReturnedToTitle = true;
        GameSession.Clear();
        SceneLoader.LoadScene(_returnSceneName);
    }
    private void Awake()
    {
        MaxScore = PlayerPrefs.GetInt(MaxScoreKey, 0);
    }

}