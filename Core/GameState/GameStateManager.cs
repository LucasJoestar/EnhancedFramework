// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedFramework.Core.GameStates {
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
        [Space(5f)] public bool CanPause = true;

        /// <summary>
        /// Is the game currently paused?
        /// </summary>
        public bool IsPaused = false;

        /// <summary>
        /// Is the game currently performing a loading operation?
        /// </summary>
        [Space(5f)] public bool IsLoading = false;

        /// <summary>
        /// Is the game currently performing an unloading operation?
        /// </summary>
        public bool IsUnloading = false;

        /// <summary>
        /// Is the game application currently being quit?
        /// </summary>
        [Space(5f)] public bool IsQuitting = false;
        #endregion

        #region Behaviour
        /// <summary>
        /// Resets the values back to default.
        /// </summary>
        public virtual GameStateOverride Reset() {
            HasControl = true;

            CanPause = false;
            IsPaused = false;

            IsLoading = false;
            IsUnloading = false;

            IsQuitting = false;

            return this;
        }
        #endregion
    }

    /// <summary>
    /// Global <see cref="GameState"/> manager instance,
    /// used to dynamically push and pop states on the stack of the game.
    /// </summary>
    public class GameStateManager : EnhancedSingleton<GameStateManager>, IEarlyUpdate {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init | UpdateRegistration.Early;

        #region Global Members
        [Section("Game State Manager")]

        [SerializeField, Enhanced, DisplayName("State Override")]
        private SerializedType<GameStateOverride> overrideType = new SerializedType<GameStateOverride>(typeof(GameStateOverride), SerializedTypeConstraint.BaseType);

        [SerializeField, Enhanced, DisplayName("Default State")]
        private SerializedType<GameState> defaultStateType = new SerializedType<GameState>(typeof(DefaultGameState));

        /// <summary>
        /// Current global states override shared values.
        /// </summary>
        [field: SerializeReference, Space(10f), Enhanced, ReadOnly]
        public GameStateOverride StateOverride { get; private set; } = new GameStateOverride();

        /// <summary>
        /// The game state being currently active and enabled.
        /// </summary>
        [field: SerializeReference, Space(10f)]
        public GameState CurrentState { get; private set; } = null;

        [SerializeField, Enhanced, ReadOnly, Block] private BufferR<Reference<GameState>> states = new BufferR<Reference<GameState>>();

        // -----------------------

        /// <summary>
        /// The total amount of <see cref="GameState"/> currently pushed in the stack.
        /// </summary>
        public int GameStateCount {
            get { return states.Count; }
        }

        private readonly List<IGameStateOverrideCallback> overrideCallbacks = new List<IGameStateOverrideCallback>();

        private readonly List<GameState> pushPendingStates = new List<GameState>();
        private readonly List<GameState> popPendingStates = new List<GameState>();
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Push initial default state.
            GameState.CreateState(defaultStateType);
        }

        void IEarlyUpdate.Update() {
            // Push and pop pending states.
            bool _refresh = false;

            ManagePendingStates(pushPendingStates, PushState);
            ManagePendingStates(popPendingStates, PopState);

            if (_refresh) {
                RefreshCurrentState();
            }

            // Current state update.
            CurrentState.OnUpdate();

            // ----- Local Method ----- \\

            void ManagePendingStates(List<GameState> _states, Action<GameState, bool, bool> _action) {
                if (_states.Count != 0) {
                    foreach (GameState _state in _states) {
                        _action(_state, false, false);
                    }

                    _states.Clear();
                    _refresh = true;
                }
            }
        }

        #if UNITY_EDITOR
        private void OnValidate() {
            if (StateOverride.GetType() != overrideType) {
                StateOverride = Activator.CreateInstance(overrideType) as GameStateOverride;
            }
        }
        #endif
        #endregion

        #region Override Callbacks
        /// <summary>
        /// Registers an object to receive callback on state overrides.
        /// </summary>
        /// <param name="_callback">Callback receiver to register.</param>
        public void RegisterOverrideCallback(IGameStateOverrideCallback _callback) {
            overrideCallbacks.Add(_callback);
            _callback.OnGameStateOverride(StateOverride);
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
        /// Tells to push new state on the game stack on the next frame.
        /// </summary>
        /// <param name="_state">The new state to add to the stack on the next frame.</param>
        public void PushStateOnNextFrame(GameState _state) {
            pushPendingStates.Add(_state);
            _state.OnPushedOnPending();
        }

        /// <summary>
        /// Tells to pop a specific state from the game stack on the next frame.
        /// </summary>
        /// <param name="_state">The state to remove from the stack on the next frame.</param>
        public void PopStateOnNextFrame(GameState _state) {
            popPendingStates.Add(_state);
        }

        /// <summary>
        /// Pushes a new state on the game state stack.
        /// </summary>
        /// <param name="_state">New state to add to the stack.</param>
        public void PushState(GameState _state) {
            PushState(_state, true);
        }

        /// <summary>
        /// Pops an already push-in stack buffer.
        /// </summary>
        /// <param name="_state">Existing stack to remove.</param>
        public void PopState(GameState _state) {
            PopState(_state, true);
        }

        /// <summary>
        /// Pops the first state on the stack of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of game state to pop from the stack.</typeparam>
        /// <returns>True if a state of this type could be found and was removed, false otherwise.</returns>
        public bool PopState<T>() where T : GameState {
            for (int i = 0; i < states.Count; i++) {
                if (states.GetKeyAt(i) is T) {
                    PopStateAt(i, true);
                    return true;
                }
            }

            return false;
        }

        /// <param name="_stateType"><inheritdoc cref="PopState{T}" path="/typeparam[@name='T']"/></param>
        /// <inheritdoc cref="PopState{T}"/>
        public bool PopState(Type _stateType) {
            if (!_stateType.IsSubclassOf(typeof(GameState))) {
                Debug.LogError($"GameState - The type \'{_stateType.Name}\' does not inherit from \'{typeof(GameState).Name}\'");
                return false;
            }

            for (int i = 0; i < states.Count; i++) {
                if (states.GetKeyAt(i).GetType() == _stateType) {
                    PopStateAt(i, true);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Pops and removes all non-persistent <see cref="GameState"/> from the stack.
        /// </summary>
        public void PopNonPersistentStates() {
            for (int i = states.Count; i-- > 0;) {
                GameState _state = states.GetKeyAt(i);

                if (!_state.IsPersistent) {
                    PopState(_state, false);
                }
            }

            RefreshCurrentState();
        }

        // -----------------------

        private void PushState(GameState _state, bool _autoRefresh, bool _removeFromPending = true) {
            if (_removeFromPending) {
                RemoveFromPendingState(_state);
            }

            states.Push(_state, _state.Priority);

            this.Log($"GameState => Push '{_state}'");

            _state.OnPushedOnStack();

            if (_autoRefresh) {
                RefreshCurrentState();
            }
        }

        private void PopState(GameState _state, bool _autoRefresh, bool _removeFromPending = true) {
            int _index = states.IndexOfKey(_state);
            if (_index != -1) {
                PopStateAt(_index, _autoRefresh, _removeFromPending);
            }
        }

        private void PopStateAt(int _index, bool _autoRefresh, bool _removeFromPending = true) {
            GameState _state = states.GetKeyAt(_index);
            if (_removeFromPending) {
                RemoveFromPendingState(_state);
            }

            states.RemoveAt(_index);
            this.Log($"GameState => Pop '{_state}'");

            if (_autoRefresh) {
                RefreshCurrentState();
            }

            _state.OnRemovedFromStack();
        }

        private bool RemoveFromPendingState(GameState _state) {
            if (ManagePendingState(pushPendingStates) || ManagePendingState(popPendingStates)) {
                return true;
            }

            return false;

            // ----- Local Method ----- \\

            bool ManagePendingState(List<GameState> _states) {
                int _index = _states.IndexOf(_state);

                if (_index != -1) {
                    _states.RemoveAt(_index);
                    return true;
                }

                return false;
            }
        }

        // -----------------------

        private void RefreshCurrentState() {
            // Current state update.
            GameState _current = states.Value;

            if (_current != CurrentState) {
                GameState _previous = CurrentState;
                CurrentState = _current;

                this.Log($"GameState => Set new state \'{_current}\'");

                if (_previous.IsActive()) {
                    _previous.OnDisabled();
                }

                _current.OnEnabled();
            }

            // State override.
            GameStateOverride _override = StateOverride.Reset();

            for (int i = 0; i < states.Count; i++) {
                states.GetKeyAt(i).Value.OnStateOverride(_override);
            }

            foreach (IGameStateOverrideCallback _callback in overrideCallbacks) {
                _callback.OnGameStateOverride(_override);
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get the <see cref="GameState"/> pushed on the stack at a specific index.
        /// <para/>
        /// Use <see cref="GameStateCount"/> to get the total amount of states currently pushed in the stack.
        /// </summary>
        /// <param name="_index">Index of the state to get.</param>
        /// <returns>The <see cref="GameState"/> at the given index from the stack.</returns>
        public GameState GetGameStateAt(int _index) {
            return states.GetKeyAt(_index);
        }
        #endregion
    }
}
