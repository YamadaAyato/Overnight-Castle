using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterImageSpawner : MonoBehaviour
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

    public void Spawn(CharacterType selectedCharacter)
    {
        CharacterPrefab data = _characterPrefabs.Find(
            x => x.characterType == selectedCharacter);

        if (data == null || data.prefab == null)
        {
            Debug.LogWarning($"{selectedCharacter} 用のプレハブが未設定です。", this);
            return;
        }

        _currentInstance = Instantiate(data.prefab, _spawnParent);

        // 初期状態の画像、アニメーションを反映
        _currentInstance.PlayImageType();
    }

    [SerializeField] private CharacterImageSpawner _spawner;

    public void OnClickMage()
    {
        _spawner.Spawn(CharacterType.Lord);
    }
}