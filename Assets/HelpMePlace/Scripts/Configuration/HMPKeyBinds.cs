using UnityEngine;

namespace HelpMePlace
{
    /// <summary>
    /// All KeyBinds for use in the system.
    /// </summary>
    public class HMPKeyBinds : MonoBehaviour
    {
#if UNITY_EDITOR
        /// <summary>
        /// All of these constants can be changed to whatever is required. 
        /// These are shown on the editor UI, if the Configuration > Show keybinds is checked. 
        /// </summary>

        internal const KeyCode ToggleOverlayMap = KeyCode.Tab;

        internal const KeyCode ExitTool = KeyCode.Escape;

        internal const KeyCode PlaceObject = KeyCode.Space;

        internal const KeyCode SelectionMoveLeft = KeyCode.A;

        internal const KeyCode SelectionMoveRight = KeyCode.D;

        internal const KeyCode SelectNextGroup = KeyCode.W;

        internal const KeyCode SelectPreviousGroup = KeyCode.S;

        internal const KeyCode CycleRotationModes = KeyCode.R;

        internal const KeyCode CycleRotationAxis = KeyCode.G;

        internal const KeyCode SetLookAtPosition = KeyCode.E;

        internal const KeyCode RandomFromRowToggle = KeyCode.T;

        internal const KeyCode HandleChangeMatchNormalOfGround = KeyCode.Q;
        
        public static KeyCode SimulatePhysics = KeyCode.V;

        public static KeyCode FenceMode = KeyCode.C;
        public static KeyCode FenceModeIncreaseDistance = KeyCode.KeypadPlus;
        public static KeyCode FenceModeDecreaseDistance = KeyCode.KeypadMinus;
        public static KeyCode FenceModeIncreaseWidth = KeyCode.RightBracket;
        public static KeyCode FenceModeDecreaseWidth = KeyCode.LeftBracket;

        //public static KeyCode FenceModeCancelPlacement = KeyCode.B;
        //public static KeyCode FenceModeConfirmPlacement = KeyCode.Space;

        public static KeyCode ReEnableHMP = KeyCode.Keypad5;

        public static KeyCode Keybind1 = KeyCode.Keypad1;
        public static KeyCode Keybind2 = KeyCode.Keypad2;
        public static KeyCode Keybind3 = KeyCode.Keypad3;
        public static KeyCode Keybind4 = KeyCode.Keypad4;
        public static KeyCode Keybind5 = KeyCode.Keypad5;
        public static KeyCode Keybind6 = KeyCode.Keypad6;
        public static KeyCode Keybind7 = KeyCode.Keypad7;
        public static KeyCode Keybind8 = KeyCode.Keypad8;
        public static KeyCode Keybind9 = KeyCode.Keypad9;
        public static KeyCode Keybind0 = KeyCode.Keypad0;
#endif
    }
}
