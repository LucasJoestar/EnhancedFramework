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
    /// <see cref="FsmStateAction"/> used to reset all an <see cref="AudioSnapshotController"/>.
    /// </summary>
    [Tooltip("Resets all Audio Snapshot.")]
    [ActionCategory(ActionCategory.Audio)]
    public sealed class AudioSnapshotReset : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Instant
        // -------------------------------------------

        [Tooltip("If true, instantly reset the Snapshots.")]
        public FsmBool Instant;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            Instant = false;
        }

        public override void OnEnter() {
            base.OnEnter();

            ResetSnapshots();
            Finish();
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        private void ResetSnapshots() {
            AudioManager.Instance.ResetSnapshots(Instant.Value);
        }
        #endregion
    }
}
