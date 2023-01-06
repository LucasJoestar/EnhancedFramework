// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="IFadingObject"/>-related fading mode.
    /// </summary>
    public enum FadingMode {
        None = 0,
        Show,
        Hide,
        FadeInOut,
    }

    /// <summary>
    /// Base interface to inherit any fading object from.
    /// </summary>
    public interface IFadingObject {
        #region Content
        /// <summary>
        /// Indicates whether this object is currently visible or not.
        /// </summary>
        bool IsVisible { get; }

        // -----------------------

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
        /// Fades in this object and show it, then wait for a given time, and fade out.
        /// </summary>
        /// <param name="_duration">The duration to wait before fading out.</param>
        /// <param name="_onAfterFadeIn">Called right after fading int.</param>
        /// <param name="_onBeforeFadeOut">Called right before fading out.</param>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null);

        /// <summary>
        /// Fades this object according to a given <see cref="FadingMode"/>.
        /// </summary>
        /// <param name="_mode">The <see cref="FadingMode"/> used to fade this object.</param>
        /// <param name="_onComplete"><inheritdoc cref="Show(Action)" path="/param[@name='_onComplete']"/></param>
        /// <param name="_inOutWaitDuration">The duration to wait before fading out if using <see cref="FadingMode.FadeInOut"/>.</param>
        void Fade(FadingMode _mode, Action _onComplete = null, float _inOutWaitDuration = .5f);

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

        // -----------------------

        /// <param name="_isInstant">If true, instantly fades this object.</param>
        /// <inheritdoc cref="Show(Action)"/>
        void Show(bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="Hide(Action)"/>
        void Hide(bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="Fade(FadingMode, Action, float)"/>
        void Fade(FadingMode _mode, bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="Invert(Action)"/>
        void Invert(bool _isInstant, Action _onComplete = null);

        /// <param name="_isInstant"><inheritdoc cref="Show(bool)" path="/param[@name='_isInstant']"/></param>
        /// <inheritdoc cref="SetVisibility(bool, Action)"/>
        void SetVisibility(bool _isVisible, bool _isInstant, Action _onComplete = null);
        #endregion
    }
}
