// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using UnityEngine;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// Game global state shared values, overridden by the current states in the stack.
    /// </summary>
    [Serializable]
    public abstract class GameStateOverride {
        #region Global Members
        /// <summary>
        /// Determines whether the hardware cursor is currently visible or not.
        /// </summary>
        public bool IsCursorVisible = true;

        /// <summary>
        /// Determines the behaviour of the hardware cursor.
        /// </summary>
        public CursorLockMode CursorLockMode = CursorLockMode.None;

        /// <summary>
        /// Is the game currently performing a loading operation?
        /// </summary>
        [Space(5f)] public bool IsLoading = false;

        /// <summary>
        /// Is the game currently performing an unloading operation?
        /// </summary>
        public bool IsUnloading = false;

        /// <summary>
        /// Is the game currently in a pause state?
        /// </summary>
        [Space(5f)] public bool IsPaused = false;

        /// <summary>
        /// Is the game application currently being quit?
        /// </summary>
        [Space(5f)] public bool IsQuitting = false;
        #endregion

        #region Behaviour        
        /// <summary>
        /// Resets the values back to default.
        /// </summary>
        public virtual GameStateOverride Reset() {
            IsCursorVisible = true;
            CursorLockMode = CursorLockMode.None;

            IsLoading = false;
            IsUnloading = false;

            IsPaused = false;
            IsQuitting = false;

            return this;
        }

        /// <summary>
        /// Applies the modifications made to this <see cref="GameStateOverride"/>.
        /// </summary>
        internal protected void Apply() {
            // Cursor overrides.
            Cursor.visible = IsCursorVisible;
            Cursor.lockState = CursorLockMode;
        }
        #endregion
    }
}
