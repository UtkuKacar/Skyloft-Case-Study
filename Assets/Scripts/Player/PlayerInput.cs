using UnityEngine;
using Skyloft.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Skyloft.Player
{
    /// <summary>
    /// Reads movement input from the VirtualJoystick UI or keyboard fallback (in Editor/Standalone).
    /// Implements IMovementInput to decouple input gathering from movement mechanics.
    /// </summary>
    public class PlayerInput : MonoBehaviour, IMovementInput
    {
        [Header("UI Input Source")]
        [Tooltip("Virtual joystick providing touch/drag input.")]
        [SerializeField] private VirtualJoystick virtualJoystick;

        public Vector2 MoveInput
        {
            get
            {
                if (!isActiveAndEnabled)
                    return Vector2.zero;
                // Primary: Virtual Joystick
                if (virtualJoystick != null && virtualJoystick.MoveInput.sqrMagnitude > 0f)
                {
                    return virtualJoystick.MoveInput;
                }

                // Editor & Standalone keyboard fallback (WASD / Arrows) for rapid testing
#if ENABLE_INPUT_SYSTEM
                var keyboard = Keyboard.current;
                if (keyboard != null)
                {
                    float x = 0f;
                    float y = 0f;

                    if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) x -= 1f;
                    if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) x += 1f;
                    if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) y += 1f;
                    if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) y -= 1f;

                    Vector2 keyInput = new Vector2(x, y);
                    if (keyInput.sqrMagnitude > 1f)
                    {
                        keyInput.Normalize();
                    }

                    if (keyInput.sqrMagnitude > 0f)
                    {
                        return keyInput;
                    }
                }
#endif
                return Vector2.zero;
            }
        }

        public bool HasInput => MoveInput.sqrMagnitude > 0.0001f;

        public void SetJoystick(VirtualJoystick joystick)
        {
            virtualJoystick = joystick;
        }
    }
}
