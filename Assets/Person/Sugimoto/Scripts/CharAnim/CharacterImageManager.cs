using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterImageManager : MonoBehaviour
{
    [Serializable]
    private class CharacterPrefab
    {
        public CharacterType characterType => _characterType;
        public ChangeImageAnimBase prefab => _prefab;

        [SerializeField] private CharacterType _characterType;
        [SerializeField] private ChangeImageAnimBase _prefab;
    }

    [SerializeField] private Transform _spawnParent;
    [SerializeField] private List<CharacterPrefab> _characterPrefabs;
    [SerializeField] private float _reactionDuration;

    private ChangeImageAnimBase _currentInstance;
    private Tween _returnTween;
    private CharacterType _characterType;

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
        _currentInstance.SetImageType(ImageType.Normal);
    }

    /// <summary>
    ///     指定した画像状態へ変更する
    /// </summary>
    /// <param name="imageType">変更する画像状態</param>
    public void PlayAnimation(ImageType imageType)
    {
        if (_currentInstance == null)
        {
            return;
        }

        _returnTween?.Kill();
        _currentInstance.SetImageType(imageType);
    }

    /// <summary>
    ///     指定した画像状態を一時的に再生する
    /// </summary>
    /// <param name="imageType">変更する画像状態</param>
    public void PlayReaction(ImageType imageType)
    {
        if (_currentInstance == null)
        {
            return;
        }

        _returnTween?.Kill();
        _currentInstance.SetImageType(imageType);

        _returnTween = DOVirtual
            .DelayedCall(
                _reactionDuration,
                () =>
                {
                    if (_currentInstance != null)
                    {
                        _currentInstance.SetImageType(ImageType.Normal);
                    }
                })
            .SetLink(gameObject);
    }

    private void OnDestroy()
    {
        _returnTween?.Kill();
    }
}