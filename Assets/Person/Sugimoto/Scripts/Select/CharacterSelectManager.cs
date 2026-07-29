using GameFoundation.Runtime.Attributers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///     キャラクター選択を管理するクラス
/// </summary>
public class CharacterSelectManager : MonoBehaviour
{
    /// <summary> 選択したキャラクターデータ </summary>
    public SelectCharacterData SelectedCharacter { get; private set; }

    [Header("キャラクター設定")]
    [SerializeField] private SelectCharacterData[] _selectCharacterDatas;

    [Header("ボタン設定")]
    [SerializeField] private CharacterSelectButton _characterSelectButtonPrefab;
    [SerializeField] private Transform _buttonParent;

    [Header("キャラクタープレビュー設定")]
    [SerializeField] private Image _characterImage;
    [SerializeField] private TMP_Text _characterName;

    [Header("スキル1表示設定")]
    [SerializeField] private TMP_Text _skillName1;
    [SerializeField] private TMP_Text _skillDescription1;

    [Header("スキル2表示設定")]
    [SerializeField] private TMP_Text _skillName2;
    [SerializeField] private TMP_Text _skillDescription2;

    [Header("選択設定")]
    [SerializeField] private SelectionController _selectionController;

    [Header("シーン設定")]
    [SerializeField, SceneNameSelector] private string _inGameSceneName;

    /// <summary>
    ///     選択したキャラクターデータを設定する
    /// </summary>
    /// <param name="character">選択したキャラクターデータ</param>
    /// <param name="index">選択したボタンのインデックス</param>
    public void SelectCharacter(SelectCharacterData character, int index)
    {
        if (character == null)
        {
            Debug.LogError("選択するキャラクターデータがnullです。", this);
            return;
        }

        SelectedCharacter = character;

        _characterImage.gameObject.SetActive(true);
        _characterImage.sprite = character.CharacterSprite;
        _characterName.text = character.CharacterName;

        _skillName1.text = character.SkillName1;
        _skillDescription1.text = character.SkillDescription1;

        _skillName2.text = character.SkillName2;
        _skillDescription2.text = character.SkillDescription2;
    }

    /// <summary>
    ///     指定したインデックスのキャラクターボタンを選択する
    /// </summary>
    /// <param name="index">選択するボタンのインデックス</param>
    public void SelectButton(int index)
    {
        _selectionController.Select(index);
    }

    /// <summary>
    ///     選択したキャラクターでゲームを開始する
    /// </summary>
    public void StartGame()
    {
        if (SelectedCharacter == null)
        {
            Debug.LogError("キャラクターが選択されていません。", this);
            return;
        }

        CharacterDefinition characterDefinition = SelectedCharacter.CharacterDefinition;

        if (characterDefinition == null)
        {
            Debug.LogError("選択したキャラクターデータにCharacterDefinitionが設定されていません。", this);

            return;
        }

        if (string.IsNullOrEmpty(_inGameSceneName))
        {
            Debug.LogError("InGameSceneNameが設定されていません。", this);
            return;
        }

        if (!GameSession.SetSelectedCharacter(characterDefinition))
        {
            return;
        }

        SceneLoader.LoadScene(_inGameSceneName);
    }

    private void Awake()
    {
        CheckNull();
    }

    private void Start()
    {
        CreateCharacterButtons();
        HideCharacterPreview();
    }

    /// <summary>
    ///     キャラクター選択用のボタンを生成する
    /// </summary>
    private void CreateCharacterButtons()
    {
        if (_selectCharacterDatas == null ||
            _selectCharacterDatas.Length == 0)
        {
            return;
        }

        CharacterSelectButton[] buttons =
            new CharacterSelectButton[_selectCharacterDatas.Length];

        for (int index = 0; index < _selectCharacterDatas.Length; index++)
        {
            SelectCharacterData characterData = _selectCharacterDatas[index];

            if (characterData == null)
            {
                Debug.LogError($"SelectCharacterDatasの{index}番目が設定されていません。", this);

                continue;
            }

            CharacterSelectButton button = Instantiate(
                _characterSelectButtonPrefab,
                _buttonParent);

            button.Initialize(characterData, this, index);
            buttons[index] = button;
        }

        _selectionController.Initialize(buttons);
    }

    /// <summary>
    ///     キャラクターのプレビューを非表示にする
    /// </summary>
    private void HideCharacterPreview()
    {
        if (_characterImage != null)
        {
            _characterImage.gameObject.SetActive(false);
        }

        if (_characterName != null)
        {
            _characterName.text = string.Empty;
        }

        if (_skillName1 != null)
        {
            _skillName1.text = string.Empty;
        }

        if (_skillDescription1 != null)
        {
            _skillDescription1.text = string.Empty;
        }

        if (_skillName2 != null)
        {
            _skillName2.text = string.Empty;
        }

        if (_skillDescription2 != null)
        {
            _skillDescription2.text = string.Empty;
        }
    }

    /// <summary>
    ///     必要な設定が存在するか確認する
    /// </summary>
    private void CheckNull()
    {
        if (_selectCharacterDatas == null ||
            _selectCharacterDatas.Length == 0)
        {
            Debug.LogError("SelectCharacterDatasが設定されていません。", this);
        }

        if (_characterSelectButtonPrefab == null)
        {
            Debug.LogError("CharacterSelectButtonPrefabが設定されていません。", this);
        }

        if (_buttonParent == null)
        {
            Debug.LogError("ButtonParentが設定されていません。", this);
        }

        if (_characterImage == null)
        {
            Debug.LogError("CharacterImageが設定されていません。", this);
        }

        if (_characterName == null)
        {
            Debug.LogError("CharacterNameが設定されていません。", this);
        }

        if (_skillName1 == null)
        {
            Debug.LogError("SkillName1が設定されていません。", this);
        }

        if (_skillDescription1 == null)
        {
            Debug.LogError("SkillDescription1が設定されていません。", this);
        }

        if (_skillName2 == null)
        {
            Debug.LogError("SkillName2が設定されていません。", this);
        }

        if (_skillDescription2 == null)
        {
            Debug.LogError("SkillDescription2が設定されていません。", this);
        }

        if (_selectionController == null)
        {
            Debug.LogError("SelectionControllerが設定されていません。", this);
        }

        if (string.IsNullOrEmpty(_inGameSceneName))
        {
            Debug.LogError("InGameSceneNameが設定されていません。", this);
        }
    }
}