// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Enum referencing all <see cref="Selectable"/> possible states.
    /// </summary>
    public enum SelectableState {
        Normal,
        Highlighted,
        Pressed,
        Selected,
        Disabled
    }

    /// <summary>
    /// Base class to inherit your own <see cref="EnhancedButton"/> effects from.
    /// </summary>
    public abstract class EnhancedButtonEffect : EnhancedBehaviour {
        #region Behaviour
        /// <summary>
        /// Called when changing the selection state of the associated button.
        /// </summary>
        /// <param name="_button">Source button of this effect.</param>
        /// <param name="_state"><see cref="SelectableState"/> of the button.</param>
        /// <param name="_instant">Whether the effect should be applied instantly or not.</param>
        public abstract void OnSelectionState(EnhancedButton _button, SelectableState _state, bool _instant);
        #endregion
    }

    /// <summary>
    /// Enhanced <see cref="Button"/> behaviour, being automatically selected on mouse hover.
    /// </summary>
    public class EnhancedButton : Button {
        #region Global Members
        [Tooltip("Automatically selects this button whenever its GameObject gets enabled")]
        [SerializeField, HideInInspector] public bool AutoSelectOnEnabled = false;

        [Space(5f)]

        [SerializeField, HideInInspector] public EnhancedButtonEffect[] Effects = new EnhancedButtonEffect[0];
        #endregion

        #region Mono Behaviour
        protected override void OnEnable() {
            base.OnEnable();
            
            // Selection.
            if (AutoSelectOnEnabled) {
                Select();
            } else {
                OnDeselect(null);
            }
        }
        #endregion

        #region Selectable
        public override void OnPointerEnter(PointerEventData eventData) {
            base.OnPointerEnter(eventData);

            // Select on hover.
            Select();
        }

        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);

            // Simulate pointer exit.
            OnPointerExit(null);
        }

        protected override void DoStateTransition(SelectionState _state, bool _instant) {
            base.DoStateTransition(_state, _instant);

            // Apply effects.
            SelectableState _selectableState = GetSelectableState(_state);

            foreach (var _effect in Effects) {
                if (_effect == null) {
                    continue;
                }

                _effect.OnSelectionState(this, _selectableState, _instant);
            }
        }
        #endregion

        #region Utility
        private SelectableState GetSelectableState(SelectionState _state) {
            switch (_state) {
                case SelectionState.Normal:
                default:
                    return SelectableState.Normal;

                case SelectionState.Highlighted:
                    return SelectableState.Highlighted;

                case SelectionState.Pressed:
                    return SelectableState.Pressed;

                case SelectionState.Selected:
                    return SelectableState.Selected;

                case SelectionState.Disabled:
                    return SelectableState.Disabled;

            }
        }
        #endregion
    }
}
