// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// <see cref="SplashManager"/>-related <see cref="GameState"/>, applied while in the splash scene.
    /// </summary>
    [Serializable, DisplayName("Utility/Splash")]
    public class SplashGameState : GameState {
        #region Global Members
        /// <summary>
        /// High priority to remain above gameplay states, but lower than loadings
        /// (which need to be active to execute).
        /// </summary>
        public const int PriorityConst = 99;

        public override int Priority {
            get { return PriorityConst; }
        }

        // Persist between loading (as the game first scene is loaded during its execution).
        public override bool IsPersistent {
            get { return true; }
        }

        // -----------------------

        /// <inheritdoc cref="SplashGameState"/>
        public SplashGameState() : base(false) { }
        #endregion

        #region State Override
        public const int ChronosPriority = 999;

        // -----------------------

        public override bool OverrideChronos(out float _chronos, out int _priority) {
            _chronos = 0f;
            _priority = ChronosPriority;

            return true;
        }
        #endregion
    }
}
