// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using UnityEngine;

namespace EnhancedFramework.GameStates {
    /// <summary>
    /// Base class to inherit your own game states from.
    /// <para/>
    /// Note that inheriting from this class will only give you access to the base <see cref="GameStateOverride"/> value.
    /// <br/> For getting access to a specific override type, please use <see cref="GameState{T}"/> instead.
    /// <para/>
    /// Newly created <see cref="GameState"/> are automatically pushed on the stack.
    /// </summary>
    [Serializable]
    public abstract class GameState : IComparable<GameState> {
        #region Global Members
        /// <summary>
        /// Priority of this state. The state with the higher priority in the list is the one enabled.
        /// <br/> Note that the priority value must (absolutely) be unique for each different class type.
        /// </summary>
        public abstract int Priority { get; }

        /// <summary>
        /// Indicates if this <see cref="GameState"/> is currently on the stack or not.
        /// </summary>
        public bool IsOnStack { get; private set; } = false;

        // -----------------------

        /// <inheritdoc cref="GameState(bool)"/>
        public GameState() : this(true) { }

        /// <summary>
        /// Creates a new state and automatically push it on the stack.
        /// </summary>
        /// <param name="_pushOnNextFrame">Should the state be pushed on the stack during this frame or the next one?</param>
        protected GameState(bool _pushOnNextFrame) {
            #if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
            #endif

            if (_pushOnNextFrame) {
                GameStateManager.Instance.PushStateOnNextFrame(this);
            } else {
                GameStateManager.Instance.PushState(this);
            }
        }
        #endregion

        #region Creation / Destruction
        /// <summary>
        /// Creates a new game state, that is automatically added on the stack.
        /// </summary>
        /// <typeparam name="T">The type of game state to create.</typeparam>
        /// <returns>The newly created state.</returns>
        public static T CreateState<T>() where T : GameState, new() {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Removes the first state of a specific type found on the stack.
        /// </summary>
        /// <typeparam name="T">The type of game state to remove.</typeparam>
        /// <returns>True if a game state of that type was found and removed, false otherwise.</returns>
        public static bool RemoveState<T>() where T : GameState {
            foreach (GameState _state in GameStateManager.Instance.states) {
                if (_state is T) {
                    _state.RemoveState();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes this state from the game stack.
        /// </summary>
        public void RemoveState() {
            GameStateManager.Instance.PopState(this);
        }
        #endregion

        #region State Override
        /// <summary>
        /// Implement this to override and modify shared various values on the game global state.
        /// </summary>
        /// <param name="_state">Global game state shared values to override.</param>
        public virtual void OnStateOverride(GameStateOverride _state) { }
        #endregion

        #region Behaviour
        internal void OnPushedOnStack() {
            IsOnStack = true;
            OnInit();
        }

        internal void OnRemovedFromStack() {
            IsOnStack = false;
            OnTerminate();
        }

        // -----------------------

        /// <summary>
        /// Called when this state has been pushed on stack.
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// Called when this state is being enabled and set as the currently active one.
        /// </summary>
        protected internal virtual void OnEnabled() { }

        /// <summary>
        /// Called each and every frame during update while this state is active.
        /// </summary>
        protected internal virtual void OnUpdate() { }

        /// <summary>
        /// Called when this state is being disabled.
        /// </summary>
        protected internal virtual void OnDisabled() { }

        /// <summary>
        /// Called when this state has been removed from the stack.
        /// </summary>
        protected virtual void OnTerminate() { }
        #endregion

        #region Comparer
        int IComparable<GameState>.CompareTo(GameState _other) {
            return Priority.CompareTo(_other.Priority);
        }
        #endregion
    }

    /// <summary>
    /// Base class to inherit your own game states from, associated with a specific override type.
    /// <para/>
    /// Inherit from this class instead of <see cref="GameState"/> to get access to a specific override type.
    /// </summary>
    /// <typeparam name="T">Override type to be associated with this state.</typeparam>
    [Serializable]
    public abstract class GameState<T> : GameState where T : GameStateOverride {
        #region Constructors
        /// <inheritdoc cref="GameState(bool)"/>
        public GameState() : base() { }

        /// <inheritdoc cref="GameState(bool)"/>
        protected GameState(bool _addOnStack) : base(_addOnStack) { }
        #endregion

        #region State Override
        public override sealed void OnStateOverride(GameStateOverride _state) {
            base.OnStateOverride(_state);

            OnStateOverride(_state as T);
        }

        /// <inheritdoc cref="OnStateOverride(GameStateOverride)"/>
        public virtual void OnStateOverride(T _state) { }
        #endregion
    }

    /// <summary>
    /// Contains <see cref="GameState"/>-related extension utility method(s).
    /// </summary>
    public static class GameStateExtensions {
        #region Content
        /// <summary>
        /// Get if this state is currently active (not null and on the stack) or not.
        /// </summary>
        /// <param name="_state">The <see cref="GameState"/> to check.</param>
        /// <returns>True if this state is not null and currently on the stack, false otherwise.</returns>
        public static bool IsActive(this GameState _state) {
            return (_state != null) && _state.IsOnStack;
        }
        #endregion
    }
}
