using UnityEngine;
using UnityEngine.InputSystem;

public class SelectionController : MonoBehaviour
{
    private CharacterSelectButton[] _buttons;
    private int _selectedIndex;

    public void Initialize(CharacterSelectButton[] buttons)
    {
        _buttons = buttons;

        if (_buttons == null || _buttons.Length == 0)
        {
            Debug.LogWarning("No selectable characters are available.");
            return;
        }

        Select(0);
    }

    /// <summary>
    /// ボタンのセレクト機能
    /// </summary>
    /// <param name="index"></param>
    public void Select(int index)
    {
        if (_buttons == null || index < 0 || index >= _buttons.Length)
        {
            return;
        }

        _buttons[_selectedIndex].SetSelected(false);
        _selectedIndex = index;
        _buttons[_selectedIndex].Select();
    }

    private void Update()
    {
        if (_buttons == null || _buttons.Length == 0 || Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            Move(-1);
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            Move(1);
        }
    }

    /// <summary>
    /// 十字キーの移動で使うやつ
    /// </summary>
    /// <param name="direction"></param>
    private void Move(int direction)
    {
        Select(_selectedIndex + direction);
    }
}
