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
    /// <see cref="FsmStateAction"/> used to stop a <see cref="EnhancedPlayablePlayer"/>.
    /// </summary>
    [Tooltip("Stops an Enhanced Playable Player")]
    [ActionCategory("Playable")]
    public class EnhancedPlayableStop : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable
        // -------------------------------------------

        [Tooltip("The Playable to stop.")]
        [RequiredField, ObjectType(typeof(EnhancedPlayablePlayer))]
        public FsmObject Playable = null;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Playable = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            Stop();
            Finish();
        }

        // -----------------------

        private void Stop() {
            if (Playable.Value is EnhancedPlayablePlayer _playable) {
                _playable.Stop();
            }
        }
        #endregion
    }
}
