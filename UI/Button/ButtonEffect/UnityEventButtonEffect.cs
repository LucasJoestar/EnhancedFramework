// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Events;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Invoke a <see cref="UnityEvent"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Button Effect/Event Button Effect"), DisallowMultipleComponent]
    public class UnityEventButtonEffect : EnhancedButtonEffect {
        #region Global Members
        [Section("Unity Event Effect")]

        [SerializeField] private EnumValues<SelectableState, UnityEvent> events = new EnumValues<SelectableState, UnityEvent>(null);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant) {
            events[_state].Invoke();
        }
        #endregion
    }
}
