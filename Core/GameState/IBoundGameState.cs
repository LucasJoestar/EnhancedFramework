// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// Inherit your own <see cref="GameState"/> from this interface to mark it as "bounded".
    /// <para/>
    /// A bounded <see cref="GameState"/> should be attached for its lifetime to an object instance,
    /// <br/> making it a direct submodule for its associated duration.
    /// <para/>
    /// For instance, a "CinematicGameState" can be a good candidate for a bounded GameState,
    /// <br/> as it will probably only need to be bounded to a given cutscene for its lifetime.
    /// </summary>
    public interface IBoundGameState {
        #region Content
        /// <summary>
        /// Bounds this GameState to a given <see cref="EnhancedBehaviour"/> instance.
        /// </summary>
        /// <param name="_behaviour">The bounded <see cref="EnhancedBehaviour"/> instance.</param>
        void Bound(EnhancedBehaviour _behaviour);
        #endregion
    }

    /// <typeparam name="T">This <see cref="GameState"/> bounded object instance type.</typeparam>
    /// <inheritdoc cref="IBoundGameState"/>
    public interface IBoundGameState<T> {
        #region Content
        /// <summary>
        /// Bounds this GameState to a given object instance.
        /// </summary>
        /// <param name="_object">The bounded object instance.</param>
        void Bound(T _object);
        #endregion
    }
}
