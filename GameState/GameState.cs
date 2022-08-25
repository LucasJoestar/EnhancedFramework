// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;

namespace EnhancedFramework.GameStates {
    /// <summary>
    /// Base class to inherit your own game states from.
    /// <para/>
    /// Note that inheriting from this class will only give you access to the base <see cref="GameStateOverride"/> value.
    /// <br/> For getting access to a specific override type, please use <see cref="GameState{T}"/> instead.
    /// </summary>
    [Serializable]
    public abstract class GameState : IComparable<GameState> {
        #region Global Members
        /// <summary>
        /// Priority of this state. The state with the higher priority in the list is the one enabled.
        /// <br/> Note that the priority value must (absolutely) be unique for each different class type.
        /// </summary>
        public abstract int Priority { get; }
        #endregion

        #region Creation
        /// <summary>
        /// Creates and push a new game state on the stack.
        /// </summary>
        /// <typeparam name="T">The type of game state to create.</typeparam>
        /// <returns>The newly created state.</returns>
        public static T CreateState<T>() where T : GameState {
            T _state = Activator.CreateInstance<T>();
            GameStateManager.Instance.PushState(_state);

            return _state;
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
        /// <summary>
        /// Called when this state is being created.
        /// </summary>
        protected internal virtual void OnCreated() { }

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
        /// Called when this state is being destroyed.
        /// </summary>
        protected internal virtual void OnDestroyed() { }
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
        #region State Override
        public override sealed void OnStateOverride(GameStateOverride _state) {
            base.OnStateOverride(_state);

            OnStateOverride(_state as T);
        }

        /// <inheritdoc cref="OnStateOverride(GameStateOverride)"/>
        public virtual void OnStateOverride(T _state) { }
        #endregion
    }
}
