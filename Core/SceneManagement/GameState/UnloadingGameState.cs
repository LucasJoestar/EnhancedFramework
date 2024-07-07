// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// Base interface for every unloading <see cref="GameState"/>.
    /// <br/> Used within a <see cref="SerializedType{T}"/> instead of the generic <see cref="UnloadingGameState{T}"/> class.
    /// </summary>
    internal interface IUnloadingState { }

    /// <summary>
    /// Base <see cref="GameState{T}"/> to inherit your own unloading state from.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="GameState{T}" path="/typeparam[@name='T']"/></typeparam>
    [Serializable]
    public abstract class UnloadingGameState<T> : GameState<T>, IUnloadingState where T : GameStateOverride {
        #region Global Members
        // Keep this state between loadings.
        public override bool IsPersistent {
            get { return true; }
        }

        // Manage unloading operations from this state callbacks.
        public override IGameStateLifetimeCallback LifetimeCallback {
            get { return EnhancedSceneManager.Instance; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="UnloadingGameState{T}"/>
        public UnloadingGameState() : base(false) { }
        #endregion

        #region State Override
        public override void OnStateOverride(T _state) {
            base.OnStateOverride(_state);

            _state.IsUnloading = true;
        }
        #endregion
    }

    /// <summary>
    /// Default <see cref="GameState"/> applied when the game is performing a <see cref="SceneBundle"/> unloading operation.
    /// </summary>
    [Serializable, DisplayName("Loading/Unloading [Default]")]
    public sealed class DefaultUnloadingGameState : UnloadingGameState<GameStateOverride> {
        #region Global Members
        /// <summary>
        /// Doesn't need to use a high priority, as the operations are called as soon as the state is created.
        /// </summary>
        public const int PriorityConst = 0;

        public override int Priority {
            get { return PriorityConst; }
        }
        #endregion
    }
}
