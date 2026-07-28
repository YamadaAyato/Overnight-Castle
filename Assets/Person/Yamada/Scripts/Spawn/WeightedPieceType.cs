using System;
using UnityEngine;

/// <summary>
///     ピースの種類と抽選の重みを保持するクラス
/// </summary>
[Serializable]
public class WeightedPieceType
{
    /// <summary> ピースの種類 </summary>
    public PieceType PieceType => _pieceType;

    /// <summary> 抽選の重み </summary>
    public int SpawnWeight => _spawnWeight;

    [SerializeField] private PieceType _pieceType;
    [SerializeField, Min(0)] private int _spawnWeight = 1;
}