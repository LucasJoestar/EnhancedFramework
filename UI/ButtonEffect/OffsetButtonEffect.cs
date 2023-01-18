// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if TEXT_MESH_PRO_PACKAGE
using EnhancedEditor;
using UnityEngine;
using TMPro;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Set the offset of the button text.
    /// </summary>
    public class OffsetButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Offset Effect")]

        [SerializeField] private EnumValues<SelectableState, Vector2> offset = new EnumValues<SelectableState, Vector2>(Vector2.zero);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            // Issue management.
            if (!(_button.targetGraphic is TextMeshProUGUI _text)) {
                return;
            }

            Vector2 _offset = offset[_state];
            _text.margin = new Vector4(_offset.x, _offset.y, _text.margin.y, _text.margin.z);
        }
        #endregion
    }
}
#endif
