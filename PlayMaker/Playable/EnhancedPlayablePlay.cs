// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Playables;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to play a <see cref="EnhancedPlayablePlayer"/>.
    /// </summary>
    [Tooltip("Plays an Enhanced Playable Player")]
    [ActionCategory("Playable")]
    public sealed class EnhancedPlayablePlay : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable - Paused - Stopped
        // -------------------------------------------

        [Tooltip("The Playable to play.")]
        [RequiredField, ObjectType(typeof(EnhancedPlayablePlayer))]
        public FsmObject Playable = null;

        [Tooltip("Event to send when the Playable is paused.")]
        public FsmEvent PausedEvent;

        [Tooltip("Event to send when the Playable is stopped.")]
        public FsmEvent StoppedEvent;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Playable     = null;
            PausedEvent  = null;
            StoppedEvent = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            if (Playable.Value is EnhancedPlayablePlayer _playable) {
                var _director = _playable.GetPlayableDirector();

                _director.paused    += OnPaused;
                _director.stopped   += OnStopped;

                _playable.Play();
            }

            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            if (Playable.Value is EnhancedPlayablePlayer _playable) {
                var _director = _playable.GetPlayableDirector();

                _director.paused    -= OnPaused;
                _director.stopped   -= OnStopped;
            }
        }

        // -----------------------

        private void OnPaused(PlayableDirector _playable) {
            Fsm.Event(PausedEvent);
        }

        private void OnStopped(PlayableDirector _playable) {
            Fsm.Event(StoppedEvent);
        }
        #endregion
    }
}
