// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace EnhancedFramework.UI {
    /// <summary>
    /// <see cref="FadingGroupController"/> used to manage UI interaction.
    /// </summary>
    public class FadingGroupInteractionController : FadingGroupController {
        #region Global Members
        [Section("Fading Group Interaction Controller")]

        [SerializeField] private bool useCancelCallback = false;
        [SerializeField] private UnityEvent onCancel = new UnityEvent();
        #endregion

        #region Callback
        private static readonly PairCollection<FadingGroup, FadingGroupInteractionController> groupBuffer = new PairCollection<FadingGroup, FadingGroupInteractionController>();

        /// <summary>
        /// Called when requesting to enable the interface inputs.
        /// </summary>
        public static event Action OnEnableInputs = null;

        /// <summary>
        /// Called when requesting to disable the interface inputs.
        /// </summary>
        public static event Action OnDisableInputs = null;

        // -----------------------

        public override void OnShowStarted(FadingGroup _group) {
            base.OnShowStarted(_group);

            // Disable inputs until completion.
            OnDisableInputs?.Invoke();
        }

        public override void OnShowPerformed(FadingGroup _group) {
            base.OnShowPerformed(_group);

            // Disable last group.
            if (groupBuffer.SafeLast(out var _active) && (_active.First != _group)) {
                _active.First.SetInteractable(false);
            }

            // Register group.
            groupBuffer.Add(_group, this);
        }

        public override void OnShowCompleted(FadingGroup _group) {
            base.OnShowCompleted(_group);

            // Enable inputs once completed.
            OnEnableInputs?.Invoke();
        }

        // -----------------------

        public override void OnHideStarted(FadingGroup _group) {
            base.OnHideStarted(_group);

            // Disable inputs until completion.
            OnDisableInputs?.Invoke();
        }

        public override void OnHidePerformed(FadingGroup _group) {
            base.OnHidePerformed(_group);

            bool _wasActive = groupBuffer.SafeLast(out var _lastActive) && (_lastActive.First == _group);

            // Unregister group.
            groupBuffer.Remove(_group);

            // Enable last group.
            if (_wasActive && groupBuffer.SafeLast(out var _active)) {

                FadingGroup _activeGroup = _active.First;

                _activeGroup.SetInteractable(true);
                _activeGroup.EnableCanvas(true, true);
            }
        }

        public override void OnHideCompleted(FadingGroup _group) {
            base.OnHideCompleted(_group);

            // Enable inputs once completed.
            if (groupBuffer.Count != 0) {
                OnEnableInputs?.Invoke();
            }
        }

        // -----------------------

        /// <summary>
        /// Can be called when receiving a cancel event from UI, which will execute the last registered associated group callback.
        /// </summary>
        /// <returns>True if a cancel event could be called, false otherwise.</returns>
        public static bool OnGroupCancelEvent() {

            // Call last registered cancel event.
            for (int i = groupBuffer.Count; i-- > 0;) {

                FadingGroupInteractionController _controller = groupBuffer[i].Second;

                if (_controller.useCancelCallback) {

                    // Call event.
                    _controller.onCancel.Invoke();
                    return true;
                }
            }

            return false;
        }
        #endregion
    }
}
