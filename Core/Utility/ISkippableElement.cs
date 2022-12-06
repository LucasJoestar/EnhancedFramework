// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base interface for any skippable element of the game.
    /// </summary>
    public interface ISkippableElement {
        #region Content
        /// <summary>
        /// Can this element be skipped?
        /// </summary>
        bool IsSkippable { get; }

        /// <summary>
        /// Tries to skip this element.
        /// </summary>
        /// <returns>True if this element could be successfully skipped, false otherwise.</returns>
        bool Skip();
        #endregion
    }
}
