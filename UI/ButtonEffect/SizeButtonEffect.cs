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
    /// Set the size of the button text.
    /// </summary>
    public class SizeButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Size Effect")]

        [SerializeField] private EnumValues<SelectableState, float> size = new EnumValues<SelectableState, float>(1f);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            // Issue management.
            if (!(_button.targetGraphic is TextMeshProUGUI _text)) {
                return;
            }

            _text.fontSize = size[_state];
        }
        #endregion
    }
}
#endif