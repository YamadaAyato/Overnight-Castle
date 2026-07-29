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

    [SerializeField] private SelectionController _selectionController;

    /// <summary>
    /// 選択したキャラクターデータ
    /// </summary>
    public SelectCharacterData _selectedCharacter { get; private set; }

    /// <summary>
    /// 選択したユニットをセットする
    /// </summary>
    /// <param name="character"></param>
    public void SelectCharacter(SelectCharacterData character, int index)
    {
        _selectedCharacter = character;

        _characterImage.gameObject.SetActive(true);
        _characterImage.sprite = character.CharacterImage;
        _characterName.text = character.CharacterName;
        _description.text = character.SkillDescription;
    }

    /// <summary>
    /// キモティーモ
    /// </summary>
    /// <param name="index"></param>
    public void SelectButton(int index)
    {
        _selectionController.Select(index);
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
        CharacterSelectButton[] buttons =
            new CharacterSelectButton[_selectCharacterDatas.Length];

        for (int i = 0; i < _selectCharacterDatas.Length; i++)
        {
            CharacterSelectButton button =
                Instantiate(_chairSelectButton, _buttonParent);

            button.Initialize(_selectCharacterDatas[i], this, i);

            buttons[i] = button;
        }

        _selectionController.Initialize(buttons);
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

        if (_selectionController == null)
        {
            Debug.LogError("_selectionController が null です。");
        }
    }
}
