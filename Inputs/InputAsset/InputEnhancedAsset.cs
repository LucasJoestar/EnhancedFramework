// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Base class for any Enhanced input <see cref="ScriptableObject"/> asset.
    /// </summary>
    public abstract class InputEnhancedAsset : ScriptableObject {
        #region Global Members
        /// <summary>
        /// Is this input asset currently enabled?
        /// </summary>
        public abstract bool IsEnabled { get; }
        #endregion

        #region Utility
        /// <summary>
        /// Enables this input asset.
        /// </summary>
        public abstract void Enable();

        /// <summary>
        /// Disables this input asset.
        /// </summary>
        public abstract void Disable();
        #endregion
    }
}
