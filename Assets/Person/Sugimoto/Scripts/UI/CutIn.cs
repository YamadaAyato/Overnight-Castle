using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CutInAnimation : MonoBehaviour
{
    [Header("カットイン設定")]
    [SerializeField] private GameObject _cutInPanel;
    [SerializeField] private RectMask2D _rectMask;
    [SerializeField] private Image _charHead;
    [SerializeField] private Image _charBody;

    [Header("スキルアイコン設定")]
    [SerializeField] private Image _firstSkillIcon;
    [SerializeField] private Image _secondSkillIcon;
    [SerializeField] private Color _usedSkillColor = Color.black;

    [Header("アニメーション設定")]
    [SerializeField] private float _showTime = 0.3f;
    [SerializeField] private float _waitTime = 1f;
    [SerializeField] private float _hideTime = 0.3f;

    private CharacterSkillController _characterSkillController;
    private Sequence _sequence;
    private Vector4 _defaultPadding;

    private void Awake()
    {
        _defaultPadding = _rectMask.padding;
        _cutInPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        _characterSkillController.OnSkillUsed -= HandleSkillUsed;
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="image"></param>
    /// <param name="image2"></param>
    public bool Initialize(CharacterSkillController characterSkillController, Sprite image, Sprite image2)
    {
        if (characterSkillController == null)
        {
            Debug.LogError("CharacterSkillControllerが設定されていません", this);
            return false;
        }
        _characterSkillController = characterSkillController;
        _characterSkillController.OnSkillUsed += HandleSkillUsed;

        _charHead.sprite = image;
        _charBody.sprite = image2;

        ResetSkillIcons();
        return true;
    }

    private void HandleSkillUsed(CharacterSkillSlot characterSkillSlot, CharacterSkillDefinition characterSkillDefinition)
    {
        SetSkillIconUsed(characterSkillSlot);
        PlayCutIn();
    }

    private void SetSkillIconUsed(CharacterSkillSlot skillSlot)
    {
        if (skillSlot == CharacterSkillSlot.Skill1)
        {
            _firstSkillIcon.color = _usedSkillColor;
        }
        else if (skillSlot == CharacterSkillSlot.Skill2)
        {
            _secondSkillIcon.color = _usedSkillColor;
        }
    }

    private void ResetSkillIcons()
    {
        _firstSkillIcon.color = Color.white;
        _secondSkillIcon.color = Color.white;
    }

    /// <summary>
    /// カットインアニメーション
    /// </summary>
    public void PlayCutIn()
    {
        _rectMask.padding = _defaultPadding;

        _sequence?.Kill();

        _cutInPanel.SetActive(true);

        _sequence = DOTween.Sequence();

        // padding.xを現在値から200fまで変化させる
        _sequence.Append(
            DOTween.To(
                () => _rectMask.padding.x,
                value =>
                {
                    Vector4 padding = _rectMask.padding;
                    padding.x = value;
                    _rectMask.padding = padding;
                },
                200f,
                _showTime
            ).SetEase(Ease.OutQuad)
        );

        _sequence.Append(
            DOTween.To(
                () => _rectMask.padding.z,
                value =>
                {
                    Vector4 padding = _rectMask.padding;
                    padding.z = value;
                    _rectMask.padding = padding;
                },
                800f,
                _showTime
            ).SetEase(Ease.OutQuad)
        );

        _sequence.AppendInterval(_waitTime);
    }
}