// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// Completely pauses the game and set its chornos to 0 while active.
    /// </summary>
    [Serializable, DisplayName("Utility/Pause Chronos")]
    public class PauseChronosGameState : GameState {
        #region Global Members
        /// <summary>
        /// One of the highest priority, to prevent most of the other states to continue updating.
        /// </summary>
        public const int PriorityConst = int.MaxValue - 999;

        public override int Priority {
            get { return PriorityConst; }
        }

        // Prevent from discarding this state.
        public override bool IsPersistent {
            get { return true; }
        }

        public override IGameStateLifetimeCallback LifetimeCallback {
            get { return ChronosManager.Instance; }
        }
        #endregion

        #region State Override
        public const int ChronosPriority = int.MaxValue;

        // -----------------------

        public override void OnGameStateOverride(GameStateOverride _state) {
            base.OnGameStateOverride(_state);

            // Set the cursor free.
            _state.IsCursorVisible = true;
            _state.CursorLockMode = CursorLockMode.None;
        }

        public override bool OverrideChronos(out float _chronos, out int _priority) {
            _chronos = 0f;
            _priority = ChronosPriority;

            return true;
        }
        #endregion
    }
}
