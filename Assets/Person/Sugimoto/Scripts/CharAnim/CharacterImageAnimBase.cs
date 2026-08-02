using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// アニメーションのベースクラス
/// </summary>
public abstract class ChangeImageAnimBase : MonoBehaviour
{
    [Header("Image")]
    [SerializeField] private Image _image;
    [SerializeField] private ImageType _imageType;

    [Header("Sprites")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _failureSprite;
    [SerializeField] private Sprite _happySprite;
    [SerializeField] private Sprite _highScoreSprite;
    [SerializeField] private Sprite _skillImageSprite;

    [Header("Animation Target")]
    [SerializeField] protected Transform _characterTransform;

    protected Tween _animation;
    protected Vector3 _defaultLocalPosition;
    protected Quaternion _defaultLocalRotation;
    protected Vector3 _defaultLocalScale;
    private bool _hasDefaultTransform;

    protected virtual void Awake()
    {
        CacheDefaultTransform();
    }

    private void OnValidate()
    {
        ChangeImage(false);
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    /// <summary>状態を反映して、アニメーションを再生</summary>
    public void PlayImageType()
    {
        ChangeImage(true);
    }

    /// <summary>対応する画像とアニメーションを反映</summary>
    public void SetImageType(ImageType imageType)
    {
        _imageType = imageType;
        ChangeImage(true);
    }

    protected virtual void PlayNormalAnimation() { }
    protected virtual void PlayFailureAnimation() { }
    protected virtual void PlayHappyAnimation() { }
    protected virtual void PlayHighScoreAnimation() { }
    protected virtual void PlaySkillAnimation() { }

    /// <summary>Transformが設定されているかどうか</summary>
    protected bool CheckCharacterTransform()
    {
        if (_characterTransform != null)
        {
            return true;
        }

        Debug.LogWarning("Character Transformが設定されていません。", this);
        return false;
    }

    /// <summary>現在のTransformをアニメーション開始前の状態として保存します。</summary>
    protected void CacheDefaultTransform()
    {
        if (!CheckCharacterTransform())
        {
            return;
        }

        _defaultLocalPosition = _characterTransform.localPosition;
        _defaultLocalRotation = _characterTransform.localRotation;
        _defaultLocalScale = _characterTransform.localScale;
        _hasDefaultTransform = true;
    }

    /// <summary>再生中のアニメーションを停止し、Transformを初期状態に戻す</summary>
    protected void StopAnimation()
    {
        _animation?.Kill();
        _animation = null;

        if (_characterTransform == null || !_hasDefaultTransform)
        {
            return;
        }

        _characterTransform.localPosition = _defaultLocalPosition;
        _characterTransform.localRotation = _defaultLocalRotation;
        _characterTransform.localScale = _defaultLocalScale;
    }

    private void ChangeImage(bool playAnimation)
    {
        if (_image == null)
        {
            Debug.LogWarning("Imageが設定されていません。", this);
            return;
        }

        if (playAnimation)
        {
            StopAnimation();
        }

        switch (_imageType)
        {
            case ImageType.Normal:
                _image.sprite = _normalSprite;
                if (playAnimation) PlayNormalAnimation();
                break;

            case ImageType.Failure:
                _image.sprite = _failureSprite;
                if (playAnimation) PlayFailureAnimation();
                break;

            case ImageType.HappyImage:
                _image.sprite = _happySprite;
                if (playAnimation) PlayHappyAnimation();
                break;


            case ImageType.HighScore:
                _image.sprite = _highScoreSprite;
                if (playAnimation) PlayHighScoreAnimation();
                break;

            case ImageType.SkillImage:
                _image.sprite = _skillImageSprite;
                if (playAnimation) PlaySkillAnimation();
                break;
        }
    }
}
