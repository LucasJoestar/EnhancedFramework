// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple <see cref="Enum"/>-related extension methods.
    /// </summary>
    public static class EnumExtensions {
        #region Conversion
        /// <summary>
        /// Get the integer value of a specific enum.
        /// </summary>
        /// <param name="_value">Enum value to convert.</param>
        /// <returns>Int value of this enum.</returns>
        public static int ToInt(this Enum _value) {
            return Convert.ToInt32(_value);
        }
        #endregion
    }
}
