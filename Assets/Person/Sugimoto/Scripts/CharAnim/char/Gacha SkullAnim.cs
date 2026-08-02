using DG.Tweening;
using UnityEngine;

public class GachaSkullAnim : ChangeImageAnimBase
{
    [Header("Shake Animation")]
    [SerializeField] private float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeStrength = 20f;
    [SerializeField] private int _shakeVibrato = 10;
    [SerializeField, Min(0f)] private float _shakeInterval = 2f;

    [Header("Rotate Animation")]
    [SerializeField] private float _targetAngle = 20f;
    [SerializeField] private float _rotateTime = 0.3f;
    [SerializeField, Min(0f)] private float _rotateInterval = 1f;

    [Header("Up Down Animation")]
    [SerializeField] private float _moveY = 20f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField, Min(0f)] private float _moveInterval = 2f;

    private Vector2 _defaultAnchoredPosition;
    private Vector3 _defaultEulerAngles;

    protected override void Awake()
    {
        base.Awake();

        if (_characterTransform is RectTransform rectTransform)
        {
            _defaultAnchoredPosition = rectTransform.anchoredPosition;
            _defaultEulerAngles = rectTransform.localEulerAngles;
        }
    }

    protected override void PlayNormalAnimation()
    {
        PlayUpDownAnimation();
    }

    protected override void PlayFailureAnimation()
    {
        if (!TryGetCharacterRect(out RectTransform rectTransform))
        {
            return;
        }

        _animation?.Kill();

        rectTransform.anchoredPosition = _defaultAnchoredPosition;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            rectTransform.DOShakeAnchorPos(
                duration: _shakeDuration,
                strength: new Vector2(_shakeStrength, 0f),
                vibrato: _shakeVibrato,
                randomness: 0f,
                snapping: false,
                fadeOut: true
            )
        );

        sequence.AppendInterval(_shakeInterval);
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }

    protected override void PlayHappyAnimation()
    {
        PlayUpDownAnimation();
    }

    protected override void PlayHighScoreAnimation()
    {
        if (!TryGetCharacterRect(out RectTransform rectTransform))
        {
            return;
        }

        _animation?.Kill();

        rectTransform.localEulerAngles = _defaultEulerAngles;

        Vector3 targetAngle =
            _defaultEulerAngles + new Vector3(0f, 0f, _targetAngle);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            rectTransform
                .DOLocalRotate(targetAngle, _rotateTime)
                .SetEase(Ease.OutQuad)
        );

        sequence.AppendInterval(_rotateInterval);

        sequence.Append(
            rectTransform
                .DOLocalRotate(_defaultEulerAngles, _rotateTime)
                .SetEase(Ease.InQuad)
        );

        sequence.AppendInterval(_rotateInterval);
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }

    protected override void PlaySkillAnimation()
    {
        PlayUpDownAnimation();
    }

    private void PlayUpDownAnimation()
    {
        if (!TryGetCharacterRect(out RectTransform rectTransform))
        {
            return;
        }

        _animation?.Kill();

        rectTransform.anchoredPosition = _defaultAnchoredPosition;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            rectTransform
                .DOAnchorPosY(
                    _defaultAnchoredPosition.y + _moveY,
                    _moveDuration
                )
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            rectTransform
                .DOAnchorPosY(
                    _defaultAnchoredPosition.y,
                    _moveDuration
                )
                .SetEase(Ease.InOutSine)
        );

        sequence.AppendInterval(_moveInterval);
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }

    private bool TryGetCharacterRect(out RectTransform rectTransform)
    {
        rectTransform = null;

        if (!CheckCharacterTransform())
        {
            return false;
        }

        rectTransform = _characterTransform as RectTransform;

        if (rectTransform == null)
        {
            Debug.LogError(
                "_characterTransformがRectTransformではありません。",
                this
            );
            return false;
        }

        return true;
    }
}