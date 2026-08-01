using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutInAnimation : MonoBehaviour
{
    [SerializeField] private Image _cutInImage;
    [SerializeField] private float _showTime = 0.3f;
    [SerializeField] private float _waitTime = 1f;
    [SerializeField] private float _hideTime = 0.3f;

    private Sequence _sequence;

    public void PlayCutIn()
    {
        _sequence?.Kill();

        _cutInImage.gameObject.SetActive(true);

        // 左から右へ表示する設定
        _cutInImage.fillOrigin = (int)Image.OriginHorizontal.Right;
        _cutInImage.fillAmount = 0f;

        _sequence = DOTween.Sequence();

        // 0から1まで表示
        _sequence.Append(
            _cutInImage.DOFillAmount(1f, _showTime)
                .SetEase(Ease.OutQuad)
        );

        // 表示したまま少し待つ
        _sequence.AppendInterval(_waitTime);

        // 右から左へ消えるように変更
        _sequence.AppendCallback(() =>
        {
            _cutInImage.fillOrigin =
                (int)Image.OriginHorizontal.Left;
        });

        // 1から0まで消す
        _sequence.Append(
            _cutInImage.DOFillAmount(0f, _hideTime)
                .SetEase(Ease.InQuad)
        );

        _sequence.OnComplete(() =>
        {
            _cutInImage.gameObject.SetActive(false);
        });
    }
}