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
        /// Set this transform local values.
        /// </summary>
        /// <param name="_transform">This transform instance.</param>
        /// <param name="_other">Other transform to copy local values from.</param>
        public static void SetLocal(this Transform _transform, Transform _other) {
            SetLocal(_transform, _other.localPosition, _other.localRotation, _other.localScale);
        }

        /// <summary>
        /// Resets this transform local values (position, rotation and scale) back to default.
        /// </summary>
        /// <param name="_transform">This transform instance.</param>
        public static void ResetLocal(this Transform _transform) {
            _transform.SetLocal(Vector3.zero, Quaternion.identity, Vector3.one);
        }

        /// <summary>
        /// Resursively finds a child object from this <see cref="Transform"/>.
        /// </summary>
        /// <param name="_transform">Root <see cref="Transform"/> instance.</param>
        /// <param name="_childName">Name of the child to find.</param>
        /// <param name="_child">Found matchinf child object.</param>
        /// <param name="_exactName">If true, checks for a child object with exactly the given name. Otherwise, checks if the given string is contained in the child name.</param>
        /// <returns>True if a matching child could be successfully found, false otherwise.</returns>
        public static bool FindChildResursive(this Transform _transform, string _childName, out Transform _child, bool _exactName = true) {

            foreach (Transform _temp in _transform) {

                // Exact name.
                if (_exactName) {

                    if (_temp.name == _childName) {

                        _child = _temp;
                        return true;
                    }

                    continue;
                }

                if (_temp.name.ToLower().Contains(_childName.ToLower())) {

                    _child = _temp;
                    return true;
                }

                if (FindChildResursive(_temp, _childName, out _child, _exactName)) {
                    return true;
                }
            }

            _child = null;
            return false;
        }
        #endregion
    }
}
