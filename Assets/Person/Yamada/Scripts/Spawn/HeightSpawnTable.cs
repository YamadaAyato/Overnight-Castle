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

    /// <summary> この高さで使用されるキャッスルパートの種類 </summary>
    public CastlePartType CastlePartType => _castlePartType;

    /// <summary> この抽選で使用される重み付きピースのリスト </summary>
    public WeightedPieceType[] WeightedPieceTypes => _weightedPieceTypes;

    [SerializeField, Tooltip("高さの閾値")] private float _heightThreshold;
    [SerializeField, Tooltip("この高さで使用されるキャッスルパートの種類")] private CastlePartType _castlePartType;
    [SerializeField, Tooltip("重み付きピースのリスト")] private WeightedPieceType[] _weightedPieceTypes;
}
