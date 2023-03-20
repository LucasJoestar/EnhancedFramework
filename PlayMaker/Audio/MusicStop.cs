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
    /// <see cref="FsmStateAction"/> used to deactivate an <see cref="AudioMusicController"/>.
    /// </summary>
    [Tooltip("Stops a Music.")]
    [ActionCategory(ActionCategory.Audio)]
    public class MusicStop : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Music - Instant
        // -------------------------------------------

        [Tooltip("Music to stop.")]
        [RequiredField, ObjectType(typeof(AudioMusicController))]
        public FsmObject Music;

        [Tooltip("If true, instantly stops this Music.")]
        public FsmBool Instant;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Music = null;
            Instant = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            Deactivate();
            Finish();
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        private void Deactivate() {
            if (Music.Value is AudioMusicController _music) {
                _music.Deactivate(Instant.Value);
            }
        }
        #endregion
    }
}
