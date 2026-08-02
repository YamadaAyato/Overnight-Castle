using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIの表記やアニメーションを作った
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Image _backImage;
    [SerializeField] private Image[] _scoreImages;
    [SerializeField] private GameObject[] _inGameObjects;

    [SerializeField]private TextMeshProUGUI _height;
    [SerializeField]private TextMeshProUGUI _heightScore;
    [SerializeField] private TextMeshProUGUI _maxScoreText;

    [SerializeField]private TextMeshProUGUI _completionScore;
    [SerializeField]private TextMeshProUGUI _totalScore;

    [SerializeField]private float _saizUPTime = 0.5f;
    [SerializeField] private float _Interval = 0.3f;

    private Sequence _sequence;
    private bool _isMaxScore;

    private void Awake()
    {
        foreach (Image image in _scoreImages)
        {
            image.gameObject.SetActive(false);
        }

        foreach (GameObject obj in _inGameObjects)
        {
            obj.SetActive(true);
        }

        _backImage?.gameObject.SetActive(false);
        _height?.gameObject.SetActive(false);
        _heightScore?.gameObject.SetActive(false);
        _maxScoreText?.gameObject.SetActive(false);
        _completionScore?.gameObject.SetActive(false);
        _totalScore?.gameObject.SetActive(false);
    }

    public void SetUI(CastleScoreResult result,bool isMaxScore)
    {
        _isMaxScore = isMaxScore;
        PlayScoreAnimation(result);
    }


    /// <summary>
    /// アニメーションの動きを実行させてる関数
    /// </summary>
    /// <param name="result"></param>
    public void PlayScoreAnimation(CastleScoreResult result)
    {
        _sequence?.Kill();

        _backImage.gameObject.SetActive(true);

        foreach (Image image in _scoreImages)
        {
            image.gameObject.SetActive(false);
        }

        foreach (GameObject obj in _inGameObjects)
        {
            obj.SetActive(false);
        }

        _height.gameObject.SetActive(false);
        _heightScore.gameObject.SetActive(false);
        _completionScore.gameObject.SetActive(false);
        _totalScore.gameObject.SetActive(false);

        _sequence = DOTween.Sequence();

        // 画像をすべて先に表示
        for (int i = 0; i < _scoreImages.Length; i++)
        {
            bool addInterval = i < _scoreImages.Length - 1;
            AppendAnimation(_scoreImages[i].gameObject, addInterval);
        }

        //順番に表示・カウントアップ
        _sequence.Join(ShowScore(result.Height, _height));
        _sequence.Join(ShowScore(result.HeightScore, _heightScore));
        _sequence.Join(ShowScore(result.CompletionScore, _completionScore));
        _sequence.Join(ShowScore(result.TotalScore, _totalScore));
        if (_isMaxScore && _maxScoreText != null)
        {
            AppendAnimation(_maxScoreText.gameObject, false);
        }
    }

    /// <summary>
    /// 大きさを変えるアニメーション
    /// </summary>
    /// <param name="scoreImage"></param>
    /// <param name="addInterval"></param>
    private void AppendAnimation(GameObject scoreImage, bool addInterval = true)
    {
        Vector3 defaultScale = scoreImage.transform.localScale;

        _sequence.AppendCallback(() =>
        {
            scoreImage.SetActive(true);
            scoreImage.transform.localScale = Vector3.zero;
        });

        _sequence.Append(
            scoreImage.transform.DOScale(defaultScale, _saizUPTime)
                .SetEase(Ease.OutBack)
        );

        if (addInterval)
        {
            _sequence.AppendInterval(_Interval);
        }
    }

    /// <summary>
    /// 文字のカウントアニメーション
    /// </summary>
    /// <param name="score"></param>
    /// <param name="scoreText"></param>
    /// <returns></returns>
    private Tween ShowScore(float score, TextMeshProUGUI scoreText)
    {
        _sequence.AppendCallback(() =>
        {
            scoreText.gameObject.SetActive(true);
        } );
        float currentScore = 0;

        return DOTween.To(
            () => currentScore,
            value =>
            {
                currentScore = value;
                scoreText.text = ($"{currentScore.ToString()}");
            },
            score,
            _saizUPTime
        );
    }
}
