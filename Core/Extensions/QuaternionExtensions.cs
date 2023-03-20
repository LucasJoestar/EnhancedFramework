// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="Quaternion"/> struct.
    /// </summary>
    public static class QuaternionExtensions {
        #region Vector
        /// <summary>
        /// Converts this quaternion to its direction <see cref="Vector3"/>.
        /// </summary>
        /// <param name="_quaternion">Quaternion to convert.</param>
        /// <returns><see cref="Vector3"/> looking to this quaternion rotation.</returns>
        public static Vector3 ToDirection(this Quaternion _quaternion) {
            return _quaternion * Vector3.forward;
        }
        #endregion
    }
}
