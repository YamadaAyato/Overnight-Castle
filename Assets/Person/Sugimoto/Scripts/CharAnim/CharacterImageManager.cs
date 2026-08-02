using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterImageManager : MonoBehaviour
{
    [Serializable]
    private class CharacterPrefab
    {
        public CharacterType characterType;
        public ChangeImageAnimBase prefab;
    }

    [SerializeField] private Transform _spawnParent;
    [SerializeField] private List<CharacterPrefab> _characterPrefabs;

    private ChangeImageAnimBase _currentInstance;

    /// <summary>
    /// 指定されたキャラにあったプレハブを生成する
    /// </summary>
    /// <param name="selectedCharacter"></param>
    public void Spawn(CharacterDefinition selectedCharacter)
    {
        CharacterPrefab data = _characterPrefabs.Find(
            x => x.characterType == selectedCharacter.CharacterType);

        if (data == null || data.prefab == null)
        {
            Debug.LogWarning($"{selectedCharacter} 用のプレハブが未設定です。", this);
            return;
        }

        _currentInstance = Instantiate(data.prefab, _spawnParent);

        // 初期状態の画像、アニメーションを反映
        _currentInstance.PlayImageType();
    }

    /// <summary>
    /// 指定の遷移のアニメーションを再生する
    /// </summary>
    /// <param name="imageType"></param>
    public void playAnim(ImageType imageType) 
    {
        _currentInstance.SetImageType(imageType);
    }
}