using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
///     次に出現するピースの画像を表示するビュー
/// </summary>
public class NextPieceView : MonoBehaviour
{
    [SerializeField] private PieceSpawner _pieceSpawner;
    [SerializeField] private Image _image;
    [SerializeField] private float _moveDistance = 60f;
    [SerializeField] private float _duration = 0.12f;

    private Vector2 _basePos;

    private void OnEnable()
    {
        if (_pieceSpawner == null)
        {
            Debug.LogError("PieceSpawnerが設定されていません。");
            return;
        }

        _basePos = _image.rectTransform.anchoredPosition;
        _pieceSpawner.OnNextPieceDefinitionChanged += ShowNextPiece;
        SetSprite(_pieceSpawner.NextPieceDefinition);
        //ShowNextPiece(_pieceSpawner.NextPieceDefinition);
    }

    private void OnDisable()
    {
        if (_pieceSpawner != null)
        {
            _pieceSpawner.OnNextPieceDefinitionChanged -= ShowNextPiece;
        }

        DOTween.Kill(_image.rectTransform);
        //_image.rectTransform.anchoredPosition = _basePos;
    }

    /// <summary>
    ///     次に出現するピースの画像を表示する
    /// </summary>
    /// <param name="pieceDefinition"></param>
    private void ShowNextPiece(PieceDefinition pieceDefinition)
    {
        var rt = _image.rectTransform;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(rt.DOAnchorPosY(_basePos.y + _moveDistance, _duration));
        sequence.AppendCallback(() =>
        {
            SetSprite(pieceDefinition);
            rt.anchoredPosition = new Vector2(_basePos.x, _basePos.y - _moveDistance);
        });
        sequence.Append(rt.DOAnchorPosY(_basePos.y, _duration));
        /*
        if (_image == null)
        {
            return;
        }

        if (pieceDefinition == null ||
            pieceDefinition.Sprite == null)
        {
            _image.sprite = null;
            _image.enabled = false;
            return;
        }

        _image.sprite = pieceDefinition.Sprite;
        _image.enabled = true; */
    }
    private void SetSprite(PieceDefinition pieceDefinition)
    {
        bool hasSprite = pieceDefinition != null && pieceDefinition.Sprite != null;
        _image.sprite = hasSprite ? pieceDefinition.Sprite : null;
        _image.enabled = hasSprite;
    }
}
