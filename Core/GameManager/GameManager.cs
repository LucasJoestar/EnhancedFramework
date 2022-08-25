// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Settings;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EnhancedFramework.GameStates")]
namespace EnhancedFramework.Core {
    /// <summary>
    /// This application global game manager.
    /// <para/> Inherit from this to implement your own spin in it.
    /// </summary>
    public class GameManager : EnhancedSingleton<GameManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Game Manager")]

        [SerializeField, Enhanced, Required] protected GameSettings settings = null;

        // -----------------------

        /// <summary>
        /// Indicates whether the application is currently being quit or not.
        /// </summary>
        public static bool IsQuittingApplication { get; internal set; } = false;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Settings initialization.
            settings.Init();
        }
        #endregion
    }
}
