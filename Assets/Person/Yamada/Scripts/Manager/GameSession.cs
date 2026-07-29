using UnityEngine;

/// <summary>
///     ゲームセッションの状態を管理する静的クラス
/// </summary>
public static class GameSession
{
    /// <summary> 選択されたキャラクターの定義 </summary>
    public static CharacterDefinition SelectedCharacter { get; private set; }

    /// <summary> 選択されたキャラクターが存在するかどうかを示すプロパティ </summary>
    public static bool HasSelectedCharacter => SelectedCharacter != null;

    /// <summary>
    ///     選択されたキャラクターを設定するメソッド
    /// </summary>
    /// <param name="character">設定するキャラクターの定義</param>
    /// <returns>設定が成功したかどうか</returns>
    public static bool SetSelectedCharacter(CharacterDefinition character)
    {
        if (character == null)
        {
            Debug.LogError("Selected character cannot be null.");
            return false;
        }

        SelectedCharacter = character;
        return true;
    }

    /// <summary>
    ///     選択されたキャラクターを取得するメソッド
    /// </summary>
    /// <param name="character">取得するキャラクターの定義</param>
    /// <returns>取得が成功したかどうか</returns>
    public static bool TryGetSelectedCharacter(out CharacterDefinition character)
    {
        character = SelectedCharacter;
        return character != null;
    }

    /// <summary>
    ///     ゲームセッションをクリアするメソッド
    /// </summary>
    public static void Clear()
    {
        SelectedCharacter = null;
    }
}
