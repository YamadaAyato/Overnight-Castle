using System;
using UnityEngine;

/// <summary>
///     重み付き抽選のためのピース情報を保持するクラス
/// </summary>
[Serializable]
public class WeightedPiece
{
    /// <summary> 抽選対象のピースPrefab </summary>
    public FallingPiece PiecePrefab => _piecePrefab;

    /// <summary> 抽選の重み </summary>
    public int SpawnWeight => _spawnWeight;

    [SerializeField] private FallingPiece _piecePrefab;
    [SerializeField, Min(0f)] private int _spawnWeight;
}
