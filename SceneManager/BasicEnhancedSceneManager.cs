// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.GameStates;

namespace EnhancedFramework.SceneManagement {
    /// <summary>
    /// Basic <see cref="EnhancedSceneManager{T, U}"/> using <see cref="LoadingState"/>.
    /// <para/>
    /// Can be used if no further adjustement require to be made on the base scene manager behaviour.
    /// </summary>
    public class BasicEnhancedSceneManager : EnhancedSceneManager<BasicEnhancedSceneManager, LoadingState> { }
}
