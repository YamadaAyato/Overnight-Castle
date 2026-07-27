using System;
using UnityEngine;

/// <summary>
///     重み付き抽選のためのピース情報を保持するクラス
/// </summary>
[Serializable]
public class WeightedPiece
{
    public FallingPiece PiecePrefab => _piecePrefab;

    public int SpawnWeight => _spawnWeight;

    [SerializeField] private FallingPiece _piecePrefab;
    [SerializeField, Min(0f)] private int _spawnWeight;
}
