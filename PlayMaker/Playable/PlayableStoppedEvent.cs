// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Playables;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to send an event when a <see cref="PlayableDirector"/> is being stopped.
    /// </summary>
    [Tooltip("Sends an Event when a Playable is being stopped")]
    [ActionCategory("Playable")]
    public class PlayableStoppedEvent : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable - Event
        // -------------------------------------------

        [Tooltip("The Playable used by the event.")]
        [RequiredField, ObjectType(typeof(PlayableDirector))]
        public FsmObject Playable = null;

        [Tooltip("Event to send when the Playable is being stopped.")]
        public FsmEvent StoppedEvent;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Playable = null;
            StoppedEvent = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            if (Playable.Value is PlayableDirector _playable) {
                _playable.stopped += OnStopped;
            }

            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            if (Playable.Value is PlayableDirector _playable) {
                _playable.stopped -= OnStopped;
            }
        }

        // -----------------------

        private void OnStopped(PlayableDirector _playable) {
            Fsm.Event(StoppedEvent);
        }
        #endregion
    }
}
