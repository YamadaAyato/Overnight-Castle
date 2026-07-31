using DG.Tweening;
using UnityEngine;

/// <summary>
///     夜から朝へ変化する背景を横方向へ移動させるクラス
/// </summary>
public class DayCycleBackgroundView : MonoBehaviour
{
    /// <summary>
    ///     タイマーの時間に応じて背景画像をスクロールさせる
    /// </summary>
    /// <param name="timer">スクロールに使用するタイマー</param>
    public void StartScroll(Timer timer)
    {
        if(timer == null)
        {
            Debug.LogError("Timerが設定されていません", this);
            return;
        }

        if (!CalculateScrollRange())
        {
            return;
        }

        _timer = timer;
        _timer.OnTimeAdded += HandleTimeAdded;

        // 背景画像の位置を初期化
        Vector2 position = _backgroundImage.anchoredPosition;
        position.x = 0f;
        _backgroundImage.anchoredPosition = position;

        StartScrollTween(_timer.CurrentTime);
    }

    [SerializeField] private RectTransform _viewport;
    [SerializeField] private RectTransform _backgroundImage;
    [SerializeField] private Ease _scrollEase = Ease.Linear;

    private Tween _scrollTween;
    private Timer _timer;
    private float _endPositionX;

    /// <summary>
    ///     タイマーに時間が追加されたときの処理
    /// </summary>
    /// <param name="addedTime">追加された時間</param>
    private void HandleTimeAdded(float addedTime)
    {
        // タイマーの時間に応じてスクロールを再開
        StartScrollTween(_timer.CurrentTime);
    }

    /// <summary>
    ///     タイマーの時間に応じて背景画像をスクロールさせるTweenを開始する
    /// </summary>
    /// <param name="duration">スクロールにかかる時間</param>
    private void StartScrollTween(float duration)
    {
        _scrollTween?.Kill();

        // タイマーの時間が0以下の場合は、背景画像を終了位置に移動させる
        if (duration <= 0f)
        {
            Vector2 position = _backgroundImage.anchoredPosition;
            position.x = _endPositionX;
            _backgroundImage.anchoredPosition = position;
            return;
        }

        // 背景画像を終了位置までスクロールさせるTweenを開始
        _scrollTween = _backgroundImage
            .DOAnchorPosX(_endPositionX, duration)
            .SetEase(_scrollEase)
            // Tweenが終了したときに、背景画像を終了位置に固定する
            .SetLink(gameObject);
    }

    /// <summary>
    ///     背景画像のスクロール範囲を計算する
    /// </summary>
    /// <returns>スクロール範囲の計算に成功したかどうか</returns>
    private bool CalculateScrollRange()
    {
        if(_viewport == null)
        {
            Debug.LogError("Viewportが設定されていません", this);
            return false;
        }

        if(_backgroundImage == null)
        {
            Debug.LogError("BackgroundImageが設定されていません", this);
            return false;
        }
        
        Canvas.ForceUpdateCanvases();

        float moveDistance = _backgroundImage.rect.width - _viewport.rect.width;

        if(moveDistance <= 0)
        {
            Debug.LogError("背景画像の幅がビューポートの幅より小さいため、スクロールできません", this);
            return false;
        }

        _endPositionX = -moveDistance;
        return true;
    }
}