// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Settings;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// This application global game manager.
    /// <para/> Inherit from this to implement your own spin in it.
    /// </summary>
    public class GameManager : EnhancedSingleton<GameManager> {
        #region Global Members
        [Section("Game Manager")]

        /// <summary>
        /// This game ruling settings.
        /// <br/> Can be used to access various specific settings from there.
        /// </summary>
        [SerializeField, Enhanced, Required] protected GameSettings settings = null;

        // -----------------------

        /// <summary>
        /// Is the application currently being shut down?
        /// </summary>
        public static bool IsQuittingApplication { get; protected set; } = false;
        #endregion

        #region Enhanced Behaviour
        protected override void Awake() {
            base.Awake();

            // Settings initialization.
            settings.Init();
        }

        protected virtual void OnApplicationQuit() {
            // Register that the application is currently being closed.
            // Using script execution order, this information can then be used from any other class.
            IsQuittingApplication = true;
        }
        #endregion
    }
}
