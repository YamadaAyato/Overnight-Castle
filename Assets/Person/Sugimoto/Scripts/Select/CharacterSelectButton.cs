using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;

    private SelectCharacterData _selectData;
    private CharacterSelectManager _manager;
    private Vector3 _defaultScale;
    private Image _image;

    private int _index;

    private void Awake()
    {
        CheckNull();
        _image = GetComponent<Image>();
        _defaultScale = transform.localScale;

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
        transform.DOScale(1.2f, 0.2f)
.       SetLoops(2, LoopType.Yoyo);
        SetSelected(true);
    }

    /// <summary>
    /// 選択時の色を変更する
    /// 多分後で消す
    /// </summary>
    public void SetSelected(bool selected)
    {

        _image.color = selected
            ? Color.red
            : Color.white;
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
