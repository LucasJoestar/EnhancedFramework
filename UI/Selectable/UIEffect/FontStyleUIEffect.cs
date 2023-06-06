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
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Effect/Font UI Effect"), DisallowMultipleComponent]
    public class FontStyleUIEffect : EnhancedSelectableEffect {
        #region Global Members
        [Section("Font Style Effect")]

        [SerializeField, Enhanced, Required] private TextMeshProUGUI text = null;

        [Space(5f)]

        [SerializeField] private EnumValues<SelectableState, FontStyles> styles = new EnumValues<SelectableState, FontStyles>(FontStyles.Normal);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedSelectable _selectable, SelectableState _state, bool _instant) {
            text.fontStyle = styles[_state];
        }
        #endregion
    }
}
#endif
