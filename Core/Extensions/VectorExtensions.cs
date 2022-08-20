// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System.Text;
using UnityEngine;

namespace EnhancedFramework.Core {
	/// <summary>
	/// Multiple extension methods related to the <see cref="Vector2"/> and <see cref="Vector3"/> classes.
	/// </summary>
	public static class VectorExtensions {
		#region Random
		/// <summary>
		/// Get a random value between this vector X & Y components.
		/// </summary>
		/// <param name="_value">Vector to get random value from.</param>
		/// <returns>Random value between this vector X & Y components.</returns>
		public static float Random(this Vector2 _value) {
			return UnityEngine.Random.Range(_value.x, _value.y);
		}
		#endregion

		#region Null Check
		/// <inheritdoc cref="IsNull(Vector3)"/>
		public static bool IsNull(this Vector2 _vector) {
			return (_vector.x == 0f) && (_vector.y == 0f);
		}

		/// <summary>
		/// Get if a specific vector is null, that is if all its component values are equal to zero.
		/// </summary>
		public static bool IsNull(this Vector3 _vector) {
			return (_vector.x == 0f) && (_vector.y == 0f) && (_vector.z == 0f);
		}
		#endregion

		#region Geometry
		/// <summary>
		/// Subtracts any part of a direction vector parallel to a normal vector,
		/// leaving only the perpendicular component.
		/// </summary>
		public static Vector3 PerpendicularSurface(this Vector3 _direction, Vector3 _normal) {
			return _direction - (_normal * Vector3.Dot(_direction, _normal));
		}

		/// <summary>
		/// Get this vector flat value, that is without its vertical (Y axis) component.
		/// </summary>
		public static Vector3 Flat(this Vector3 _vector) {
			return new Vector3(_vector.x, 0f, _vector.z);
        }

		/// <summary>
		/// Sets this vector horizontal (X axis) component.
		/// </summary>
		public static Vector3 SetX(this Vector3 _vector, float _value) {
			return new Vector3(_value, _vector.y, _vector.z);
        }

		/// <summary>
		/// Sets this vector vertical (Y axis) component.
		/// </summary>
		public static Vector3 SetY(this Vector3 _vector, float _value) {
			return new Vector3(_vector.x, _value, _vector.z);
		}

		/// <summary>
		/// Sets this vector forward (Z axis) component.
		/// </summary>
		public static Vector3 SetZ(this Vector3 _vector, float _value) {
			return new Vector3(_vector.x, _vector.y, _value);
		}
		#endregion

		#region Quaternion
		/// <summary>
		/// Converts this vector to a quaternion.
		/// </summary>
		/// <param name="_euler">Vector to convert.</param>
		/// <returns>Quaternion created based on this vector.</returns>
		public static Quaternion ToQuaternion(this Vector3 _euler) {
			return Quaternion.Euler(_euler);
		}

		/// <summary>
		/// Rotates this vector by a quaternion.
		/// </summary>
		/// <param name="_vector">Vector to rotate.</param>
		/// <param name="_quaternion">Quaternion used to rotate the vector.</param>
		/// <returns>The rotated vector.</returns>
		public static Vector3 Rotate(this Vector3 _vector, Quaternion _quaternion) {
			return _quaternion * _vector;
		}

		/// <summary>
		/// Rotates inverse this vector by a quaternion.
		/// </summary>
		/// <param name="_vector">Vector to rotate.</param>
		/// <param name="_quaternion">Quaternion used to rotate inverse the vector.</param>
		/// <returns>The rotated vector.</returns>
		public static Vector3 RotateInverse(this Vector3 _vector, Quaternion _quaternion) {
			return Quaternion.Inverse(_quaternion) * _vector;
		}
		#endregion

		#region ToString
		/// <inheritdoc cref="ToStringX(Vector3, int)"/>
		public static string ToStringX(this Vector2 _value, int _decimals) {
			StringBuilder _builder = new StringBuilder("0.");
			for (int i = 0; i < _decimals; i++) {
				_builder.Append('#');
			}

			return _value.ToString(_builder.ToString());
		}

		/// <summary>
		/// Parse a specific vector into a string with a specific amount of decimals.
		/// </summary>
		/// <param name="_value">Vector value to parse.</param>
		/// <param name="_decimals">Total amount of decimals to show.</param>
		/// <returns>This vector value parsed into string.</returns>
		public static string ToStringX(this Vector3 _value, int _decimals) {
			StringBuilder _builder = new StringBuilder("0.");
			for (int i = 0; i < _decimals; i++) {
				_builder.Append('#');
			}

			return _value.ToString(_builder.ToString());
		}
		#endregion
	}
}
