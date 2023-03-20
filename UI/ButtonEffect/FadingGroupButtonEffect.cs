// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;
using EnhancedFramework.Core;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Fades a <see cref="FadingGroup"/>.
    /// </summary>
    public class FadingGroupButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Fading Group Effect")]

        [SerializeField, Enhanced, Required] private FadingObjectBehaviour group = null;

        [Space(5f)]

        [SerializeField] private EnumValues<SelectableState, FadingMode> fade = new EnumValues<SelectableState, FadingMode>(FadingMode.None);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            group.Fade(fade[_state]);
        }
        #endregion
    }
}
