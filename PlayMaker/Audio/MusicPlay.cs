// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to activate an <see cref="AudioMusicController"/>.
    /// </summary>
    [Tooltip("Plays a Music.")]
    [ActionCategory(ActionCategory.Audio)]
    public class MusicPlay : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Music - Instant - Stop
        // -------------------------------------------

        [Tooltip("Music to play.")]
        [RequiredField, ObjectType(typeof(AudioMusicController))]
        public FsmObject Music;

        [Tooltip("If true, instantly play this Music.")]
        public FsmBool Instant;

        [Tooltip("Stops this Music when exiting the state.")]
        public FsmBool StopOnExit;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Music = null;
            Instant = false;
            StopOnExit = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            Activate();
            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            Deactivate();
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        private void Activate() {
            if (Music.Value is AudioAssetController _audio) {
                _audio.Activate(Instant.Value);
            }
        }

        private void Deactivate() {
            if (StopOnExit.Value && (Music.Value is AudioAssetController _audio)) {
                _audio.Deactivate(Instant.Value);
            }
        }
        #endregion
    }
}
