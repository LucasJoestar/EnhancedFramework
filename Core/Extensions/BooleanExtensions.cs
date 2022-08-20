// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

namespace EnhancedFramework.Core {
    /// <summary>
    /// Multiple extension methods related to the <see cref="bool"/> value type.
    /// </summary>
    public static class BooleanExtensions {
        #region Content
        /// <summary>
        /// Get a boolean as a sign.
        /// <br/> 1 if true, -1 otherwise.
        /// </summary>
        /// <param name="boolean">Boolean to get sign from.</param>
        /// <returns>Returns this boolean sign as 1 or -1.</returns>
        public static int Sign(this bool _boolean) {
            return _boolean ? 1 : -1;
        }

        /// <summary>
        /// Get a boolean as a sign.
        /// <br/> 1 if true, -1 otherwise.
        /// </summary>
        /// <param name="_boolean">Boolean to get sign from.</param>
        /// <returns>Returns this boolean sign as 1 or -1.</returns>
        public static float Signf(this bool _boolean) {
            return _boolean ? 1f : -1f;
        }
        #endregion
    }
}
