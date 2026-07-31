using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImageAnim : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image _image;
    [SerializeField] private ImageType _imageType;

    [Header("Sprites")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _failureSprite;
    [SerializeField] private Sprite _highScoreSprite;
    [SerializeField] private Sprite _skillImageSprite;

    [Header("Animation Target")]
    [SerializeField] private Transform _characterTransform;

    [Header("Normal Animation")]
    [SerializeField] private float _rotateAngle = 360f;
    [SerializeField] private float _rotateDuration = 1f;
    [SerializeField, Min(0f)] private float _rotateInterval = 2f;

    [Header("Up Down Animation")]
    [SerializeField] private float _moveY = 20f;
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField, Min(0f)] private float _moveInterval = 2f;

    private Tween _animation;
    private Vector3 _defaultLocalPosition;
    private Quaternion _defaultLocalRotation;


    public void PrayImageType() 
    {
        ChangeImage(true);
    }

    /// <summary>
    /// キャラの状態を変化させる
    /// </summary>
    /// <param name="imageType"></param>
    public void SetImageType(ImageType imageType)
    {
        _imageType = imageType;
        ChangeImage(true);
    }

    private void Awake()
    {
        if (_characterTransform == null)
        {
            return;
        }

        _defaultLocalPosition = _characterTransform.localPosition;
        _defaultLocalRotation = _characterTransform.localRotation;
    }
    private void OnValidate()
    {
        // Inspector上では画像だけ変更する
        ChangeImage(false);
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    /// <summary>
    /// 画像とアニメーション
    /// </summary>
    /// <param name="playAnimation"></param>
    private void ChangeImage(bool playAnimation)
    {
        if (_image == null)
        {
            return;
        }

        // 前のアニメーションを停止して状態を戻す
        StopAnimation();

        switch (_imageType)
        {
            case ImageType.Normal:
                _image.sprite = _normalSprite;

                if (playAnimation)
                {
                    PlayNormalAnimation();
                }
                break;

            case ImageType.Failure:
                _image.sprite = _failureSprite;

                if (playAnimation)
                {
                    PlayFailureAnimation();
                }
                break;

            case ImageType.HighScore:
                _image.sprite = _highScoreSprite;

                if (playAnimation)
                {
                    PlayHighScoreAnimation();
                }
                break;

            case ImageType.SkillImage:
                _image.sprite = _skillImageSprite;

                if (playAnimation)
                {
                    PlaySkillAnimation();
                }
                break;
        }
    }

    /// <summary>
    /// ノーマル状態のうなずくアニメーション
    /// </summary>
    private void PlayNormalAnimation()
    {
        if (_characterTransform == null)
        {
            Debug.LogWarning("Character Transformが設定されていません。", this);
            return;
        }

        Sequence sequence = DOTween.Sequence();

        // 指定角度まで傾く → 元の角度へ戻る
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

        // 次のアニメーションまで待機
        sequence.AppendInterval(_rotateInterval);

        // 「傾く→戻る→待機」を無限ループ
        sequence.SetLoops(-1, LoopType.Restart);

        _animation = sequence;
    }

    private void PlayFailureAnimation()
    {

    }

    private void PlayHighScoreAnimation()
    {

    }

    private void PlaySkillAnimation()
    {
        if (_characterTransform == null)
        {
            Debug.LogWarning(
                "Character Transformが設定されていません。",
                this
            );
            return;
        }

        _animation?.Kill();

        _characterTransform.localPosition = _defaultLocalPosition;

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

    /// <summary>
    /// アニメーションが重複しないように全部直す
    /// </summary>
    private void StopAnimation()
    {
        _animation?.Kill();
        _animation = null;

        if (_characterTransform == null)
        {
            return;
        }
    }
}