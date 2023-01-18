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
    /// Set the font styles of the button text.
    /// </summary>
    public class FontStyleButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Font Style Effect")]

        [SerializeField] private EnumValues<SelectableState, FontStyles> styles = new EnumValues<SelectableState, FontStyles>(FontStyles.Normal);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            // Issue management.
            if (!(_button.targetGraphic is TextMeshProUGUI _text)) {
                return;
            }

            _text.fontStyle = styles[_state];
        }
        #endregion
    }
}
#endif