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
        #region Mathematic
        /// <summary>
        /// Clamps the given value between a minimum and a maximum.
        /// </summary>
        /// <param name="_value">The value to restrict inside the range of the min and max values.</param>
        /// <param name="_min">The minimum value to compare against.</param>
        /// <param name="_max">The maximum value to compare against.</param>
        /// <returns>The result between the min and max values.</returns>
        public static double Clamp(double _value, double _min, double _max) {
            return (_value < _min)
                 ? _min : ((_value > _max)
                        ? _max : _value);
        }


        /// <inheritdoc cref="IsInRange(float, float, float)"/>
        public static bool IsInRange(int _value, int _min, int _max) {
            return (_value >= _min) && (_value <= (_max));
        }

        /// <summary>
        /// Get if a specific value is within a given range.
        /// </summary>
        /// <param name="_value">The value to evaluate.</param>
        /// <param name="_min">Minimum allowed value.</param>
        /// <param name="_max">Maximum allowed value.</param>
        /// <returns>True if the specified value is within the given range, false otherwise.</returns>
        public static bool IsInRange(float _value, float _min, float _max) {
            return (_value >= _min) && (_value <= (_max));
        }
        #endregion

        #region Decimal
        /// <summary>
        /// Rounds a given <see cref="float"/> value to a given decimals place.
        /// </summary>
        /// <param name="_value">The value to round.</param>
        /// <param name="_decimal">The final amount of decimal(<see langword="sealed"/>).</param>
        /// <returns>The rounded <see cref="float"/> value.</returns>
        public static float RoundToDecimal(float _value, int _decimal) {
            if (_decimal == 0) {
                return Mathf.Round(_value);
            }

            float _factor = Mathf.Pow(10f, _decimal);
            return Mathf.Round(_value * _factor) / _factor;
        }

        public static float FloorToDecimal(float _value, int _decimal) {
            if (_decimal == 0) {
                return Mathf.Floor(_value);
            }

            float _factor = Mathf.Pow(10f, _decimal);
            return Mathf.Floor(_value * _factor) / _factor;
        }

        public static float CeilToDecimal(float _value, int _decimal) {
            if (_decimal == 0) {
                return Mathf.Ceil(_value);
            }

            float _factor = Mathf.Pow(10f, _decimal);
            return Mathf.Ceil(_value * _factor) / _factor;
        }
        #endregion

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
