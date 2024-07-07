// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// Base interface for every loading <see cref="GameState"/>.
    /// <br/> Used within a <see cref="SerializedType{T}"/> instead of the generic <see cref="LoadingGameState{T}"/> class.
    /// </summary>
    internal interface ILoadingState { }

    /// <summary>
    /// Base <see cref="GameState{T}"/> to inherit your own loading state from.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="GameState{T}" path="/typeparam[@name='T']"/></typeparam>
    [Serializable]
    public abstract class LoadingGameState<T> : GameState<T>, ILoadingState where T : GameStateOverride {
        #region Global Members
        // Keep this state between loadings.
        public override bool IsPersistent {
            get { return true; }
        }

        // Manage loading operations from this state callbacks.
        public override IGameStateLifetimeCallback LifetimeCallback {
            get { return EnhancedSceneManager.Instance; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="LoadingGameState{T}"/>
        public LoadingGameState() : base(false) { }
        #endregion

        #region State Override
        public override void OnStateOverride(T _state) {
            base.OnStateOverride(_state);

            _state.IsLoading = true;
        }
        #endregion
    }

    /// <summary>
    /// Default <see cref="GameState"/> applied when the game is performing a <see cref="SceneBundle"/> loading operation.
    /// </summary>
    [Serializable, DisplayName("Loading/Loading [Default]")]
    public sealed class DefaultLoadingGameState : LoadingGameState<GameStateOverride> {
        #region Global Members
        /// <summary>
        /// Uses a high priority to make sure the state is the active one.
        /// </summary>
        public const int PriorityConst = 999;

        public override int Priority {
            get { return PriorityConst; }
        }
        #endregion
    }
}
