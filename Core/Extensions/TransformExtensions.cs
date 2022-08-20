// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="Transform"/> class.
    /// </summary>
	public static class TransformExtensions {
        #region Extensions
        /// <summary>
        /// Get the value of a specific world-coordinates vector, relatively to this transform local orientation.
        /// </summary>
        /// <param name="_transform">Transform of the object.</param>
        /// <param name="_vector">Vector to get relative value.</param>
		public static Vector3 RelativeVector(this Transform _transform, Vector3 _vector) {
            return _vector.RotateInverse(_transform.rotation);
        }

        /// <summary>
        /// Get the value of a specific local-space oriented vector in world space.
        /// </summary>
        /// <param name="_transform">Transform of the object.</param>
        /// <param name="_vector">Vector to get world value.</param>
        public static Vector3 WorldVector(this Transform _transform, Vector3 _vector) {
            return _vector.Rotate(_transform.rotation);
        }
        #endregion
    }
}
