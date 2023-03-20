// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.Settings;
using System;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// This application global game manager.
    /// <para/> Inherit from this to implement your own spin in it.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-98)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Manager/Game Manager"), DisallowMultipleComponent]
    public class GameManager : EnhancedSingleton<GameManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Game Manager")]

        [SerializeField, Enhanced, Required] protected GameSettings settings = null;

        // -----------------------

        /// <summary>
        /// Getter for the current player instance <see cref="Transform"/>.
        /// <para/>
        /// You need to set it at runtime in order to be properly able to use it.
        /// </summary>
        public static Func<Transform> PlayerGetter = null;

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

        #region Utility
        /// <summary>
        /// Get the current player instance.
        /// <br/> Uses the <see cref="PlayerGetter"/> delegate.
        /// </summary>
        /// <param name="_player">Player instance transform (null if none).</param>
        /// <returns>True if a player could be successfully found, false otherwise.</returns>
        public static bool GetPlayer(out Transform _player) {

            if (PlayerGetter == null) {
                _player = null;
                return false;
            }

            _player = PlayerGetter();
            return _player.IsValid();
        }

        /// <typeparam name="T">Player component type to get.</typeparam>
        /// <param name="_player">Player instance component (null if none).</param>
        /// <inheritdoc cref="GetPlayer(out Transform)"/>
        public static bool GetPlayer<T>(out T _player) where T : Component {
            if (GetPlayer(out Transform _transform)) {
                return _transform.TryGetComponent(out _player);
            }

            _player = null;
            return false;
        }
        #endregion
    }
}
