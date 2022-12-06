// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// Original <see cref="GameState"/>, acting as the default one when no override is applied.
    /// </summary>
    [Serializable, DisplayName("<Default>")]
    public class DefaultGameState : GameState {
        #region Global Members
        /// <summary>
        /// The default state, which is added on game start and never removed,
        /// <br/> uses the lower priority to make sure there is always an active state in the game, whatever might happen.
        /// </summary>
        public const int PriorityConst = -1;

        public override int Priority {
            get { return PriorityConst; }
        }

        public override bool IsPersistent {
            get { return true; }
        }

        // -----------------------

        /// <inheritdoc cref="DefaultGameState"/>
        public DefaultGameState() : base(false) { }
        #endregion
    }
}
