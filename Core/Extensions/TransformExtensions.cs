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

        /// <summary>
        /// Set this transform local values.
        /// </summary>
        /// <param name="_transform">This transform instance.</param>
        /// <param name="_position">The local position of the transform.</param>
        /// <param name="_rotation">The local rotation of the transform.</param>
        /// <param name="_scale">The local scale of the transform.</param>
        public static void SetLocal(this Transform _transform, Vector3 _position, Quaternion _rotation, Vector3 _scale) {
            _transform.localPosition = _position;
            _transform.localRotation = _rotation;
            _transform.localScale = _scale;
        }

        /// <summary>
        /// Resets this transform local values (position, rotation and scale) back to default.
        /// </summary>
        /// <param name="_transform">This transform instance.</param>
        public static void ResetLocal(this Transform _transform) {
            _transform.SetLocal(Vector3.zero, Quaternion.identity, Vector3.one);
        }
        #endregion
    }
}
