// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="Bounds"/> class.
    /// </summary>
    public static class BoundsExtensions {
        #region Content
        /// <summary>
        /// Get the world space center position of this bounding box.
        /// </summary>
        /// <param name="_bounds">Bounds to get world position.</param>
        /// <param name="_transform">The transform this bounds is attached to.</param>
        /// <returns>World space position of these bounds.</returns>
        public static Vector3 Position(this Bounds _bounds, Transform _transform) {
            return _bounds.center + _transform.position;
        }

        /// <summary>
        /// Get the world space max position of this bounding box.
        /// </summary>
        /// <param name="_bounds">Bounds to get max position.</param>
        /// <param name="_transform">The transform this bounds is attached to.</param>
        /// <param name="_isRightSide">True for right-side oriented position, false for left.</param>
        /// <returns>World space max position of these bounds.</returns>
        public static Vector3 Max(this Bounds _bounds, Transform _transform, bool _isRightSide = true) {
            Vector3 _pos = _bounds.Position(_transform);
            return _pos + (_bounds.extents * _isRightSide.Sign());
        }

        /// <summary>
        /// Get the world space coordinates of this bounding box.
        /// </summary>
        /// <param name="_bounds">Bounds to get world space coordinates.</param>
        /// <param name="_transform">The transform this bounds is attached to.</param>
        /// <returns>World space coordinates of these bounds.</returns>
        public static Bounds ToWorld(this Bounds _bounds, Transform _transform) {
            return new Bounds(_bounds.center + _transform.position, _bounds.size);
        }

        /// <summary>
        /// Does another bounding box intersects with this bounding box?
        /// </summary>
        /// <param name="_bounds">Reference bounds.</param>
        /// <param name="_transform">The transform this reference bounds is attached to.</param>
        /// <param name="_other">Other bounds to test intersection.</param>
        /// <param name="_otherTransform">The transform the other bounds is attached to.</param>
        /// <returns>True if the two bounds do intersect each other, false otherwise.</returns>
        public static bool Intersects(this Bounds _bounds, Transform _transform, Bounds _other, Transform _otherTransform) {
            Bounds a = _bounds.ToWorld(_transform);
            Bounds b = _other.ToWorld(_otherTransform);

            return a.Intersects(b);
        }
        #endregion
    }
}
