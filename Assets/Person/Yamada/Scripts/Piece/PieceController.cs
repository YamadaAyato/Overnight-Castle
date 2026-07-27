using System;
using UnityEngine;

/// <summary>
///     プレイヤーの入力に応じてピースを操作するクラス
/// </summary>
public class PieceController : MonoBehaviour
{
    /// <summary> ピースが落下したときに発火するイベント </summary>
    public event Action<FallingPiece> OnPieceDropped;

    /// <summary>
    ///     ピースの移動入力を設定する
    /// </summary>
    /// <param name="input">移動入力の値</param>
    public void SetMoveInput(float input)
    {
        _moveInput = Mathf.Clamp(input, -1f, 1f);
    }

    /// <summary>
    ///     ピースの回転入力を設定する
    /// </summary>
    /// <param name="input">回転入力の値</param>
    public void SetRotateInput(float input)
    {
        _rotateInput = Mathf.Clamp(input, -1f, 1f);
    }

    /// <summary>
    ///     現在操作中のピースを設定する
    /// </summary>
    /// <param name="piece">現在操作中のピース</param>
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
    ///     現在操作中のピースを落下させる
    /// </summary>
    public void DropCurrentPiece()
    {
        if (!_canControl || _currentPiece == null)
        {
            return;
        }

        // 現在操作中のピースを落下させる
        FallingPiece droppedPiece = _currentPiece;

        _moveInput = 0f;
        _rotateInput = 0f;
        _currentPiece = null;

        droppedPiece.Drop();
        OnPieceDropped?.Invoke(droppedPiece);
    }

    /// <summary>
    ///     プレイヤーによるピースの操作を開始する
    /// </summary>
    public void StartControl()
    {
        _canControl = true;
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
        _horizontalLimit = Mathf.Max(0f, width / 2f);
    }

    [Header("操作設定")]
    [SerializeField, Min(0f), Tooltip("ピースの移動速度")] private float _moveSpeed = 5f;
    [SerializeField, Min(0f), Tooltip("ピースの回転速度")] private float _rotateSpeed = 120f;

    private FallingPiece _currentPiece;
    private float _moveInput;
    private float _rotateInput;
    private float _horizontalLimit;

    private bool _canControl = true;

    private void FixedUpdate()
    {
        // 現在操作中のピースが存在し、操作可能な場合のみ移動と回転を行う
        if (_canControl && _currentPiece != null)
        {
            Move();
            Rotate();
        }
    }

    /// <summary>
    ///     ピースを移動する
    /// </summary>
    private void Move()
    {
        Vector3 position = _currentPiece.transform.position;

        position.x += _moveInput * _moveSpeed * Time.fixedDeltaTime;
        position.x = Mathf.Clamp(position.x, -_horizontalLimit, _horizontalLimit);
        _currentPiece.transform.position = position;
    }

    /// <summary>
    ///     ピースを回転する
    /// </summary>
    private void Rotate()
    {
        Vector3 rotation = _currentPiece.transform.eulerAngles;

        rotation.z += _rotateInput * _rotateSpeed * Time.fixedDeltaTime;
        _currentPiece.transform.eulerAngles = rotation;
    }
}
