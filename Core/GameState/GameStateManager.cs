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
    /// when the <see cref="GameStateManager.stateOverride"/> values are overridden by a new state.
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
    /// Default game global state shared values.
    /// </summary>
    [Serializable, DisplayName("<Default>")]
    public class DefaultGameStateOverride : GameStateOverride {
        #region Global Members
        /// <summary>
        /// Whether the player has control of their character or not.
        /// </summary>
        [Space(5f)] public bool HasControl = true;

        /// <summary>
        /// Whether the game can be paused or not.
        /// </summary>
        [Space(5f)] public bool CanPause = true;

        /// <summary>
        /// Is the game currently paused?
        /// </summary>
        public bool IsPaused = false;

        /// <summary>
        /// Whether the black bars on top and bottom of the screen should be visible or not.
        /// </summary>
        [Space(5f)] public bool ShowBlackBars = false;
        #endregion

        #region Behaviour
        /// <summary>
        /// Resets the values back to default.
        /// </summary>
        public override GameStateOverride Reset() {
            base.Reset();

            HasControl = true;

            CanPause = false;
            IsPaused = false;

            ShowBlackBars = false;

            return this;
        }
        #endregion
    }

    /// <summary>
    /// Global <see cref="GameState"/> manager instance,
    /// used to dynamically push and pop states on the stack of the game.
    /// </summary>
    public class GameStateManager : EnhancedSingleton<GameStateManager>, IPermanentUpdate {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init | UpdateRegistration.Permanent;

        #region Global Members
        [Section("Game State Manager")]

        [SerializeField, Enhanced, DisplayName("Default State")]
        private SerializedType<GameState> defaultStateType = new SerializedType<GameState>(typeof(DefaultGameState));

        [SerializeField, Enhanced, ReadOnly]
        private PolymorphValue<GameStateOverride> stateOverride = new PolymorphValue<GameStateOverride>(SerializedTypeConstraint.None,
                                                                                                        typeof(DefaultGameStateOverride));

        /// <summary>
        /// The game state being currently active and enabled.
        /// </summary>
        [field: SerializeReference, Space(10f)]
        public GameState CurrentState { get; private set; } = null;

        [SerializeField, Enhanced, ReadOnly] private BufferR<Reference<GameState>> states = new BufferR<Reference<GameState>>();

        // -----------------------

        /// <summary>
        /// Current global states override shared values.
        /// </summary>
        public GameStateOverride StateOverride {
            get { return stateOverride; }
        }

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

        void IPermanentUpdate.Update() {
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
        #endregion

        #region Override Callbacks
        /// <summary>
        /// Registers an object to receive callback on state overrides.
        /// </summary>
        /// <param name="_callback">Callback receiver to register.</param>
        public void RegisterOverrideCallback(IGameStateOverrideCallback _callback) {
            overrideCallbacks.Add(_callback);
            _callback.OnGameStateOverride(stateOverride);
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
        private readonly int chronosID = EnhancedUtility.GenerateGUID();

        // -----------------------

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
                this.LogErrorMessage($"The type \'{_stateType.Name}\' does not inherit from \'{typeof(GameState).Name}\'");
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

            // Prevent from having multiple states of the same type on the stack if it is not allowed.
            if (!_state.MultipleInstance && IsOnStack(_state.GetType(), out _)) {
                this.LogWarningMessage($"A GameState of type \"{_state.GetType()}\" is already pushed on the stack");
                return;
            }

            states.Push(_state, _state.Priority);

            this.LogMessage($"Push \"{_state}\"");

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
            this.LogMessage($"Pop \"{_state}\"");

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

                this.LogMessage($"Set new state \"{_current}\"");

                if (_previous.IsActive()) {
                    _previous.OnDisabled();
                }

                _current.OnEnabled();
            }

            // State & chronos overrides.
            GameStateOverride _override = stateOverride.Value.Reset();
            float _chronos = 1f;
            int _priority = -1;

            for (int i = 0; i < states.Count; i++) {
                GameState _state = states.GetKeyAt(i);
                _state.OnGameStateOverride(_override);

                if (_state.OverrideChronos(out float _chronosTemp, out int _priorityTemp) && (_priorityTemp >= _priority)) {
                    _chronos = _chronosTemp;
                    _priority = _priorityTemp;
                }
            }

            ChronosManager.Instance.ApplyOverride(chronosID, _chronos, _priority);
            _override.Apply();

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

        /// <typeparam name="T"><inheritdoc cref="IsActive(Type, out GameState, bool)" path="/param[@name='_type']"/></typeparam>
        /// <inheritdoc cref="IsActive(Type, out GameState, bool)"/>
        public bool IsActive<T>(out GameState _state, bool _inherit = true) where T : GameState {
            Type _type = typeof(T);
            return IsActive(_type, out _state, _inherit);
        }

        /// <summary>
        /// Get if any <see cref="GameState"/> of a specific type is a currently active.
        /// </summary>
        /// <param name="_type">The <see cref="GameState"/> type to check.</param>
        /// <param name="_state">The first found active <see cref="GameState"/> of the given type (null if none).</param>
        /// <param name="_inherit">If true, will also check for any type inheriting from the given one.</param>
        /// <returns>True if any <see cref="GameState"/> of the given type is active, false otherwise.</returns>
        public bool IsActive(Type _type, out GameState _state, bool _inherit = true) {
            foreach (var _pair in states) {
                _state = _pair.First.Value;
                if (IsType(_state)) {
                    return true;
                }
            }

            foreach (var _gameState in pushPendingStates) {
                if (IsType(_gameState)) {
                    _state = _gameState;
                    return true;
                }
            }

            _state = null;
            return false;

            // ----- Local Method ----- \\

            bool IsType(GameState _state) {
                Type _stateType = _state.GetType();

                if ((_stateType == _type) || (_inherit && _stateType.IsSubclassOf(_type))) {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get if any <see cref="GameState"/> of a specific type is currently on the stack.
        /// </summary>
        /// <param name="_type">The <see cref="GameState"/> type to check.</param>
        /// <param name="_state">The first found <see cref="GameState"/> on the stack of the given type (null if none).</param>
        /// <returns>True if any <see cref="GameState"/> of the given type is currently on the stack, false otherwise.</returns>
        public bool IsOnStack(Type _type, out GameState _state) {
            foreach (var _pair in states) {
                _state = _pair.First.Value;

                if (_state.GetType() == _type) {
                    return true;
                }
            }

            _state = null;
            return false;
        }
        #endregion

        #region Logger
        public override Color GetLogMessageColor(LogType _type) {
            return SuperColor.Lime.Get();
        }
        #endregion
    }
}
