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
    /// <see cref="FsmStateAction"/> used to deactivate an <see cref="AudioAmbientController"/>.
    /// </summary>
    [Tooltip("Deactivates an Audio Ambient.")]
    [ActionCategory(ActionCategory.Audio)]
    public sealed class AudioAmbientDeactivate : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Ambient - Instant
        // -------------------------------------------

        [Tooltip("Audio Ambient to deactivate.")]
        [RequiredField, ObjectType(typeof(AudioAmbientController))]
        public FsmObject Ambient;

        [Tooltip("If true, instantly deactivates this Ambient.")]
        public FsmBool Instant;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Ambient = null;
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
            if (Ambient.Value is AudioAmbientController _ambient) {
                _ambient.Deactivate(Instant.Value);
            }
        }
        #endregion
    }
}
