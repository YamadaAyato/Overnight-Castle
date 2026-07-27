using System;
using UnityEngine;

/// <summary>
///     ピースの物理設定を保持するクラス
/// </summary>
[Serializable]
public class PiecePhysicsSettings
{
    /// <summary> Rigidbody2dの質量 </summary>
    public float Mass => _mass;

    /// <summary> Rigidbody2dの重力スケール </summary>
    public float GravityScale => _gravityScale;

    /// <summary> Rigidbody2dの線形減衰 </summary>
    public float LinearDamping => _linearDamping;

    /// <summary> Rigidbody2dの角度減衰 </summary>
    public float AngularDamping => _angularDamping;

    /// <summary>
    ///     Rigidbody2Dに設定を適用する
    /// </summary>
    /// <param name="rigidbody2D">設定を適用するRigidbody2D</param>
    public void ApplyToRigidbody2D(Rigidbody2D rigidbody2D)
    {
        rigidbody2D.mass = _mass;
        rigidbody2D.gravityScale = _gravityScale;
        rigidbody2D.linearDamping = _linearDamping;
        rigidbody2D.angularDamping = _angularDamping;
    }

    [SerializeField, Min(0.01f), Tooltip("質量")] private float _mass = 1f;
    [SerializeField, Min(0.01f), Tooltip("重力スケール")] private float _gravityScale = 1f;
    [SerializeField, Min(0f), Tooltip("線形減衰")] private float _linearDamping = 0f;
    [SerializeField, Min(0f), Tooltip("角度減衰")] private float _angularDamping = 0f;
}
