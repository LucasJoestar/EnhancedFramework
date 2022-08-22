// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.GameState {
    /// <summary>
    /// Original game state, acting as the default one when no override is applied.
    /// </summary>
    internal class DefaultState : GameState {
        #region Global Members
        /// <summary>
        /// The default state, which is added on game start and never removed,
        /// <br/> uses the lower priority to make sure there is always an active state in the game, whatever might happen.
        /// </summary>
        public const int DefaultStatePriority = -1;

        public override int Priority => DefaultStatePriority;
        #endregion
    }

    /// <summary>
    /// Game global state shared values, overridden by the current states in the stack.
    /// </summary>
    [Serializable]
    public class GameStateOverride {
        #region Global Members
        /// <summary>
        /// Whether the player has control of their character or not.
        /// </summary>
        public bool HasControl = true;

        /// <summary>
        /// Whether the game can be paused or not.
        /// </summary>
        public bool CanPause = true;

        /// <summary>
        /// Is the game currently paused?
        /// </summary>
        public bool IsPaused = false;

        /// <summary>
        /// Is the game currently in a loding state?
        /// </summary>
        public bool IsLoading = false;

        /// <summary>
        /// Is the game application currently being quit?
        /// </summary>
        public bool IsQuitting = false;
        #endregion
    }

    /// <summary>
    /// All <see cref="GameState"/> manager instance,
    /// used to dynamically push and pop states to the game.
    /// </summary>
    public class GameStateManager : EnhancedSingleton<GameStateManager>, IEarlyUpdate {
        #region Global Members
        /// <summary>
        /// 
        /// </summary>
        [field:SerializeField, Section("Game State Manager"), Enhanced, ReadOnly]
        public GameState CurrentState { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        [field: SerializeField, Enhanced, ReadOnly]
        public GameStateOverride StateOverride { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        [SerializeField] private SerializedType<GameStateOverride> overrideType = new SerializedType<GameStateOverride>(typeof(GameStateOverride), true);

        [SerializeField, Enhanced, ReadOnly] private Buffer<GameState> states = new Buffer<GameState>() { };

        #endregion

        #region Enhanced Behaviour
        protected override void Awake() {
            base.Awake();

            StateOverride = Activator.CreateInstance(overrideType) as GameStateOverride;

            // Push initial default state.
            PushState(new DefaultState());
        }
        #endregion

        #region State Management
        public void PushState(GameState _state) {
            CurrentState = states.Push(_state, _state.Priority);
        }

        public void PopState(GameState _state) {
            CurrentState = states.Pop(_state.Priority);
        }
        #endregion

        #region Update
        void IEarlyUpdate.Update() {
            CurrentState.OnUpdate();
        }
        #endregion
    }
}
