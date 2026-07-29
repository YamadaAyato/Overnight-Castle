using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
///     プレイヤーの入力を読み取り、PieceControllerに伝えるクラス
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private const string MOVE_ACTION_NAME = "Move";
    private const string ROTATE_ACTION_NAME = "Rotate";
    private const string DROP_ACTION_NAME = "Drop";
    private const string SKILL1_ACTION_NAME = "Skill1";
    private const string SKILL2_ACTION_NAME = "Skill2";

    [SerializeField] private PieceController _controller;
    [SerializeField] private CharacterSkillController _skillController;

    private InputAction _moveAction;
    private InputAction _rotateAction;
    private InputAction _dropAction;
    private InputAction _skill1Action;
    private InputAction _skill2Action;

    private void Reset()
    {
        _controller = GetComponent<PieceController>();
        _skillController = GetComponent<CharacterSkillController>();
    }

    private void Awake()
    {
        if (_controller == null)
        {
            _controller = GetComponent<PieceController>();
        }

        if (_skillController == null)
        {
            _skillController = GetComponent<CharacterSkillController>();
        }

        PlayerInput playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions.FindAction(MOVE_ACTION_NAME, true);
        _rotateAction = playerInput.actions.FindAction(ROTATE_ACTION_NAME, true);
        _dropAction = playerInput.actions.FindAction(DROP_ACTION_NAME, true);
        _skill1Action = playerInput.actions.FindAction(SKILL1_ACTION_NAME, true);
        _skill2Action = playerInput.actions.FindAction(SKILL2_ACTION_NAME, true);
    }

    private void OnEnable()
    {
        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;

        _rotateAction.performed += OnRotatePerformed;
        _rotateAction.canceled += OnRotateCanceled;

        _dropAction.performed += OnDropPerformed;

        _skill1Action.performed += OnSkill1Performed;
        _skill2Action.performed += OnSkill2Performed;
    }

    private void OnDisable()
    {
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;

        _rotateAction.performed -= OnRotatePerformed;
        _rotateAction.canceled -= OnRotateCanceled;

        _dropAction.performed -= OnDropPerformed;

        _skill1Action.performed -= OnSkill1Performed;
        _skill2Action.performed -= OnSkill2Performed;

        if (_controller != null)
        {
            _controller.SetMoveInput(0f);
            _controller.SetRotateInput(0f);
        }
    }

    /// <summary>
    ///     移動入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _controller.SetMoveInput(context.ReadValue<float>());
    }

    /// <summary>
    ///     移動入力がキャンセルされたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        _controller.SetMoveInput(0f);
    }

    /// <summary>
    ///     回転入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        _controller.SetRotateInput(context.ReadValue<float>());
    }

    /// <summary>
    ///     回転入力がキャンセルされたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        _controller.SetRotateInput(0f);
    }

    /// <summary>
    ///     ピースを落下させる入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnDropPerformed(InputAction.CallbackContext context)
    {
        _controller.DropCurrentPiece();
    }

    /// <summary>
    ///     スキル1を使用する入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnSkill1Performed(InputAction.CallbackContext context)
    {
        if (_skillController == null) return;

        _skillController.UseFirstSkill();
    }

    /// <summary>
    ///     スキル2を使用する入力が行われたときに呼ばれるコールバック
    /// </summary>
    /// <param name="context">入力コンテキスト</param>
    private void OnSkill2Performed(InputAction.CallbackContext context)
    {
        if (_skillController == null) return;
        _skillController.UseSecondSkill();
    }
}
