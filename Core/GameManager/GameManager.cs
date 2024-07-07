// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.Settings;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

namespace EnhancedFramework.Core {
    /// <summary>
    /// This application global game manager.
    /// <para/> Inherit from this to implement your own spin in it.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-980)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "General/Game Manager"), DisallowMultipleComponent]
    public sealed class GameManager : EnhancedSingleton<GameManager> {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Game Manager")]

        [Tooltip("Settings of the game")]
        [SerializeField, Enhanced, Required] private GameSettings settings = null;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("This game random generation seed")]
        [SerializeField, Enhanced, ReadOnly] private int seed = 0;

        // -----------------------

        /// <summary>
        /// This game random generation seed.
        /// </summary>
        public int Seed {
            get { return seed; }
        }

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

            // Initialization.
            settings.Init();

            // Seed.
            int _seed = seed;
            if (_seed == 0) {
                _seed = EnhancedUtility.GenerateGUID();
            }

            SetSeed(_seed);
        }
        #endregion

        #region Scripting Symbols
        /// <inheritdoc cref="GameSettings.GetScriptingSymbols"/>
        public string[] GetScriptingSymbols() {
            return settings.GetScriptingSymbols();
        }

        /// <inheritdoc cref="GameSettings.IsScriptingSymbolDefined"/>
        public bool IsScriptingSymbolDefined(string _symbol) {
            return settings.IsScriptingSymbolDefined(_symbol);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Sets this game random generation seed.
        /// </summary>
        /// <param name="_seed">This game generation seed.</param>
        public void SetSeed(int _seed) {

            seed = _seed;
            Random.InitState(_seed);

            this.LogMessage($"Seed - {_seed}");
        }

        // -------------------------------------------
        // Player
        // -------------------------------------------

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

        #region Logger
        public override Color GetLogMessageColor(LogType _type) {
            return SuperColor.Raspberry.Get();
        }
        #endregion
    }
}
