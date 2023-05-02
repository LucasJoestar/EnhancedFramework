// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Sets the position and size of a <see cref="RectTransform"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Button Effect/RectTransform Button Effect")]
    public class RectTransformButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Audio Effect")]

        [Tooltip("RectTransform instance to set the position and size")]
        [SerializeField, Enhanced, Required] private RectTransform source = null;

        [Tooltip("Target position and size of the RectTransform")]
        [SerializeField, Enhanced, Required] private RectTransform target = null;

        [Space(5f)]

        [Tooltip("Whether to set the RectTransform position and size or not")]
        [SerializeField] private EnumValues<SelectableState, bool> enable = new EnumValues<SelectableState, bool>();
        #endregion

        #region Behaviour
        private static readonly Vector3[] cornerBuffer = new Vector3[4];

        // -----------------------

        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {

            // Ignore.
            if (!enable.GetValue(_state, out bool _enable) || !_enable) {
                return;
            }

            // Clockwise: Bottom-Left, Top-Left, Top-Right, Bottom-Right.
            target.GetWorldCorners(cornerBuffer);

            for (int i = 0; i < cornerBuffer.Length; i++) {

                Vector3 _corner = cornerBuffer[i];
                cornerBuffer[i] = source.InverseTransformPoint(_corner);
            }

            Vector3 _size       = cornerBuffer[2] - cornerBuffer[0];
            Vector2 _position   = source.anchoredPosition + (Vector2)((cornerBuffer[0] + (_size / 2f)));

            source.anchoredPosition = _position;
            source.sizeDelta = _size;
        }
        #endregion
    }
}
