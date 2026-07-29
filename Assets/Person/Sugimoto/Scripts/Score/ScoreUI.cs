using DG.Tweening;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField]private TextMeshProUGUI _height;
    [SerializeField]private TextMeshProUGUI _heightScore;
    [SerializeField]private TextMeshProUGUI _completionScore;
    [SerializeField]private TextMeshProUGUI _totalScore;

    [SerializeField]private float _maxSaiz = 1.2f;
    [SerializeField]private float _saizUPTime = 0.5f;
    [SerializeField] private float _Interval = 0.3f;

    private void Awake()
    {
        _image.gameObject.SetActive(false);
        _height.gameObject.SetActive(false);
        _heightScore.gameObject.SetActive(false);
        _completionScore.gameObject.SetActive(false);
        _totalScore.gameObject.SetActive(false);
    }

    public void SetUI(CastleScoreResult castleScoreResult) 
    {
        _height.text = castleScoreResult.Height.ToString();
        _heightScore.text = castleScoreResult.HeightScore.ToString();
        _completionScore.text = castleScoreResult.CompletionScore.ToString();
        _totalScore.text = castleScoreResult.TotalScore.ToString();
        PlayScoreAnimation();
    }

    private Sequence _sequence;


    /// <summary>
    /// スコアのアニメーション関連
    /// </summary>
    public void PlayScoreAnimation()
    {
        _sequence?.Kill();

        _image.gameObject.SetActive(true);
        _height.gameObject.SetActive(false);
        _heightScore.gameObject.SetActive(false);
        _completionScore.gameObject.SetActive(false);
        _totalScore.gameObject.SetActive(false);

        _sequence = DOTween.Sequence(); 

        _sequence.AppendCallback(() => _height.gameObject.SetActive(true));
        _sequence.Append(_height.transform.DOScale(_maxSaiz, _saizUPTime));
        _sequence.AppendInterval(_Interval);

        _sequence.AppendCallback(() => _heightScore.gameObject.SetActive(true));
        _sequence.Append(_heightScore.transform.DOScale(_maxSaiz, _saizUPTime));
        _sequence.AppendInterval(_Interval);

        _sequence.AppendCallback(() => _completionScore.gameObject.SetActive(true));
        _sequence.Append(_completionScore.transform.DOScale(_maxSaiz, _saizUPTime));
        _sequence.AppendInterval(_Interval);

        _sequence.AppendCallback(() => _totalScore.gameObject.SetActive(true));
        _sequence.Append(_totalScore.transform.DOScale(_maxSaiz, _saizUPTime));
    }
}
