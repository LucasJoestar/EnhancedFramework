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

        [SerializeField, Enhanced, Required] private TextMeshProUGUI text = null;

        [Space(5f)]

        [SerializeField] private EnumValues<SelectableState, Vector2> offset = new EnumValues<SelectableState, Vector2>(Vector2.zero);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            Vector2 _offset = offset[_state];
            text.margin = new Vector4(_offset.x, _offset.y, text.margin.y, text.margin.z);
        }
        #endregion
    }
}
#endif
