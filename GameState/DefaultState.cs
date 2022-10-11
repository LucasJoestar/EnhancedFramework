// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;

namespace EnhancedFramework.GameStates {
    /// <summary>
    /// Original <see cref="GameState"/>, acting as the default one when no override is applied.
    /// </summary>
    [Serializable]
    public class DefaultState : GameState {
        #region Global Members
        /// <summary>
        /// The default state, which is added on game start and never removed,
        /// <br/> uses the lower priority to make sure there is always an active state in the game, whatever might happen.
        /// </summary>
        public const int DefaultStatePriority = -1;

        public override int Priority => DefaultStatePriority;

        // -----------------------

        /// <inheritdoc cref="DefaultState"/>
        public DefaultState() : base(false) { }
        #endregion
    }
}
