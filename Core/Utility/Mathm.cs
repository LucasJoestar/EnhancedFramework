// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Contains multiple useful math utility methods.
    /// </summary>
    public static class Mathm {
        #region Sign
        /// <inheritdoc cref="BooleanExtensions.Sign(bool)"/>
        public static int Sign(bool _boolean) {
            return _boolean.Sign();
        }

        /// <summary>
        /// Get the sign of a specific integer.
        /// </summary>
        /// <param name="_value">Integer value to get sign from.</param>
        /// <returns>-1 if smaller than 0, 1 otherwise.</returns>
        public static int Sign(int _value) {
            return (_value < 0) ? -1 : 1;
        }

        /// <summary>
        /// Get the sign of a specific float.
        /// </summary>
        /// <param name="_value">Float value to get sign from.</param>
        /// <returns>-1 if smaller than 0, 1 otherwise.</returns>
        public static int Sign(float _value) {
            return (_value < 0f) ? -1 : 1;
        }

        /// <summary>
        /// Get if two floats have a different sign.
        /// </summary>
        /// <param name="a">First float to compare.</param>
        /// <param name="b">Second float to compare.</param>
        /// <returns>True if the floats have different signs, false otherwise.</returns>
        public static bool HaveDifferentSign(float a, float b) {
            return Sign(a) != Sign(b);
        }

        /// <summary>
        /// Get if two floats have a different sign and are not null.
        /// </summary>
        /// <param name="a">First float to compare.</param>
        /// <param name="b">Second float to compare.</param>
        /// <returns>True if the floats have different signs and are not equal to 0, false otherwise.</returns>
        public static bool HaveDifferentSignAndNotNull(float a, float b) {
            return (a != 0f) && (b != 0f) && HaveDifferentSign(a, b);
        }
        #endregion

        #region Equality
        private const float FloatPrecision = .001f;

        // -----------------------

        /// <summary>
        /// Get if three integers are all equal.
        /// </summary>
        /// <param name="a">First integer to compare.</param>
        /// <param name="b">Second integer to compare.</param>
        /// <param name="c">Third integers to compare.</param>
        /// <returns>True if all integers are equal, false otherwise.</returns>
        public static bool AreEquals(int a, int b, int c) {
            return (a == b) && (b == c);
        }

        /// <summary>
        /// Get if three floats are all equal.
        /// </summary>
        /// <param name="a">First float to compare.</param>
        /// <param name="b">Second float to compare.</param>
        /// <param name="c">Third float to compare.</param>
        /// <returns>True if all floats are equal, false otherwise.</returns>
        public static bool AreEquals(float a, float b, float c) {
            return (Math.Abs(a - b) < FloatPrecision) && (Math.Abs(b - c) < FloatPrecision);
        }

        /// <inheritdoc cref="VectorExtensions.IsNull(Vector2)"/>
        public static bool IsVectorNull(Vector2 _value) {
            return _value.IsNull();
        }

        /// <inheritdoc cref="VectorExtensions.IsNull(Vector3)"/>
        public static bool IsVectorNull(Vector3 _value) {
            return _value.IsNull();
        }
        #endregion

        #region Geometry
        /// <inheritdoc cref="VectorExtensions.PerpendicularSurface(Vector3, Vector3)"/>
        public static Vector3 ParallelSurface(Vector3 _direction, Vector3 _normal) {
            return _direction.PerpendicularSurface(_normal);
        }
        #endregion
    }
}
