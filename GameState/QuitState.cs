// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using UnityEngine;
using System;

namespace EnhancedFramework.GameStates {
    /// <summary>
    /// <see cref="GameState"/> automatically pushed on the stack when the application is being quit.
    /// </summary>
    [Serializable]
    public class QuitState : GameState {
        #region Global Members
        /// <summary>
        /// Make sure this state is behind all other gameplay states,
        /// <br/> but in front of <see cref="DefaultState"/> for a correct override on <see cref="GameStateOverride.IsQuitting"/>.
        /// </summary>
        public const int QuitStatePriority = 0;

        public override int Priority => QuitStatePriority;
        #endregion

        #region State Override
        public override void OnStateOverride(GameStateOverride _state) {
            base.OnStateOverride(_state);

            // Indicates the game is currently being quit.
            GameManager.IsQuittingApplication = true;
            _state.IsQuitting = true;
        }
        #endregion

        #region Game Quit
        [RuntimeInitializeOnLoadMethod]
        private static void OnInitialization() {
            Application.quitting += OnQuit;
        }

        private static void OnQuit() {
            CreateState<QuitState>();
        }
        #endregion
    }
}
