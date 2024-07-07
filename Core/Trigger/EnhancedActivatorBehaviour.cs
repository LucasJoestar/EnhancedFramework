// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
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

        public override UpdateRegistration UpdateRegistration {
            get {
                UpdateRegistration _value = base.UpdateRegistration;
                if (ActivateOnPlay) {
                    _value |= UpdateRegistration.Play;
                }

                return _value;
            }
        }

        #region Global Members
        [PropertyOrder(1), Space(10f)]

        [Tooltip("If true, activates this object on trigger enter")]
        [SerializeField] protected bool isTrigger = true;

        [Tooltip("If true, deactivates this controller on trigger exit")]
        [SerializeField, Enhanced, ShowIf(nameof(isTrigger))] protected bool onlyWhileOnTrigger = true;

        [Tooltip("If true, activates this object right after loading")]
        [SerializeField, Enhanced, ShowIf(nameof(isTrigger), ConditionType.False)] protected bool activateOnPlay = true;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Delay before activating this object (in seconds)")]
        [SerializeField, Enhanced, Range(0f, 10f)] protected float activationDelay = 0f;

        [Tooltip("Delay before deactivating this object (in seconds)")]
        [SerializeField, Enhanced, Range(0f, 10f)] protected float deactivationDelay = 0f;

        [Space(5f)]

        [Tooltip("If true delay will not be affected by the game time scale")]
        [SerializeField] protected bool unscaledTime = false;

        // -----------------------

        [PropertyOrder(9)]
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Current state of this object")]
        [SerializeField, Enhanced, ReadOnly] protected State state = State.Inactive;

        // -----------------------

        /// <summary>
        /// Whether this object should be activated when enabled or not.
        /// </summary>
        public bool ActivateOnPlay {
            get { return !isTrigger && activateOnPlay; }
        }
        #endregion

        #region Enhanced Behaviour
        private bool wasPlaying = false;

        // -----------------------

        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Reactivation.
            if (ActivateOnPlay && wasPlaying) {
                Activate();
            }
        }

        protected override void OnPlay() {
            base.OnPlay();

            // Activation.
            if (ActivateOnPlay) {
                Activate();
            }
        }

        protected override void OnBehaviourDisabled() {

            // Deactivation.
            wasPlaying = state == State.Active;
            Deactivate(true);

            base.OnBehaviourDisabled();
        }
        #endregion

        #region Behaviour
        private Action onDeactivationComplete = null;
        private Action onActivationComplete   = null;

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

            onActivationComplete ??= OnComplete;
            delay = Delayer.Call(activationDelay, onActivationComplete, unscaledTime);

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

            onDeactivationComplete ??= OnComplete;
            delay = Delayer.Call(deactivationDelay, onDeactivationComplete, unscaledTime);

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
        protected override void OnEnterTrigger(ITriggerActor _actor, EnhancedBehaviour _behaviour) {

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

            base.OnEnterTrigger(_actor, _behaviour);
            Activate();
        }

        protected override void OnExitTrigger(ITriggerActor _actor, EnhancedBehaviour _behaviour) {

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

            base.OnExitTrigger(_actor, _behaviour);
            Deactivate();
        }

        // -----------------------

        protected override bool InteractWithTrigger(EnhancedBehaviour _actor) {
            return isTrigger && base.InteractWithTrigger(_actor);
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
