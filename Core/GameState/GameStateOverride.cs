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
        /// Is the game currently performing a loading operation?
        /// </summary>
        public bool IsLoading = false;

        /// <summary>
        /// Is the game currently performing an unloading operation?
        /// </summary>
        public bool IsUnloading = false;

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
            IsQuitting = false;

            IsLoading = false;
            IsUnloading = false;

            return this;
        }
        #endregion
    }
}
