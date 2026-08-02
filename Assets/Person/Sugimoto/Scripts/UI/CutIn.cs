using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CutInAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _cutInPanel;
    [SerializeField] private RectMask2D _rectMask;

    [SerializeField] private float _showTime = 0.3f;
    [SerializeField] private float _waitTime = 1f;
    [SerializeField] private float _hideTime = 0.3f;

    private Image _charHead; 
    private Image _charBode;
    private Sequence _sequence;
    private Vector4 _defaultPadding;

    private void Awake()
    {
        _cutInPanel.SetActive(false);
        _defaultPadding = _rectMask.padding;
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="image"></param>
    /// <param name="image2"></param>
    public void Initialize(Image image, Image image2)
    {
        _charHead = image;
        _charBode = image2;
    }

    /// <summary>
    /// カットインアニメーション
    /// </summary>
    public void PlayCutIn()
    {
        _rectMask.padding = _defaultPadding;

        _sequence?.Kill();

        _cutInPanel.SetActive(true);

        _sequence = DOTween.Sequence();

        // padding.xを現在値から200fまで変化させる
        _sequence.Append(
            DOTween.To(
                () => _rectMask.padding.x,
                value =>
                {
                    Vector4 padding = _rectMask.padding;
                    padding.x = value;
                    _rectMask.padding = padding;
                },
                200f,
                _showTime
            ).SetEase(Ease.OutQuad)
        );

        _sequence.Append(
            DOTween.To(
                () => _rectMask.padding.z,
                value =>
                {
                    Vector4 padding = _rectMask.padding;
                    padding.z = value;
                    _rectMask.padding = padding;
                },
                800f,
                _showTime
            ).SetEase(Ease.OutQuad)
        );

        _sequence.AppendInterval(_waitTime);
    }
}