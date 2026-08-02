using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     落下するピースの挙動を制御するクラス
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class FallingPiece : MonoBehaviour
{
    /// <summary> ピースの定義 </summary>
    public PieceDefinition PieceDefinition => _pieceDefinition;

    /// <summary> ピースの種類 </summary>
    public PieceType PieceType => _pieceDefinition != null 
        ? _pieceDefinition.PieceType 
        : PieceType.Normal;

    /// <summary> ピースの城の部位の種類 </summary>
    public CastlePartType PartType => _pieceDefinition != null 
        ? _pieceDefinition.PartType 
        : CastlePartType.Foundation;

    /// <summary> ピースのスコア </summary>
    public int Score => _pieceDefinition != null 
        ? _pieceDefinition.Score 
        : 0;

    /// <summary> ピースが落下したかどうか </summary>
    public bool HasDropped => _hasDropped;

    /// <summary> ピースが着地したかどうか </summary>
    public bool HasLanded => _hasLanded;

    /// <summary> ピースの最高位置Y座標 </summary>
    public float HighestPositionY => _collider2D.bounds.max.y;

    /// <summary> ピースが着地したときに発火するイベント </summary>
    public event Action<FallingPiece> OnLanded;


    /// <summary> ピースが破壊されたときに発火するイベント </summary>
    public event Action<FallingPiece> OnDestroyed;


    /// <summary>
    ///     ピースの初期化処理
    /// </summary>
    /// <param name="pieceDefinition">ピースの定義</param>
    /// <param name="globalPiecePhysicsSettings">全体物理設定</param>
    /// <param name="deletePositionY">削除するY座標の閾値</param>
    public void Initialize(
        PieceDefinition pieceDefinition,
        GlobalPiecePhysicsSettings globalPiecePhysicsSettings,
        float deletePositionY)
    {
        if (pieceDefinition == null)
        {
            Debug.LogError("PieceDefinitionが設定されていません");
            return;
        }

        _pieceDefinition = pieceDefinition;
        _deletePositionY = deletePositionY;

        ApplySprite(pieceDefinition.Sprite);

        PiecePhysicsSettings settings =
            globalPiecePhysicsSettings != null
            ? globalPiecePhysicsSettings.PiecePhysicsSettings
            : pieceDefinition.PhysicsSettings;

        ApplyPhysicsSettings(settings);
        SetControllable();
    }

    /// <summary>
    ///     ピースを制御可能な状態にする
    /// </summary>
    public void SetControllable()
    {
        _hasDropped = false;
        _hasLanded = false;
        _hasNotifiedDeletion = false;

        _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody2D.gravityScale = 0f;
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
    }

    /// <summary>
    ///     ピースを落下させる
    /// </summary>
    public void Drop()
    {
        if (_hasDropped) return;

        _hasDropped = true;

        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = _appliedGravityScale;
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
    }

    /// <summary>
    ///     ピースを固定する
    /// </summary>
    public void Fix()
    {
        _rigidbody2D.linearVelocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0f;
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
    }

    /// <summary>
    ///     ピースが停止しているかどうかを判定する
    /// </summary>
    /// <param name="linearVelocityThreshold">線形速度の閾値</param>
    /// <param name="angularVelocityThreshold">角速度の閾値</param>
    /// <returns>ピースが停止している場合はtrue、それ以外の場合はfalse</returns>
    public bool IsStopped(float linearVelocityThreshold, float angularVelocityThreshold)
    {
        if(!_hasDropped ||
            _rigidbody2D.bodyType != RigidbodyType2D.Dynamic)
        {
            return false;
        }

        // 閾値を正の値に変換する
        float linerThreshold = Mathf.Max(0f, linearVelocityThreshold);
        float angularThreshold = Mathf.Max(0f, angularVelocityThreshold);

        // 速度が閾値以下かどうかを判定する
        return _rigidbody2D.linearVelocity.sqrMagnitude <= linerThreshold * linerThreshold &&
               Mathf.Abs(_rigidbody2D.angularVelocity) <= angularThreshold;
    }

    private readonly List<Vector2> _originalColliderPoints = new List<Vector2>();

    private PieceDefinition _pieceDefinition;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private PolygonCollider2D _collider2D;

    private float _appliedGravityScale;
    private float _deletePositionY = float.NegativeInfinity;
    private bool _hasDropped;
    private bool _hasLanded;
    private bool _hasNotifiedDeletion;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<PolygonCollider2D>();
    }

    private void Update()
    {
        if (_hasNotifiedDeletion ||
            transform.position.y >= _deletePositionY)
        {
            return;
        }

        _hasNotifiedDeletion = true;
        OnDestroyed?.Invoke(this);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_hasDropped || _hasLanded)
            return;

        _hasLanded = true;
        OnLanded?.Invoke(this);
    }

    /// <summary>
    ///     ピースのスプライトを適用する
    /// </summary>
    /// <param name="sprite">適用するスプライト</param>
    private void ApplySprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;

        if (sprite == null)
        {
            Debug.LogError("Spriteが設定されていません", this);

            _collider2D.enabled = false;
            return;
        }

        _collider2D.enabled = true;
        ApplyPhysicsShape(sprite);
    }

    /// <summary>
    ///     ピースのスプライトに応じた物理形状を適用する
    /// </summary>
    /// <param name="sprite">適用するスプライト</param>
    private void ApplyPhysicsShape(Sprite sprite)
    {
        int pathCount = sprite.GetPhysicsShapeCount();

        if (pathCount <= 0)
        {
            ApplyRectangleCollider(sprite);
            return;
        }

        _collider2D.pathCount = pathCount;

        for (int index = 0; index < pathCount; index++)
        {
            _originalColliderPoints.Clear();

            sprite.GetPhysicsShape(index, _originalColliderPoints);
            _collider2D.SetPath(index, _originalColliderPoints);
        }
    }

    /// <summary>
    ///     ピースのスプライトに応じた矩形コライダーを適用する
    /// </summary>
    /// <param name="sprite">適用するスプライト</param>
    private void ApplyRectangleCollider(Sprite sprite)
    {
        Bounds bounds = sprite.bounds;

        Vector2[] points =
        {
            new(bounds.min.x, bounds.min.y),
            new(bounds.min.x, bounds.max.y),
            new(bounds.max.x, bounds.max.y),
            new(bounds.max.x, bounds.min.y)
        };

        _collider2D.pathCount = 1;
        _collider2D.SetPath(0, points);
    }

    /// <summary>
    ///     ピースの物理設定を適用する
    /// </summary>
    /// <param name="piecePhysicsSettings">ピースの物理設定</param>
    private void ApplyPhysicsSettings(PiecePhysicsSettings piecePhysicsSettings)
    {
        if (piecePhysicsSettings == null)
        {
            Debug.LogError("PiecePhysicsSettingsが設定されていません", this);
            return;
        }

        piecePhysicsSettings.ApplyToRigidbody2D(_rigidbody2D);
        _appliedGravityScale = piecePhysicsSettings.GravityScale;
    }
}
