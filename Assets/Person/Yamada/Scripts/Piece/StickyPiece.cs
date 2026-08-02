using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     粘着ピースの挙動を制御するクラス
/// </summary>
public class StickyPiece : MonoBehaviour
{
    [SerializeField, Tooltip("土台として判定するレイヤー")]
    private LayerMask _groundLayer;

    private readonly HashSet<FallingPiece> _connectedPieces = new HashSet<FallingPiece>();
    private readonly HashSet<Rigidbody2D> _connectedRigidbodies = new HashSet<Rigidbody2D>();
    private readonly List<FixedJoint2D> _joints = new List<FixedJoint2D>();

    private FallingPiece _fallingPiece;
    private Rigidbody2D _rigidbody2D;

    private bool _isFixedToGround;

    private void Awake()
    {
        _fallingPiece = GetComponent<FallingPiece>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_fallingPiece.HasDropped)
        {
            return;
        }

        if (IsGround(collision.gameObject))
        {
            FixAllPieces();
            return;
        }

        FallingPiece targetPiece = collision.gameObject.GetComponentInParent<FallingPiece>();

        if (!CanConnect(targetPiece))
        {
            return;
        }

        if (_isFixedToGround)
        {
            targetPiece.Fix();
            _connectedPieces.Add(targetPiece);
            return;
        }

        ConnectPiece(targetPiece);
    }

    /// <summary>
    ///     指定したピースを接続できるかどうかを判定する
    /// </summary>
    /// <param name="targetPiece">接続対象のピース</param>
    /// <returns>接続できる場合はtrue、それ以外はfalse</returns>
    private bool CanConnect(FallingPiece targetPiece)
    {
        if (targetPiece == null ||
            targetPiece == _fallingPiece ||
            !targetPiece.HasDropped)
        {
            return false;
        }

        Rigidbody2D targetRigidbody = targetPiece.GetComponent<Rigidbody2D>();

        if (targetRigidbody == null ||
            _connectedRigidbodies.Contains(targetRigidbody))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     指定したピースを粘着ピースへ接続する
    /// </summary>
    /// <param name="targetPiece">接続するピース</param>
    private void ConnectPiece(FallingPiece targetPiece)
    {
        Rigidbody2D targetRigidbody = targetPiece.GetComponent<Rigidbody2D>();

        FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
        joint.connectedBody = targetRigidbody;
        joint.autoConfigureConnectedAnchor = true;
        joint.enableCollision = false;

        _connectedPieces.Add(targetPiece);
        _connectedRigidbodies.Add(targetRigidbody);
        _joints.Add(joint);
    }

    /// <summary>
    ///     粘着ピースと接続済みのピースを固定する
    /// </summary>
    private void FixAllPieces()
    {
        if (_isFixedToGround)
        {
            return;
        }

        _isFixedToGround = true;

        _fallingPiece.Fix();

        foreach (FallingPiece connectedPiece in _connectedPieces)
        {
            if (connectedPiece != null)
            {
                connectedPiece.Fix();
            }
        }

        foreach (FixedJoint2D joint in _joints)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        _joints.Clear();
        _connectedRigidbodies.Clear();
    }

    /// <summary>
    ///     接触したオブジェクトが土台かどうかを判定する
    /// </summary>
    /// <param name="target">接触したオブジェクト</param>
    /// <returns>土台の場合はtrue、それ以外はfalse</returns>
    private bool IsGround(GameObject target)
    {
        return (_groundLayer.value & 1 << target.layer) != 0;
    }
}
