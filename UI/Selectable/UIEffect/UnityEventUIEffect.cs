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
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Effect/Event UI Effect"), DisallowMultipleComponent]
    public class UnityEventUIEffect : EnhancedSelectableEffect {
        #region Global Members
        [Section("Unity Event Effect")]

        [SerializeField] private EnumValues<SelectableState, UnityEvent> events = new EnumValues<SelectableState, UnityEvent>(null);
        #endregion

        #region Behaviour
        public override void OnSelectionState(EnhancedSelectable _selectable, SelectableState _state, bool _instant) {
            events[_state].Invoke();
        }
        #endregion
    }
}
