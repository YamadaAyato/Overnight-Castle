using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;

    private SelectCharacterData _selectData;
    private CharacterSelectManager _manager;

    private void Awake()
    {
        CheckNull();
    }

    /// <summary>
    /// キャラクターのデータをセットする
    /// </summary>
    /// <param name="SelectData">キャラのデータ</param>
    /// <param name="manager"></param>
    public void Initialize(
        SelectCharacterData SelectData,
        CharacterSelectManager manager)
    {
        _selectData = SelectData;
        _manager = manager;

        _icon.sprite = SelectData.CharacterImage;

        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        _manager.SelectCharacter(_selectData);
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