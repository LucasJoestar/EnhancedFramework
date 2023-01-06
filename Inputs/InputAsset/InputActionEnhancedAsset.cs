// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using UnityEngine;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Base class to inherit all input action related <see cref="ScriptableObject"/> assets.
    /// </summary>
    public abstract class InputActionEnhancedAsset : InputEnhancedAsset {
        public new const int MenuOrder      = FrameworkUtility.MenuOrder;

        #region Event
        /// <summary>
        /// Called when this input starts performing.
        /// <br/> Never called when using the native input system.
        /// </summary>
        public event Action<InputActionEnhancedAsset> OnStarted     = null;

        /// <summary>
        /// Called when this input peform action has been canceled.
        /// <br/> Never called when using the native input system.
        /// </summary>
        public event Action<InputActionEnhancedAsset> OnCanceled    = null;

        /// <summary>
        /// Called when this input has been fully performed.
        /// <br/> Never called when using the native input system.
        /// </summary>
        public event Action<InputActionEnhancedAsset> OnPerformed   = null;

        // -----------------------

        protected void CallOnStarted() {
            OnStarted?.Invoke(this);
        }

        protected void CallOnCanceled() {
            OnCanceled?.Invoke(this);
        }

        protected void CallOnPerformed() {
            OnPerformed?.Invoke(this);
        }
        #endregion

        #region Input
        /// <summary>
        /// Get if this input was performed this frame.
        /// </summary>
        /// <returns>True if this input was performed this frame, false otherwise.</returns>
        public abstract bool Performed();

        /// <summary>
        /// Get if this input was pressed this frame.
        /// </summary>
        /// <returns>True if this input was pressed this frame, false otherwise.</returns>
        public abstract bool Pressed();

        /// <summary>
        /// Get if this input was being hold this frame.
        /// </summary>
        /// <returns>True if this input was being hold this frame, false otherwise.</returns>
        public abstract bool Holding();

        // -----------------------

        /// <summary>
        /// Get this input value as a 1-dimensional axis.
        /// </summary>
        /// <returns>Input value as a <see cref="float"/></returns>
        public abstract float GetAxis();

        /// <summary>
        /// Get this input axis value as a <see cref="Vector2"/>.
        /// </summary>
        /// <returns>Input value as a <see cref="Vector2"/>.</returns>
        public abstract Vector2 GetVector2Axis();

        /// <summary>
        /// Get this input axis value as a <see cref="Vector3"/>.
        /// </summary>
        /// <returns>Input value as a <see cref="Vector3"/>.</returns>
        public abstract Vector3 GetVector3Axis();
        #endregion
    }
}
