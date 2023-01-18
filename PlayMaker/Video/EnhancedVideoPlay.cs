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
    /// <see cref="FsmStateAction"/> used to play a <see cref="EnhancedVideoPlayer"/>.
    /// </summary>
    [Tooltip("Plays an Enhanced Video Player")]
    [ActionCategory("Video")]
    public class EnhancedVideoPlay : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable - Paused - Resumed - Stopped
        // -------------------------------------------

        [Tooltip("The Playable to play.")]
        [RequiredField, ObjectType(typeof(EnhancedVideoPlayer))]
        public FsmObject Video = null;

        [Tooltip("Event to send when the Playable is paused.")]
        public FsmEvent PausedEvent;

        [Tooltip("Event to send when the Playable is resumed.")]
        public FsmEvent ResumedEvent;

        [Tooltip("Event to send when the Playable has stopped.")]
        public FsmEvent StoppedEvent;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Video = null;
            PausedEvent = null;
            ResumedEvent = null;
            StoppedEvent = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            if (Video.Value is EnhancedVideoPlayer _video) {
                _video.Paused   += OnPaused;
                _video.Resumed  += OnResumed;
                _video.Stopped  += OnStopped;

                _video.Play();
            }

            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            if (Video.Value is EnhancedVideoPlayer _video) {
                _video.Paused   -= OnPaused;
                _video.Resumed  -= OnResumed;
                _video.Stopped  -= OnStopped;
            }
        }

        // -----------------------

        private void OnPaused(VideoPlayer _video) {
            Fsm.Event(PausedEvent);
        }

        private void OnResumed(VideoPlayer _video) {
            Fsm.Event(ResumedEvent);
        }

        private void OnStopped(VideoPlayer _video) {
            Fsm.Event(StoppedEvent);
        }
        #endregion
    }
}
