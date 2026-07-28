using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     次に出現するピースの画像を表示するビュー
/// </summary>
public class NextPieceView : MonoBehaviour
{
    [SerializeField] private PieceSpawner _pieceSpawner;
    [SerializeField] private Image _image;

    private void OnEnable()
    {
        if (_pieceSpawner == null)
        {
            Debug.LogError("PieceSpawnerが設定されていません。");
            return;
        }
        _pieceSpawner.OnNextPieceDefinitionChanged += ShowNextPiece;
        ShowNextPiece(_pieceSpawner.NextPieceDefinition);
    }

    private void OnDisable()
    {
        if (_pieceSpawner != null)
        {
            _pieceSpawner.OnNextPieceDefinitionChanged -= ShowNextPiece;
        }
    }

    /// <summary>
    ///     次に出現するピースの画像を表示する
    /// </summary>
    /// <param name="pieceDefinition"></param>
    private void ShowNextPiece(PieceDefinition pieceDefinition)
    {
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
        _image.enabled = true;
    }
}
