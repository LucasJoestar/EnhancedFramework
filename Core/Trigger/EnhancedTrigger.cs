// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base interface for a trigger object.
    /// </summary>
    public interface ITrigger {
        #region Content
        /// <summary>
        /// Called whenever an object enters this trigger.
        /// </summary>
        /// <param name="_component"><see cref="Component"/> of the object entering in this trigger.</param>
        void OnEnterTrigger(Component _component);

        /// <summary>
        /// Called whenever an object exits this trigger.
        /// </summary>
        /// <param name="_component"><see cref="Component"/> of the object exiting from this trigger.</param>
        void OnExitTrigger(Component _component);
        #endregion
    }

    /// <summary>
    /// Base <see cref="EnhancedBehaviour"/> class to inherit your own triggers from.
    /// </summary>
	public abstract class EnhancedTrigger : EnhancedBehaviour, ITrigger {
        #region Trigger
        /// <inheritdoc cref="ITrigger.OnEnterTrigger(Component)"/>
        void ITrigger.OnEnterTrigger(Component _component) {

            if (InteractWithTrigger(_component)) {
                OnEnterTrigger(_component);
            }
        }

        /// <inheritdoc cref="ITrigger.OnExitTrigger(Component)"/>
        void ITrigger.OnExitTrigger(Component _component) {

            if (InteractWithTrigger(_component)) {
                OnExitTrigger(_component);
            }
        }

        // -----------------------

        /// <inheritdoc cref="ITrigger.OnEnterTrigger(Component)"/>
        protected virtual void OnEnterTrigger(Component _component) { }

        /// <inheritdoc cref="ITrigger.OnExitTrigger(Component)"/>
        protected virtual void OnExitTrigger(Component _component) { }

        // -----------------------

        /// <summary>
        /// Called when a <see cref="Component"/> tries to interact with this trigger.
        /// </summary>
        /// <param name="_component"><see cref="Component"/> interacting with trigger.</param>
        /// <returns>True if this <see cref="Component"/> should interact with this trigger, false otherwise.</returns>
        protected virtual bool InteractWithTrigger(Component _component) {
            return true;
        }
        #endregion
    }

    /// <summary>
    /// Base ready to use <see cref="EnhancedBehaviour"/> class, that can be inherited for a quick 'activation' behaviour.
    /// </summary>
    public abstract class EnhancedActivatorBehaviour : EnhancedTrigger {
        #region State
        /// <summary>
        /// References all available states for this object.
        /// </summary>
        public enum State {
            Inactive        = 0,
            Active          = 1,

            ActivateDelay   = 2,
            DeactivateDelay = 3,
        }
        #endregion

        #region Global Members
        [PropertyOrder(1), Space(10f)]

        [Tooltip("If true, activates this object on trigger enter")]
        [SerializeField] protected bool isTrigger = true;

        [Tooltip("If true, deactivates this controller on trigger exit")]
        [SerializeField, Enhanced, ShowIf("isTrigger")] protected bool onlyWhileOnTrigger = true;

        [Tooltip("If true, activates this object when enabled")]
        [SerializeField, Enhanced, ShowIf("isTrigger", ConditionType.False)] protected bool activateOnEnabled = true;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Delay before activating this object (in seconds)")]
        [SerializeField, Enhanced, Range(0f, 10f)] protected float activationDelay = 0f;

        [Tooltip("Delay before deactivating this object (in seconds)")]
        [SerializeField, Enhanced, Range(0f, 10f)] protected float deactivationDelay = 0f;

        // -----------------------

        [PropertyOrder(9)]
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Current state of this object")]
        [SerializeField, Enhanced, ReadOnly] protected State state = State.Inactive;

        // -----------------------

        /// <summary>
        /// Whether this object should be activated when enabled or not.
        /// </summary>
        public bool ActivateOnEnabled {
            get { return !isTrigger && activateOnEnabled; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Activation.
            if (ActivateOnEnabled) {
                Activate();
            }
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Deactivation.
            Deactivate(true);
        }
        #endregion

        #region Behaviour
        protected DelayHandler delay = default;

        // -------------------------------------------
        // Activation
        // -------------------------------------------

        /// <summary>
        /// Activates this object.
        /// </summary>
        /// <param name="_instant">If true, instantly activates this object, ignoring any delay.</param>
        [Button(ActivationMode.Play, SuperColor.Green, IsDrawnOnTop = false)]
        public void Activate(bool _instant = false) {

            // State check.
            switch (state) {
                case State.Active:
                    return;

                case State.ActivateDelay:
                    if (!_instant) {
                        return;
                    }
                    break;

                // If object whas not inactive yet, simply cancel its operation.
                case State.DeactivateDelay:
                    delay.Cancel();
                    SetState(State.Active);
                    return;

                case State.Inactive:
                default:
                    break;
            }

            delay.Cancel();

            if (_instant) {

                OnComplete();
                return;
            }

            SetState(State.ActivateDelay);
            delay = Delayer.Call(activationDelay, OnComplete, false);

            // ----- Local Method ----- \\

            void OnComplete() {
                OnActivation();
            }
        }

        /// <summary>
        /// Deactivates this object and resets its behaviour.
        /// </summary>
        /// <param name="_instant">If true, instantly deactivates this object, ignoring any delay.</param>
        [Button(ActivationMode.Play, SuperColor.Crimson, IsDrawnOnTop = false)]
        public void Deactivate(bool _instant = false) {

            // State check.
            switch (state) {
                case State.Inactive:
                    return;

                case State.DeactivateDelay:
                    if (!_instant) {
                        return;
                    }
                    break;

                // If object whas not active yet, simply cancel its operation.
                case State.ActivateDelay:
                    delay.Cancel();
                    SetState(State.Inactive);
                    return;

                case State.Active:
                default:
                    break;
            }

            delay.Cancel();

            if (_instant) {

                OnComplete();
                return;
            }

            SetState(State.DeactivateDelay);
            delay = Delayer.Call(deactivationDelay, OnComplete, false);

            // ----- Local Method ----- \\

            void OnComplete() {
                OnDeactivation();
            }
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        /// <summary>
        /// Called whenever this object is being activated.
        /// </summary>
        protected virtual void OnActivation() {
            SetState(State.Active);
        }

        /// <summary>
        /// Called whenever this object is being deactivated.
        /// </summary>
        protected virtual void OnDeactivation() {
            SetState(State.Inactive);
        }
        #endregion

        #region Trigger
        protected override void OnEnterTrigger(Component _component) {

            // Check if this object is already active.
            // There should always be only one actor in the area.
            switch (state) {

                case State.Active:
                case State.ActivateDelay:
                    return;

                case State.Inactive:
                case State.DeactivateDelay:
                default:
                    break;
            }

            base.OnEnterTrigger(_component);
            Activate();
        }

        protected override void OnExitTrigger(Component _component) {

            if (!onlyWhileOnTrigger) {
                return;
            }

            // Check if the object is the active actor.
            switch (state) {

                // Ignore.
                case State.Inactive:
                case State.DeactivateDelay:
                    return;

                case State.Active:
                case State.ActivateDelay:
                default:
                    break;
            }

            base.OnExitTrigger(_component);
            Deactivate();
        }

        // -----------------------

        protected override bool InteractWithTrigger(Component _component) {
            return isTrigger && base.InteractWithTrigger(_component);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Sets the state of this object.
        /// </summary>
        /// <param name="_state">This object state.</param>
        protected void SetState(State _state) {
            state = _state;
        }
        #endregion
    }
}
