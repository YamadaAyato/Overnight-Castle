using DG.Tweening;
using UnityEngine;

public class ChangeImageAnim : ChangeImageAnimBase
{
    [Header("Normal Animation")]
    [SerializeField] private float _rotateAngle = 15f;
    [SerializeField] private float _rotateDuration = 1f;
    [SerializeField, Min(0f)] private float _rotateInterval = 2f;

    [Header("Skill Up Down Animation")]
    [SerializeField] private float _moveY = 20f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField, Min(0f)] private float _moveInterval = 2f;

    /// <summary>
    /// ノーマル状態のアニメーション
    /// </summary>
    protected override void PlayNormalAnimation()
    {
        if (!CheckCharacterTransform())
        {
            return;
        }

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            _characterTransform
                .DOLocalRotate(
                    new Vector3(0f, 0f, _rotateAngle),
                    _rotateDuration
                )
                .SetRelative()
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
        );

        sequence.AppendInterval(_rotateInterval);
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }

    /// <summary>
    /// 失敗状態のアニメーション
    /// </summary>
    protected override void PlayFailureAnimation()
    {

    }

    /// <summary>
    /// ハイスコア状態のアニメーション
    /// </summary>
    protected override void PlayHighScoreAnimation()
    {

    }

    /// <summary>
    /// スキル状態の上下アニメーション
    /// </summary>
    protected override void PlaySkillAnimation()
    {
        if (!CheckCharacterTransform())
        {
            return;
        }

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