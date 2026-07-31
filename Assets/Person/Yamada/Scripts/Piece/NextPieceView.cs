using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
///     次に出現するピースの画像を表示するビュー
///     Next表示用の画像を下から上に流れる演出
/// </summary>
public class NextPieceView : MonoBehaviour
{
    [SerializeField] private PieceSpawner _pieceSpawner;
    [SerializeField] private Image _image;

    [Header("アニメーションでどれだけ上下に動かすか")]
    [SerializeField] private float _moveDistance = 60f;

    [Header("移動1回あたりの時間")]
    [SerializeField] private float _duration = 0.12f;

    //元の画像の位置を覚えておく変数
    private Vector2 _basePos;

    private void OnEnable()
    {
        if (_pieceSpawner == null)
        {
            Debug.LogError("PieceSpawnerが設定されていません。");
            return;
        }

        //画像の位置を「元の位置」として記録する
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
    ///     _duration秒かけて移動上に移動する演出
    /// </summary>
    /// <param name="pieceDefinition"></param>
    private void ShowNextPiece(PieceDefinition pieceDefinition)
    {
        var rt = _image.rectTransform;

        //上に移動
        Sequence sequence = DOTween.Sequence();
        sequence.Append(rt.DOAnchorPosY(_basePos.y + _moveDistance, _duration));

        //スプライトを新しいものに差し替え、下に次のピースを移動させる
        sequence.AppendCallback(() =>
        {
            SetSprite(pieceDefinition);
            rt.anchoredPosition = new Vector2(_basePos.x, _basePos.y - _moveDistance);
        });
        //下からもとのy座標まで移動
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
    /// <summary>
    /// ピースの画像を表示する
    /// </summary>
    /// <param name="pieceDefinition"></param>
    private void SetSprite(PieceDefinition pieceDefinition)
    {
        if (pieceDefinition == null || pieceDefinition.Sprite == null)
        {
            _image.sprite = null;
            _image.enabled = false;
            return;
        }

        _image.sprite = pieceDefinition.Sprite;
        _image.enabled = true;
        /*bool hasSprite = pieceDefinition != null && pieceDefinition.Sprite != null;
        _image.sprite = hasSprite ? pieceDefinition.Sprite : null;
        _image.enabled = hasSprite;*/
    }
}
