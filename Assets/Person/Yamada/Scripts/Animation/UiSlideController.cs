using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     操作説明のスライドを管理するクラス
/// </summary>
public class UiSlideController : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _previousButton;
    [SerializeField, Min(0f)] private float _pageWidth;
    [SerializeField, Min(0f)] private float _slideDuration;
    [SerializeField] private Ease _ease;

    private int _currentPageIndex;
    private int _pageCount;
    private Tween _tween;

    /// <summary>
    ///     次のページへ移動する
    /// </summary>
    public void NextPage()
    {
        if (_currentPageIndex >= _pageCount - 1 ||
            _tween != null && _tween.IsActive() && _tween.IsPlaying())
        {
            return;
        }

        SwitchPage(_currentPageIndex + 1);
    }

    /// <summary>
    ///     前のページへ移動する
    /// </summary>
    public void PreviousPage()
    {
        if (_currentPageIndex <= 0 ||
            _tween != null && _tween.IsActive() && _tween.IsPlaying())
        {
            return;
        }

        SwitchPage(_currentPageIndex - 1);
    }

    /// <summary>
    ///     指定したページへ移動する
    /// </summary>
    /// <param name="pageIndex">移動先のページインデックス</param>
    public void SwitchPage(int pageIndex)
    {
        if (pageIndex < 0 ||
            pageIndex >= _pageCount)
        {
            return;
        }

        _currentPageIndex = pageIndex;

        Vector2 targetPosition = _content.anchoredPosition;
        targetPosition.x = -_currentPageIndex * _pageWidth;

        _tween?.Kill();

        SetButtonsInteractable(false);

        _tween = _content
            .DOAnchorPos(targetPosition, _slideDuration)
            .SetEase(_ease)
            .SetLink(gameObject)
            .OnComplete(UpdateButtons);
    }

    private void Start()
    {
        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        _pageCount = _content.childCount;

        if (_pageCount == 0)
        {
            Debug.LogError("Contentにページが設定されていません。", this);
            enabled = false;
            return;
        }

        _currentPageIndex = 0;

        Vector2 position = _content.anchoredPosition;
        position.x = 0f;
        _content.anchoredPosition = position;

        UpdateButtons();
    }

    private void OnDestroy()
    {
        _tween?.Kill();
    }

    /// <summary>
    ///     ボタンの状態を更新する
    /// </summary>
    private void UpdateButtons()
    {
        _previousButton.interactable = _currentPageIndex > 0;
        _nextButton.interactable = _currentPageIndex < _pageCount - 1;
    }

    /// <summary>
    ///     両方のボタンの操作可否を設定する
    /// </summary>
    /// <param name="interactable">操作可能にする場合はtrue、それ以外はfalse</param>
    private void SetButtonsInteractable(bool interactable)
    {
        _previousButton.interactable = interactable;
        _nextButton.interactable = interactable;
    }

    /// <summary>
    ///     必要な設定が存在するか確認する
    /// </summary>
    /// <returns>設定が有効な場合はtrue、それ以外はfalse</returns>
    private bool ValidateSettings()
    {
        if (_content == null)
        {
            Debug.LogError("Contentが設定されていません。", this);
            return false;
        }

        if (_nextButton == null)
        {
            Debug.LogError("NextButtonが設定されていません。", this);
            return false;
        }

        if (_previousButton == null)
        {
            Debug.LogError("PreviousButtonが設定されていません。", this);
            return false;
        }

        if (_pageWidth <= 0f)
        {
            Debug.LogError("PageWidthが0以下です。", this);
            return false;
        }

        return true;
    }
}