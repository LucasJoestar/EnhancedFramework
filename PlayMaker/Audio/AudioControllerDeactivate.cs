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
    /// <see cref="FsmStateAction"/> used to deactivate an <see cref="AudioAssetController"/>.
    /// </summary>
    [Tooltip("Deactivates an Audio Controller.")]
    [ActionCategory(ActionCategory.Audio)]
    public class AudioControllerDeactivate : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Audio - Instant
        // -------------------------------------------

        [Tooltip("Audio Controller to deactivate.")]
        [RequiredField, ObjectType(typeof(AudioAssetController))]
        public FsmObject Audio;

        [Tooltip("If true, instantly deactivates this Controller.")]
        public FsmBool Instant;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Audio = null;
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
            if (Audio.Value is AudioAssetController _audio) {
                _audio.Deactivate(Instant.Value);
            }
        }
        #endregion
    }
}
