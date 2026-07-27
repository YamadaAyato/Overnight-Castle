using UnityEngine;

/// <summary>
///     落下するピースの挙動を制御するクラス
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingPiece : MonoBehaviour
{
    /// <summary> ピースの種類 </summary>
    public PieceType PieceType => _pieceType;
    
    /// <summary> ピースのスコア </summary>
    public int Score => _score;

    /// <summary> ピースが落下したかどうか </summary>
    public bool HasDropped => _hasDropped;

    /// <summary> ピースが着地したかどうか </summary>
    public bool HasLanded => _hasLanded;

    /// <summary> ピースの最高位置Y座標 </summary>
    public float HighestPositionY => _collider2D.bounds.max.y;


    /// <summary>
    ///     ピースの初期化処理
    /// </summary>
    /// <param name="grobalPiecePhysicsSettings">全体物理設定</param>
    /// <param name="deletePositionY">削除するY座標の閾値</param>
    public void Initialize(GlobalPiecePhysicsSettings grobalPiecePhysicsSettings, float deletePositionY)
    {
        PiecePhysicsSettings settings =
            grobalPiecePhysicsSettings != null
            ? grobalPiecePhysicsSettings.PiecePhysicsSettings
            : _piecePhysicsSettings;

        _deletePositionY = deletePositionY;
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

    [Header("ピースの種類")]
    [SerializeField] private PieceType _pieceType = PieceType.Normal;
    [SerializeField] private PiecePhysicsSettings _piecePhysicsSettings = new();
    [SerializeField] private int _score;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private float _appliedGravityScale;
    private float _deletePositionY = float.NegativeInfinity;
    private bool _hasDropped;
    private bool _hasLanded;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _collider2D = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (transform.position.y < _deletePositionY)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_hasDropped)
            return;

        _hasLanded = true;
    }

    /// <summary>
    ///     ピースの物理設定を適用する
    /// </summary>
    /// <param name="piecePhysicsSettings">ピースの物理設定</param>
    private void ApplyPhysicsSettings(PiecePhysicsSettings piecePhysicsSettings)
    {
        if (piecePhysicsSettings == null ||
            _rigidbody2D == null)
        {
            return;
        }

        piecePhysicsSettings.ApplyToRigidbody2D(_rigidbody2D);
        _appliedGravityScale = piecePhysicsSettings.GravityScale;
    }
}
