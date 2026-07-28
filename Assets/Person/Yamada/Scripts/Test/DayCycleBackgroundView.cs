using DG.Tweening;
using UnityEngine;

/// <summary>
///     夜から朝へ変化する背景を横方向へ移動させるクラス
/// </summary>
public class DayCycleBackgroundView : MonoBehaviour
{
    /// <summary>
    ///     指定した時間をかけて背景の移動を開始する
    /// </summary>
    /// <param name="duration">背景が最後まで移動する時間</param>
    public void Play(float duration)
    {
        if (_backgroundRoot == null ||
            _backgroundPanels == null ||
            _backgroundPanels.Length == 0)
        {
            Debug.LogError(
                "背景の参照が設定されていません。",
                this);

            return;
        }

        _moveTween?.Kill();

        ResetPosition();

        if (duration <= 0f)
        {
            Complete();
            return;
        }

        float destinationX =
            _initialRootLocalPosition.x -
            _moveDistance;

        _moveTween = _backgroundRoot
            .DOLocalMoveX(destinationX, duration)
            .SetEase(Ease.Linear)
            .SetUpdate(_useUnscaledTime)
            .SetLink(
                gameObject,
                LinkBehaviour.KillOnDestroy);
    }

    /// <summary>
    ///     背景の移動を停止する
    /// </summary>
    public void Stop()
    {
        _moveTween?.Kill();
        _moveTween = null;
    }

    /// <summary>
    ///     背景を朝の位置まで移動させる
    /// </summary>
    public void Complete()
    {
        _moveTween?.Kill();
        _moveTween = null;

        if (_backgroundRoot == null)
        {
            return;
        }

        Vector3 position =
            _initialRootLocalPosition;

        position.x -= _moveDistance;

        _backgroundRoot.localPosition = position;
    }

    /// <summary>
    ///     背景を夜の初期位置へ戻す
    /// </summary>
    public void ResetPosition()
    {
        _moveTween?.Kill();
        _moveTween = null;

        if (_backgroundRoot == null)
        {
            return;
        }

        _backgroundRoot.localPosition =
            _initialRootLocalPosition;
    }

    [Header("背景設定")]
    [SerializeField] private Transform _backgroundRoot;
    [SerializeField] private SpriteRenderer[] _backgroundPanels;

    [Header("配置設定")]
    [SerializeField, Min(0f), Tooltip("背景画像同士の間隔")]
    private float _panelSpacing;

    [Header("再生設定")]
    [SerializeField, Tooltip("TimeScaleの影響を受けずに移動させるか")]
    private bool _useUnscaledTime;

    private Tween _moveTween;

    private Vector3 _initialRootLocalPosition;
    private float _moveDistance;

    private void Awake()
    {
        if (_backgroundRoot == null)
        {
            _backgroundRoot = transform;
        }

        _initialRootLocalPosition =
            _backgroundRoot.localPosition;

        LayoutPanels();
        ResetPosition();
        Play(60f);
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
    }

    /// <summary>
    ///     背景画像を横一列に配置する
    /// </summary>
    [ContextMenu("背景画像を横に整列")]
    private void LayoutPanels()
    {
        if (_backgroundPanels == null ||
            _backgroundPanels.Length == 0)
        {
            _moveDistance = 0f;
            return;
        }

        float currentRightEdge = 0f;
        float lastPanelCenterX = 0f;
        bool isFirstPanel = true;

        foreach (SpriteRenderer panel
                 in _backgroundPanels)
        {
            if (panel == null ||
                panel.sprite == null)
            {
                continue;
            }

            float panelWidth =
                panel.sprite.bounds.size.x *
                Mathf.Abs(panel.transform.localScale.x);

            float centerX;

            if (isFirstPanel)
            {
                centerX = 0f;
                currentRightEdge = panelWidth / 2f;
                isFirstPanel = false;
            }
            else
            {
                centerX =
                    currentRightEdge +
                    _panelSpacing +
                    panelWidth / 2f;

                currentRightEdge =
                    centerX +
                    panelWidth / 2f;
            }

            Vector3 position =
                panel.transform.localPosition;

            position.x = centerX;
            panel.transform.localPosition = position;

            lastPanelCenterX = centerX;
        }

        // 最初の背景中央から最後の背景中央までの距離を設定する
        _moveDistance = lastPanelCenterX;
    }
}