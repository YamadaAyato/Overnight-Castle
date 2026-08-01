using UnityEngine;
using DG.Tweening;

public class ChengUI : MonoBehaviour
{
    [Header("変えたときに移動するUI")]
    [SerializeField] private RectTransform[] _upObjs;
    [SerializeField] private float _upImagePositionY = 5f;

    [SerializeField] private RectTransform[] _downObjs;
    [SerializeField] private float _downImagePositionY = 5f;

    [Header("横からスライドさせるUI")]
    [SerializeField] private RectTransform _previewObj;
    [SerializeField] private float _previewPositionX = 1500f;

    [SerializeField] private RectTransform _buttonObj;
    [SerializeField] private float _buttonPositionX = -450f;

    [SerializeField, Min(0f)] private float _duration = 1f;

    public void UIChange()
    {
        foreach (RectTransform rectTransform in _upObjs)
        {
            if (rectTransform == null)
            {
                continue;
            }

            rectTransform.DOKill();
            rectTransform.DOAnchorPosY(_upImagePositionY, _duration);
        }

        foreach (RectTransform rectTransform in _downObjs)
        {
            if (rectTransform == null)
            {
                continue;
            }

            rectTransform.DOKill();
            rectTransform.DOAnchorPosY(_downImagePositionY, _duration);
        }

        if (_previewObj != null)
        {
            _previewObj.DOKill();
            _previewObj.DOAnchorPosX(_previewPositionX, _duration);
        }

        if (_buttonObj != null)
        {
            _buttonObj.DOKill();
            _buttonObj.DOAnchorPosX(_buttonPositionX, _duration);
        }
    }
}