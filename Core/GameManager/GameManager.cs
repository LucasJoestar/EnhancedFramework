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
        /// <summary>
        /// This game ruling settings.
        /// <br/> Can be used to access various specific settings from there.
        /// </summary>
        [field: SerializeField, Section("Game Manager"), Enhanced, Required]
        public GameSettings Settings { get; protected set; } = null;

        // -----------------------

        /// <summary>
        /// Is the application currently being shut down?
        /// </summary>
        public static bool IsQuittingApplication { get; private set; } = false;
        #endregion

        #region Enhanced Behaviour
        protected override void Awake() {
            base.Awake();

            Settings.Initialize();
        }

        private void OnApplicationQuit() {
            IsQuittingApplication = true;
        }
        #endregion
    }
}
