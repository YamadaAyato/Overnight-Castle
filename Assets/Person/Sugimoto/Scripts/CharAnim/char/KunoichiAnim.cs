using DG.Tweening;
using UnityEngine;

public class KunoichiAnim : ChangeImageAnimBase
{
    [Header("Skill / High Score Animation")]
    [SerializeField] private float _moveY = 20f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField, Min(0f)] private float _moveInterval = 2f;

    [Header("Normal Car Animation")]
    [SerializeField] private float _moveX = 100f;
    [SerializeField] private float _moveXDuration = 0.3f;
    [SerializeField, Min(0f)] private float _moveXInterval = 1f;
    [SerializeField] private RectTransform _KunoichiTransform;

    [Header("Failure Animation")]
    [SerializeField] private float _moveDownAmount = 200f;
    [SerializeField] private float _downDuration = 0.5f;

    /// <summary>
    /// ノーマル状態のアニメーション
    /// </summary>
    protected override void PlayNormalAnimation()
    {
        if (_KunoichiTransform == null)
        {
            Debug.LogError("_carTransformが設定されていません。", this);
            return;
        }

        _animation?.Kill();

        Vector2 defaultPosition = _KunoichiTransform.anchoredPosition;
        float leftX = defaultPosition.x - _moveX;
        float rightX = defaultPosition.x + _moveX;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            _KunoichiTransform
                .DOAnchorPosX(leftX, _moveXDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            _KunoichiTransform
                .DOAnchorPosX(defaultPosition.x, _moveXDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            _KunoichiTransform
                .DOAnchorPosX(rightX, _moveXDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            _KunoichiTransform
                .DOAnchorPosX(defaultPosition.x, _moveXDuration)
                .SetEase(Ease.InOutSine)
        );

        sequence.AppendInterval(_moveXInterval);
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }

    /// <summary>
    /// 悪いものが降ってきたときのアニメーション
    /// </summary>
    protected override void PlayFailureAnimation()
    {
        if (!CheckCharacterTransform())
        {
            return;
        }

        _animation?.Kill();

        float targetY = _defaultLocalPosition.y - _moveDownAmount;

        _animation = _characterTransform
            .DOLocalMoveY(targetY, _downDuration)
            .SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// いいものが降ってきたときのアニメーション
    /// </summary>
    protected override void PlayHappyAnimation()
    {
        // 必要になったらここに追加
    }

    /// <summary>
    /// ハイスコア状態のアニメーション
    /// </summary>
    protected override void PlayHighScoreAnimation()
    {
        PlayUpDownAnimation();
    }

    /// <summary>
    /// スキル状態の上下アニメーション
    /// </summary>
    protected override void PlaySkillAnimation()
    {
        PlayUpDownAnimation();
    }

    private void PlayUpDownAnimation()
    {
        if (!CheckCharacterTransform())
        {
            return;
        }

        _animation?.Kill();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            _characterTransform
                .DOLocalMoveY(
                    _defaultLocalPosition.y + _moveY,
                    _moveDuration
                )
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            _characterTransform
                .DOLocalMoveY(
                    _defaultLocalPosition.y,
                    _moveDuration
                )
                .SetEase(Ease.InOutSine)
        );

        sequence.AppendInterval(_moveInterval);
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }
}