// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to send an event when an object enters or exits a <see cref="ITrigger"/>.
    /// </summary>
    [Tooltip("Sends an event when an object enters / exits a Trigger.")]
    [ActionCategory(ActionCategory.Physics)]
    public class EnhancedTriggerEvent : FsmStateAction, ITrigger {
        #region Global Members
        // -------------------------------------------
        // Trigger - Tags - Events - Object
        // -------------------------------------------

        [Tooltip("Trigger to detect trigger events on.")]
        [RequiredField, CheckForComponent(typeof(LevelTrigger))]
        public FsmOwnerDefault Trigger;

        [Tooltip("If true, checks if the Movable have all required tags to trigger the events.")]
        [RequiredField]
        public FsmBool CheckTags;

        [Tooltip("All tags to check for the Movable to have.")]
        [ObjectType(typeof(MultiTagsBehaviour))]
        [HideIf("HideTags")]
        public FsmObject Tags;

        [Tooltip("Event to send when a Movable enters the Trigger.")]
        public FsmEvent OnEnterEvent;

        [Tooltip("Event to send when a Movable exits the Trigger.")]
        public FsmEvent OnExitEvent;

        [Tooltip("The Game Object that entered / exited the Trigger.")]
        [ObjectType(typeof(GameObject))]
        [UIHint(UIHint.Variable)]
        public FsmObject StoreObject;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Trigger = null;
            CheckTags = false;
            Tags = null;
            OnEnterEvent = null;
            OnExitEvent = null;
            StoreObject = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            if (GetTrigger(out LevelTrigger _trigger)) {
                _trigger.RegisterCallback(this);
            }

            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            if (GetTrigger(out LevelTrigger _trigger)) {
                _trigger.UnregisterCallback(this);
            }
        }

        // -----------------------

        public void OnEnterTrigger(Component _component) {

            if (IsValid(_component)) {

                StoreObject.Value = _component.gameObject;
                Fsm.Event(OnEnterEvent);
            }
        }

        public void OnExitTrigger(Component _component) {

            if (IsValid(_component)) {

                StoreObject.Value = _component.gameObject;
                Fsm.Event(OnExitEvent);
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        private bool GetTrigger(out LevelTrigger _trigger) {
            GameObject _gameObject = Fsm.GetOwnerDefaultTarget(Trigger);

            if (_gameObject.IsValid()) {
                return _gameObject.TryGetComponent(out _trigger);
            }

            _trigger = null;
            return false;
        }

        private bool IsValid(Component _component) {
            return !(CheckTags.Value && (Tags.Value is MultiTagsBehaviour _tags) && !_component.HasTags(_tags.Tags));
        }
        #endregion

        #region Utility
        public bool HideTags() {
            return !CheckTags.Value;
        }
        #endregion
    }
}
