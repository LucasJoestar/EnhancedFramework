// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Base class for any Enhanced input <see cref="ScriptableObject"/> asset.
    /// </summary>
    public abstract class InputEnhancedAsset : ScriptableObject {
        #region Global Members
        public const string MenuPath    = FrameworkUtility.MenuPath + "Input/";
        public const int MenuOrder      = FrameworkUtility.MenuOrder - 50;

        // -----------------------
        
        [Section("Input Asset")]

        [SerializeField] private bool enableOnInit = false;

        // -----------------------

        /// <summary>
        /// Is this input asset currently enabled?
        /// <br/> Always return true with the native input system.
        /// </summary>
        public abstract bool IsEnabled { get; }
        #endregion

        #region Initialization
        /// <summary>
        /// Called from the <see cref="InputManager"/> to initialize this asset.
        /// </summary>
        internal void Initialize(InputDatabase _database) {
            bool _isEnabled = IsEnabled;
            OnInit(_database);

            if (_isEnabled || enableOnInit) {
                Enable();
            }
        }

        /// <summary>
        /// Initializes this input asset using a specific <see cref="InputDatabase"/>.
        /// </summary>
        /// <param name="_database"><see cref="InputDatabase"/> to initialize this asset with.</param>
        protected abstract void OnInit(InputDatabase _database);
        #endregion

        #region Utility
        /// <summary>
        /// Enables this input asset.
        /// <br/> Has no effect with the native input system.
        /// </summary>
        public abstract void Enable();

        /// <summary>
        /// Disables this input asset.
        /// <br/> Has no effect with the native input system.
        /// </summary>
        public abstract void Disable();
        #endregion
    }
}
