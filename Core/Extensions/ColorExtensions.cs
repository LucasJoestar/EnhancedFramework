// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="Color"/> struct.
    /// </summary>
	public static class ColorExtensions {
        #region Extensions
        /// <summary>
        /// Set the alpha value of this color.
        /// </summary>
        /// <param name="_color">Color to set alpha.</param>
        /// <param name="_alpha">New alpha value.</param>
		public static Color SetAlpha(this Color _color, float _alpha) {
            return new Color(_color.r, _color.g, _color.b, _alpha);
        }
        #endregion
    }
}
