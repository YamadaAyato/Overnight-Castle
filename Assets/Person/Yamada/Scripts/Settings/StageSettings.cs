using UnityEngine;

/// <summary>
///     ステージの設定を保持するSO
/// </summary>
[CreateAssetMenu(
    fileName = "StageSettings",
    menuName = "Overnight Castle/StageSettings")]
public class StageSettings : ScriptableObject
{
    /// <summary> ステージの制限時間 </summary>
    public float StageTimeLimit => _stageTimeLimit;

    /// <summary> 次のピースを生成するまでの時間 </summary>
    public float NextSpawnDelay => _nextSpawnDelay;

    /// <summary> 城の高さ計算で基準にするY座標 </summary>
    public float GroundPosY => _groundPosY;

    /// <summary> 削除するY座標の閾値 </summary>
    public float DeletePositionY => _deletePositionY;

    /// <summary> ステージの幅 </summary>
    public float StageWidth => _stageWidth;

    /// <summary> 高さに応じた抽選テーブルのリスト </summary>
    public HeightSpawnTable[] HeightSpawnTables => _heightSpawnTables;

    /// <summary> 全体物理設定を使用するか </summary>
    public bool UseGlobalPiecePhysicsSettings => _useGrobalPiecePhysicsSettings;

    /// <summary> 全体物理設定 </summary>
    public GlobalPiecePhysicsSettings GlobalPiecePhysicsSettings => _grobalPiecePhysicsSettings;

    [Header("時間設定")]
    [SerializeField, Tooltip("ステージの制限時間")] private float _stageTimeLimit = 60f;
    [SerializeField, Min(0f), Tooltip("次のピースを生成するまでの時間")] private float _nextSpawnDelay = 0.8f;

    [Header("ステージ設定")]
    [SerializeField, Tooltip("城の高さ計算で基準にするY座標")] private float _groundPosY = 10f;
    [SerializeField, Tooltip("削除するY座標の閾値")] private float _deletePositionY = 10f;
    [SerializeField, Tooltip("ステージの幅")] private float _stageWidth = 10f;
    [SerializeField, Tooltip("ステージの高さ")] private HeightSpawnTable[] _heightSpawnTables;

    [Header("全体物理設定")]
    [SerializeField, Tooltip("全体物理設定を使用するか")] private bool _useGrobalPiecePhysicsSettings = false;
    [SerializeField, Tooltip("全体物理設定")] private GlobalPiecePhysicsSettings _grobalPiecePhysicsSettings = null;
}
