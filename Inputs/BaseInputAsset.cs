// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Base class to inherit all <see cref="InputAction"/>-related <see cref="ScriptableObject"/> assets.
    /// </summary>
    public abstract class BaseInputAsset : ScriptableObject {
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
#endif
