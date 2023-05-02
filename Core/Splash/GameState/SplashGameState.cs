// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core.GameStates {

    /// <summary>
    /// Base interface for every splash <see cref="GameState"/>.
    /// <br/> Used within a <see cref="SerializedType{T}"/> instead of the generic <see cref="SplashGameState{T}"/> class.
    /// </summary>
    internal interface ISplashState { }

    /// <summary>
    /// Base <see cref="GameState{T}"/> to inherit your own splash state from.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="GameState{T}" path="/typeparam[@name='T']"/></typeparam>
    [Serializable]
    public abstract class SplashGameState<T> : GameState<T>, ISplashState where T : GameStateOverride {
        #region Global Members
        // Persist between loading (as the game first scene is loaded during its execution).
        public override bool IsPersistent {
            get { return true; }
        }

        // -----------------------

        /// <inheritdoc cref="SplashGameState"/>
        public SplashGameState() : base(false) { }
        #endregion

        #region State Override
        public override void OnStateOverride(T _state) {
            base.OnStateOverride(_state);

            // Freeze game to not trigger the main menu behaviour once loaded.
            _state.FreezeChronos = true;
        }
        #endregion
    }

    /// <summary>
    /// Default <see cref="SplashManager"/>-related <see cref="GameState"/>, applied while in the splash scene.
    /// </summary>
    [Serializable, DisplayName("Splash/Splash [Default]")]
    public class DefaultSplashGameState : SplashGameState<GameStateOverride> {
        #region Global Members
        /// <summary>
        /// High priority to remain above gameplay states, but lower than loadings
        /// (which need to be active to execute).
        /// </summary>
        public const int PriorityConst = 900;

        public override int Priority {
            get { return PriorityConst; }
        }
        #endregion
    }
}
