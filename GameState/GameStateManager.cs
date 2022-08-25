// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.GameStates {
    /// <summary>
    /// Implement this interface to receive a callback
    /// when the <see cref="GameStateManager.StateOverride"/> values are overridden by a new state.
    /// <para/>
    /// Requires to be registered first using <see cref="GameStateManager.RegisterOverrideCallback(IGameStateOverrideCallback)"/>.
    /// </summary>
    public interface IGameStateOverrideCallback {
        #region State Override
        /// <summary>
        /// Callback for when the <see cref="GameStateOverride"/> values are changed.
        /// </summary>
        /// <param name="_state">New state override values.</param>
        void OnGameStateOverride(in GameStateOverride _state);
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
    /// Global <see cref="GameState"/> manager instance,
    /// used to dynamically push and pop states on the stack of the game.
    /// </summary>
    public class GameStateManager : EnhancedSingleton<GameStateManager>, IEarlyUpdate {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Game State Manager")]

        [SerializeField] private SerializedType<GameStateOverride> overrideType = new SerializedType<GameStateOverride>(typeof(GameStateOverride), true);

        /// <summary>
        /// Current global states override shared values.
        /// </summary>
        [field: SerializeField, Enhanced, ReadOnly]
        public GameStateOverride StateOverride { get; private set; } = null;

        /// <summary>
        /// The game state being currently active and enabled.
        /// </summary>
        [field: SerializeReference, Space(15f)]
        public GameState CurrentState { get; private set; } = new DefaultState();

        [SerializeField, Enhanced, ReadOnly] private Stamp<GameState> states = new Stamp<GameState>() { };

        private Stamp<IGameStateOverrideCallback> overrideCallbacks = new Stamp<IGameStateOverrideCallback>();
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            StateOverride = Activator.CreateInstance(overrideType) as GameStateOverride;

            // Push initial default state.
            PushState(new DefaultState());
        }
        #endregion

        #region Override Callbacks
        /// <summary>
        /// Registers an object to receive callback on state overrides.
        /// </summary>
        /// <param name="_callback">Callback receiver to register.</param>
        public void RegisterOverrideCallback(IGameStateOverrideCallback _callback) {
            overrideCallbacks.Add(_callback);
        }

        /// <summary>
        /// Unregisters an object from receiving callback on state overrides.
        /// </summary>
        /// <param name="_callback">Callback receiver to unregister.</param>
        public void UnregisterOverrideCallback(IGameStateOverrideCallback _callback) {
            overrideCallbacks.Remove(_callback);
        }
        #endregion

        #region State Management
        /// <summary>
        /// Pushes a new state on the game state stack.
        /// </summary>
        /// <param name="_state">New state to add to the stack.</param>
        public void PushState(GameState _state) {
            states.Add(_state, true);

            this.Log($"GameState => Push '{_state}'");

            _state.OnCreated();
            RefreshCurrentState();
        }

        /// <summary>
        /// Pops an already push-in stack buffer.
        /// </summary>
        /// <param name="_state">Existing stack to remove.</param>
        public void PopState(GameState _state) {
            states.Remove(_state);

            this.Log($"GameState => Pop '{_state}'");

            RefreshCurrentState();
            _state.OnDestroyed();
        }

        // -----------------------

        private void RefreshCurrentState() {
            // State override.
            GameStateOverride _override = StateOverride;

            for (int i = 0; i < states.Count; i++) {
                states[i].OnStateOverride(_override);
            }

            foreach (IGameStateOverrideCallback _callback in overrideCallbacks) {
                _callback.OnGameStateOverride(_override);
            }

            // Update current state.
            GameState _current = states.Last();

            if (_current != CurrentState) {
                CurrentState.OnDisabled();
                _current.OnEnabled();

                CurrentState = _current;

                this.Log($"GameState => Set new state '{_current}'");
            }
        }
        #endregion

        #region Update
        void IEarlyUpdate.Update() {
            CurrentState.OnUpdate();
        }
        #endregion
    }
}
