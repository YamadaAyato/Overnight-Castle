using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject chackImage;
    [SerializeField] private Image backImage;
    [SerializeField] private Sprite changeImage;


    private Sprite _defaultBackSprite;
    private SelectCharacterData _selectData;
    private CharacterSelectManager _manager;
    private Vector3 _defaultScale;

    [Header("アニメーション設定")] 
    [SerializeField]private float scale = 1.2f;
    [SerializeField] private float smoleScale = 0.8f;
    [SerializeField] private float time = 0.2f;


    private int _index;

    private void Awake()
    {
        CheckNull();
        _defaultScale = transform.localScale;
        _defaultBackSprite = backImage.sprite;

        if (_button != null)
        {
            _button.transition = Selectable.Transition.None;
        }
    }

    //選択したデータを表示
    public void Select()
    {
        _manager.SelectCharacter(_selectData, _index);

        transform.DOKill();
        transform.localScale = _defaultScale;
        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOScale(scale, time));
        sequence.Append(transform.DOScale(smoleScale, time));
        sequence.Append(transform.DOScale(_defaultScale, time));
        SetSelected(true);
    }

    /// <summary>
    /// 選択時の色を変更する
    /// 多分後で消す
    /// </summary>
    public void SetSelected(bool selected)
    {
        chackImage.gameObject.SetActive(selected);
        if (selected)
        {
            backImage.sprite = changeImage;
        }
        else 
        {
            backImage.sprite = _defaultBackSprite;
        }
    }

    /// <summary>
    /// キャラクターのデータをセットする
    /// </summary>
    /// <param name="SelectData">キャラのデータ</param>
    /// <param name="manager"></param>
    public void Initialize(
        SelectCharacterData selectData,
        CharacterSelectManager manager,
        int index)
    {
        _selectData = selectData;
        _index = index;
        _manager = manager;

        _icon.sprite = _selectData.CharacterSprite;

        _button.onClick.AddListener(OnClick);

        SetSelected(false);
    }

    private void OnClick()
    {
        _manager.SelectButton(_index);
    }

    private void CheckNull()
    {
        if (_button == null)
        {
            Debug.LogError("_button が null です。");
        }
        if (_icon == null)
        {
            Debug.LogError("_icon が null です。");
        }
    }
}
