// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Video;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to send an event when a <see cref="EnhancedVideoPlayer"/> is being paused.
    /// </summary>
    [Tooltip("Sends an Event when an Enhanced Video Player is being paused")]
    [ActionCategory("Video")]
    public class EnhancedVideoPausedEvent : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable - Event
        // -------------------------------------------

        [Tooltip("The Video used by the event.")]
        [RequiredField, ObjectType(typeof(EnhancedVideoPlayer))]
        public FsmObject Video = null;

        [Tooltip("Event to send when the Video is being paused.")]
        public FsmEvent PausedEvent;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Video = null;
            PausedEvent = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            if (Video.Value is EnhancedVideoPlayer _video) {
                _video.Paused += OnPaused;
            }

            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            if (Video.Value is EnhancedVideoPlayer _video) {
                _video.Paused -= OnPaused;
            }
        }

        // -----------------------

        private void OnPaused(VideoPlayer _video) {
            Fsm.Event(PausedEvent);
        }
        #endregion
    }
}
