// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base interface to inherit any fading object from.
    /// </summary>
    public interface IFadingObject {
        #region Content
        /// <summary>
        /// Indicates whether this object is currently visible or not.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Fades in this object and show it.
        /// </summary>
        /// <param name="_onComplete">Called once this object fade is complete.</param>
        void Show(Action _onComplete = null);

        /// <summary>
        /// Fades out this object and hide it.
        /// </summary>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void Hide(Action _onComplete = null);

        /// <summary>
        /// Inverts this object visibility (show it if hidden, hide if if visible).
        /// </summary>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void Invert(Action _onComplete = null);

        /// <summary>
        /// Sets this object visibility and show/hide it accordingly.
        /// </summary>
        /// <param name="_isVisible">Should this object be visible?</param>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void SetVisibility(bool _isVisible, Action _onComplete = null);
        #endregion
    }
}
