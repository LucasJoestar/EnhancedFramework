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
    /// Set the font material of the button text.
    /// </summary>
    public class FontMaterialButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Font Material Effect")]

        [SerializeField] private EnumValues<SelectableState, Material> styles = new EnumValues<SelectableState, Material>(null);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            // Issue management.
            if (!(_button.targetGraphic is TextMeshProUGUI _text)) {
                return;
            }

            _text.fontMaterial = styles[_state];
        }
        #endregion
    }
}
#endif
