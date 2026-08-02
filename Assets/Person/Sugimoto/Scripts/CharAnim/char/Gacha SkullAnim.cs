using DG.Tweening;
using UnityEngine;

public class GachaSkullAnim : ChangeImageAnimBase
{

    [Header("Skill Up Down Animation")]
    [SerializeField] private float _moveY = 20f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField, Min(0f)] private float _moveInterval = 2f;
    protected override void PlayNormalAnimation()
    {
        // Normal画像のアニメーションを書く
    }

    protected override void PlayFailureAnimation()
    {
        // Failure画像のアニメーションを書く
    }

    protected override void PlayHappyAnimation()
    {
        // Happy画像のアニメーションを書く
    }

    protected override void PlayHighScoreAnimation()
    {
        // HighScore画像のアニメーションを書く
    }

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
