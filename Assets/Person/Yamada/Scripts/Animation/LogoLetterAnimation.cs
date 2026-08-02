using DG.Tweening;
using UnityEngine;

public class LogoLetterAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform _letter;
    [SerializeField] private RectTransform _tialeImage;

    [Header("落下設定")]
    [SerializeField] private float _startOffsetY;
    [SerializeField] private float _startDelay;
    [SerializeField] private float _fallDuraion;
    [SerializeField] private Ease _fallEase;
 
    [Header("バウンス設定")]
    [SerializeField] private float _bounceHeight;
    [SerializeField] private float _bounceDuration;
    [SerializeField] private Ease _bounceEase;

    private Sequence _sequence;
    private Vector3 _defualtPos;
    private Vector3 _defaultTitlePos;

    private void Start()
    {
        if (_letter == null)
        {
            Debug.LogError("_letter が null です。");
            return;
        }

        _defualtPos = _letter.anchoredPosition;
        _defaultTitlePos = _tialeImage.anchoredPosition;
        Play();
    }

    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    private void Play()
    {
        _sequence?.Kill();

        Vector2 startPos = _defualtPos + new Vector3(0, _startOffsetY, 0);
        Vector2 titlePos = _defaultTitlePos + new Vector3(0, _bounceHeight, 0);
        _letter.anchoredPosition = startPos;
        _letter.localRotation = Quaternion.identity;

        _sequence = DOTween.Sequence();
        _sequence.AppendInterval(_startDelay)
            .Append(_letter.DOAnchorPosY(_defualtPos.y, _fallDuraion)
            .SetEase(_fallEase))
            .Append(_tialeImage.DOAnchorPosY(titlePos.y, _bounceDuration))
            .SetEase(_bounceEase)
            .Append(_tialeImage.DOAnchorPosY(_defaultTitlePos.y, _bounceDuration))
            .SetEase(_bounceEase);


        _sequence.Append(_letter.DOAnchorPosY(_defualtPos.y - _startOffsetY, _fallDuraion)
            .SetEase(_fallEase)); 
    }
}
