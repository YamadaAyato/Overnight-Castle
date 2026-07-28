using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// キャラクター選択の管理クラス
/// </summary>
public class CharacterSelectManager : MonoBehaviour
{
    [Header("Characters")]
    [SerializeField] private SelectCharacterData[] _selectCharacterDatas;

    [Header("Button")]
    [SerializeField] private CharacterSelectButton _chairSelectButton;
    [SerializeField] private Transform _buttonParent;

    [Header("Preview")]
    [SerializeField] private Image _characterImage;
    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private TMP_Text _description;

    /// <summary>
    /// 選択したキャラクターデータ
    /// </summary>
    public SelectCharacterData _selectedCharacter { get; private set; }

    /// <summary>
    /// 選択したユニットをセットする
    /// </summary>
    /// <param name="character"></param>
    public void SelectCharacter(SelectCharacterData character)
    {
        _selectedCharacter = character;

        _characterImage.gameObject.SetActive(true);
        _characterImage.sprite = character.CharacterImage;
        _characterName.text = character.CharacterName;
        _description.text = character.SkillDescription;
    }

    private void Awake()
    {
        CheckNull();
    }


    private void Start()
    {
        CreateCharacterButtons();
        _characterImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// キャラクターのボタンを生成
    /// </summary>
    private void CreateCharacterButtons()
    {
        foreach (SelectCharacterData selectData in _selectCharacterDatas)
        {
            CharacterSelectButton button =
                Instantiate(_chairSelectButton, _buttonParent);

            button.Initialize(selectData, this);
        }
    }

    private void CheckNull()
    {
        if (_selectCharacterDatas == null)
        {
            Debug.LogError("_selectCharacterDatas が null です。");
        }

        if (_chairSelectButton == null)
        {
            Debug.LogError("_chairSelectButton が null です。");
        }

        if (_buttonParent == null)
        {
            Debug.LogError("_buttonParent が null です。");
        }

        if (_characterImage == null)
        {
            Debug.LogError("_characterImage が null です。");
        }

        if (_characterName == null)
        {
            Debug.LogError("_characterName が null です。");
        }

        if (_description == null)
        {
            Debug.LogError("_description が null です。");
        }
    }
}
