using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     プレイヤーの入力に応じてピースを操作するクラス
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PieceController : MonoBehaviour
{
    private const string MOVE_ACTION_NAME = "Move";
    private const string ROTATE_ACTION_NAME = "Rotate";
    private const string DROP_ACTION_NAME = "Drop";

    /// <summary> ピースが落下したときに発火するイベント </summary>
    public event Action<FallingPiece> OnPieceDropped;

    /// <summary>
    ///     現在操作中のピースを設定する
    /// </summary>
    /// <param name="piece"></param>
    public void SetCurrentPiece(FallingPiece piece)
    {
        if (piece == null)
        {
            return;
        }

        _currentPiece = piece;

        _moveInput = 0f;
        _rotateInput = 0f;
    }

    /// <summary>
    ///     プレイヤーによるピースの操作を停止する
    /// </summary>
    public void StopControl()
    {
        _canControl = false;

        _moveInput = 0f;
        _rotateInput = 0f;

        if (_currentPiece != null)
        {
            _currentPiece.Fix();
            _currentPiece = null;
        }
    }

    /// <summary>
    ///     ステージ幅から水平移動の制限値を設定する
    /// </summary>
    /// <param name="width">ステージの幅</param>
    public void SetStageWidth(float width)
    {
        _horizontalLimit = width / 2f;
    }

    [Header("操作設定")]
    [SerializeField, Min(0f), Tooltip("ピースの移動速度")] private float _moveSpeed = 5f;
    [SerializeField, Min(0f), Tooltip("ピースの回転速度")] private float _rotateSpeed = 120f;

    private PlayerInput _playerInput;
    private InputAction _moveAction;
    private InputAction _rotateAction;
    private InputAction _dropAction;

    private FallingPiece _currentPiece;
    private float _moveInput;
    private float _rotateInput;
    private float _horizontalLimit;

    private bool _canControl = true;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        _moveAction = _playerInput.actions.FindAction(MOVE_ACTION_NAME, true);
        _rotateAction = _playerInput.actions.FindAction(ROTATE_ACTION_NAME, true);
        _dropAction = _playerInput.actions.FindAction(DROP_ACTION_NAME, true);
    }

    private void FixedUpdate()
    {
        // 現在操作中のピースが存在し、操作可能な場合のみ移動と回転を行う
        if (_canControl && _currentPiece != null)
        {
            Move(_moveInput);
            Rotate(_rotateInput);
        }
    }

    private void OnEnable()
    {
        _moveAction.performed += OnMovePerfomed;
        _moveAction.canceled += OnMoveCanceled;

        _rotateAction.performed += OnRotatePerformed;
        _rotateAction.canceled += OnRotateCanceled;

        _dropAction.performed += OnDropPerformed;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerfomed;
        _moveAction.canceled -= OnMoveCanceled;

        _rotateAction.performed -= OnRotatePerformed;
        _rotateAction.canceled -= OnRotateCanceled;

        _dropAction.performed -= OnDropPerformed;

        _moveInput = 0f;
        _rotateInput = 0f;
    }

    /// <summary>
    ///     ピースを移動する
    /// </summary>
    /// <param name="input">移動入力値</param>
    private void Move(float input)
    {
        Vector3 position = _currentPiece.transform.position;

        position.x += input * _moveSpeed * Time.fixedDeltaTime;
        position.x = Mathf.Clamp(position.x, -_horizontalLimit, _horizontalLimit);
        _currentPiece.transform.position = position;
    }

    /// <summary>
    ///     ピースを回転する
    /// </summary>
    /// <param name="input">回転入力値</param>
    private void Rotate(float input)
    {
        Vector3 rotation = _currentPiece.transform.eulerAngles;

        rotation.z += input * _rotateSpeed * Time.fixedDeltaTime;
        _currentPiece.transform.eulerAngles = rotation;
    }

    /// <summary>
    ///     移動入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnMovePerfomed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<float>();
    }

    /// <summary>
    ///     移動入力がキャンセルされたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _moveInput = 0f;
    }

    /// <summary>
    ///     回転入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        _rotateInput = context.ReadValue<float>();
    }

    /// <summary>
    ///     回転入力がキャンセルされたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        _rotateInput = 0f;
    }

    /// <summary>
    ///     ピースを落下させる入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnDropPerformed(InputAction.CallbackContext context)
    {
        if (!_canControl || _currentPiece == null)
        {
            return;
        }

        FallingPiece droppedPiece = _currentPiece;

        _moveInput = 0f;
        _rotateInput = 0f;
        _currentPiece = null;

        droppedPiece.Drop();

        OnPieceDropped?.Invoke(droppedPiece);
    }
}
