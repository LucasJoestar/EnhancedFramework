// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Contains multiple UI selection utility members.
    /// </summary>
    public static class SelectionUtility {
        #region Content
        private const float SelectionMinDelay = .01f;

        private static float selectionTime  = 0f;
        private static bool isSelection     = false;

        /// <summary>
        /// Whether a selection is currently active or not.
        /// </summary>
        public static bool IsSelection {
            get {
                if (!isSelection) {
                    return false;
                }

                float _duration = Time.unscaledTime - selectionTime;
                return _duration > SelectionMinDelay;
            }
        }

        // -----------------------

        /// <summary>
        /// Call this when a UI object is being selected.
        /// </summary>
        /// <returns>True if a selection was active, false is this selection initiates the state.</returns>
        public static bool ApplySelection() {
            bool _isSelection = isSelection;
            isSelection = true;

            if (!_isSelection) {
                selectionTime = Time.unscaledTime;
            }

            return _isSelection;
        }

        /// <summary>
        /// Resets the current UI selection state.
        /// </summary>
        public static void ResetSelection() {
            isSelection = false;
        }
        #endregion
    }
}
