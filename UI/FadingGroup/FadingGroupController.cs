// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;

namespace EnhancedFramework.UI {
    /// <summary>
    /// <see cref="FadingGroup"/>-related event receiver.
    /// </summary>
    public class FadingGroupController : EnhancedBehaviour {
        #region Global Members
        /// <summary>
        /// If true, executes this controller event in edit mode.
        /// </summary>
        public virtual bool ExecuteInEditMode {
            get { return false; }
        }
        #endregion

        #region Callback
        /// <summary>
        /// Called when the associated group starts being shown.
        /// </summary>
        /// <param name="_group">Associated <see cref="FadingGroup"/>.</param>
        public virtual void OnShowStarted(FadingGroup _group) { }

        /// <summary>
        /// Called when the associated group is shown.
        /// </summary>
        /// <param name="_group">Associated <see cref="FadingGroup"/>.</param>
        public virtual void OnShowPerformed(FadingGroup _group) { }

        /// <summary>
        /// Called when the associated group completes its showing operation.
        /// </summary>
        /// <param name="_group">Associated <see cref="FadingGroup"/>.</param>
        public virtual void OnShowCompleted(FadingGroup _group) { }

        // -----------------------

        /// <summary>
        /// Called when the associated group starts being hidden.
        /// </summary>
        /// <param name="_group">Associated <see cref="FadingGroup"/>.</param>
        public virtual void OnHideStarted(FadingGroup _group) { }

        /// <summary>
        /// Called when the associated group is hidden.
        /// </summary>
        /// <param name="_group">Associated <see cref="FadingGroup"/>.</param>
        public virtual void OnHidePerformed(FadingGroup _group) { }

        /// <summary>
        /// Called when the associated group completes its hiding operation.
        /// </summary>
        /// <param name="_group">Associated <see cref="FadingGroup"/>.</param>
        public virtual void OnHideCompleted(FadingGroup _group) {  }
        #endregion
    }
}
