// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Centralizes the behaviour of an area of multiple <see cref="EnhancedBehaviour"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Trigger/Level Trigger Area")]
    public class LevelTriggerArea : LevelTrigger {
        #region Global Members
        [Section("Trigger Area")]

        // -----------------------

        [SerializeField, HideInInspector] private LevelTrigger[] triggers = new LevelTrigger[0];
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Trigger registration.
            foreach (LevelTrigger _trigger in triggers) {
                _trigger.RegisterCallback(this);
            }
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Trigger unregistration.
            foreach (LevelTrigger _trigger in triggers) {
                _trigger.UnregisterCallback(this);
            }
        }

        // -------------------------------------------
        // Editor
        // -------------------------------------------

        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            if (Application.isPlaying) {
                return;
            }

            // Get references.
            triggers = GetComponentsInChildren<LevelTrigger>();

            /*if (callbacks.Count == 0) {

                foreach (ITrigger _trigger in GetComponents<ITrigger>()) {
                    if (!ReferenceEquals(_trigger, this)) {
                        callbacks.Add(new SerializedInterface<ITrigger>(_trigger));
                    }
                }
            }*/
        }
        #endif
        #endregion

        #region Trigger
        private int triggerCount = 0;

        // -----------------------

        protected override void OnEnterTrigger(Component _component) {
            base.OnEnterTrigger(_component);

            if (++triggerCount == 1) {

                /*foreach (ITrigger _callback in callbacks) {
                    _callback.OnEnterTrigger(_component);
                }*/
            }
        }

        protected override void OnExitTrigger(Component _component) {
            base.OnExitTrigger(_component);

            if (--triggerCount == 0) {

                /*foreach (ITrigger _callback in callbacks) {
                    _callback.OnExitTrigger(_component);
                }*/
            }
        }
        #endregion
    }
}
