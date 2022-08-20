// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

namespace EnhancedFramework.Core {
    /// <summary>
    /// Pair struct, used to associate two values together.
    /// </summary>
    /// <typeparam name="T">Type of the first pair value.</typeparam>
    /// <typeparam name="U">Type of the second pair value.</typeparam>
    public struct Pair<T, U> {
        #region Global Members
        /// <summary>
        /// First pair value.
        /// </summary>
        public T First;

        /// <summary>
        /// Second pair value.
        /// </summary>
        public U Second;

        // -----------------------

        /// <summary>
        /// <inheritdoc cref="Pair{T, U}"/>
        /// </summary>
        /// <param name="_first"><inheritdoc cref="First" path="/summary"/></param>
        /// <param name="_second"><inheritdoc cref="Second" path="/summary"/></param>
        public Pair(T _first, U _second) {
            First = _first;
            Second = _second;
        }
        #endregion
    }
}
