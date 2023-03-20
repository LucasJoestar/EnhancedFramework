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
    /// <see cref="FsmStateAction"/> used to pause or resume the particle system global state.
    /// </summary>
    [Tooltip("Pauses/Resumes the Particle System global state.")]
    [ActionCategory(ActionCategory.Effects)]
    public class GlobalParticleSystemPause : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Pause - Reset
        // -------------------------------------------

        [Tooltip("Set to True to pause, False to resume.")]
        [RequiredField]
        public FsmBool Pause;

        [Tooltip("Reset the initial pause state when exiting the state.")]
        public FsmBool ResetOnExit;
        #endregion

        #region Behaviour
        private bool wasPaused = false;

        // -----------------------

        public override void Reset() {
            base.Reset();

            Pause = true;
            ResetOnExit = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            DoPause();
            Finish();
        }

        public override void OnExit() {
            base.OnExit();

            Revert();
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        private void DoPause() {
            wasPaused = ParticleSystemManager.Instance.IsPaused;
            ParticleSystemManager.Instance.Pause(Pause.Value);
        }

        private void Revert() {
            if (ResetOnExit.Value) {
                ParticleSystemManager.Instance.Pause(wasPaused);
            }
        }
        #endregion
    }
}
