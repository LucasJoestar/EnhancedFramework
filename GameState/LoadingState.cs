// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;

namespace EnhancedFramework.GameStates {
    /// <summary>
    /// Default <see cref="GameState"/> applied when the game enters a loading state.
    /// </summary>
    [Serializable]
    public class LoadingState : GameState {
        #region Global Members
        /// <summary>
        /// Uses a high priority to make sure the state is the active one.
        /// </summary>
        public const int LoadingStatePriority = 999;

        public override int Priority => LoadingStatePriority;
        #endregion

        #region State Override
        public override void OnStateOverride(GameStateOverride _state) {
            base.OnStateOverride(_state);

            _state.IsLoading = true;
        }
        #endregion
    }
}
