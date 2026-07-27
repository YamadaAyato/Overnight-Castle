using System;
using UnityEngine;

/// <summary>
///     高さに応じた抽選テーブルを保持するクラス
/// </summary>
[Serializable]
public class HeightSpawnTable
{
    /// <summary> この抽選が適用される高さの閾値 </summary>
    public float HeightThreshold => _heightThreshold;

    /// <summary> この抽選で使用される重み付きピースのリスト </summary>
    public WeightedPiece[] WeightedPieces => _weightedPieces;

    [SerializeField, Tooltip("高さの閾値")] private float _heightThreshold;
    [SerializeField, Tooltip("重み付きピースのリスト")] private WeightedPiece[] _weightedPieces;
}
