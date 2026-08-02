using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CarpenterBeaverAnim : ChangeImageAnimBase
{
    [Header("Normal Animation")]
    [SerializeField] private float _rotateAngle = 15f;
    [SerializeField] private float _rotateDuration = 1f;
    [SerializeField, Min(0f)] private float _rotateInterval = 2f;
    [Header("Failure Animation")]
    [SerializeField] private float _rotateZAmount = 100f;
    [SerializeField] private float _rotatezDuration = 0.5f;
    [Header("Happy Animation")]
    [SerializeField] private float _moveLeftAmount = 200f;
    [SerializeField] private float _moveDuration = 0.5f;

    [SerializeField] private Image[] _normalObjs;
    [SerializeField] private Image[] _skillObjs;



    private Vector2 _defaultAnchoredPosition;

    protected override void Awake()
    {
        base.Awake();

        if (_characterTransform is RectTransform rectTransform)
        {
            _defaultAnchoredPosition = rectTransform.anchoredPosition;
        }
    }

    protected override void PlayNormalAnimation()
    {
        SetSkillObjects(false);
        PlayUpDownAnimation();
    }

    /// <summary>
    /// 悪いものが降ってきたときのアニメーション
    /// </summary>
    protected override void PlayFailureAnimation()
    {
        SetSkillObjects(false);
        if (!CheckCharacterTransform())
        {
            return;
        }

        _animation?.Kill();

        float targetZ =
            _characterTransform.localEulerAngles.z + _rotateZAmount;

        _animation = _characterTransform
            .DOLocalRotate(
                new Vector3(
                    _characterTransform.localEulerAngles.x,
                    _characterTransform.localEulerAngles.y,
                    targetZ
                ),
                _rotatezDuration
            )
            .SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// いいものが降ってきたときのアニメーション
    /// </summary>
    protected override void PlayHappyAnimation()
    {
        SetSkillObjects(false);
        if (!CheckCharacterTransform())
        {
            return;
        }

        _animation?.Kill();

        RectTransform rectTransform =
            _characterTransform as RectTransform;

        if (rectTransform == null)
        {
            Debug.LogError(
                "_characterTransformがRectTransformではありません。",
                this
            );
            return;
        }

        rectTransform.anchoredPosition = _defaultAnchoredPosition;

        _animation = rectTransform
            .DOAnchorPosX(
                _defaultAnchoredPosition.x - _moveLeftAmount,
                _moveDuration
            )
            .SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// ハイスコア状態のアニメーション
    /// </summary>
    protected override void PlayHighScoreAnimation()
    {
        SetSkillObjects(true);
    }

    /// <summary>
    /// スキル状態のアニメーション
    /// </summary>
    protected override void PlaySkillAnimation()
    {
        SetSkillObjects(true);
    }

    private void PlayUpDownAnimation()
    {
        SetSkillObjects(false);
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
    /// Normal用とSkill用のオブジェクトを切り替える
    /// </summary>
    private void SetSkillObjects(bool isSkill)
    {
        SetActiveObjects(_normalObjs, !isSkill);
        SetActiveObjects(_skillObjs, isSkill);
    }

    /// <summary>
    /// 配列内のオブジェクトをまとめて表示・非表示にする
    /// </summary>
    private void SetActiveObjects(Image[] objects, bool isActive)
    {
        if (objects == null)
        {
            return;
        }

        foreach (Image obj in objects)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(isActive);
            }
        }
    }
}
