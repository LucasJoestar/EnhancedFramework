// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Base class for <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingObjectTransitionFadingGroup"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="FadingObjectTransitionFadingGroup"/> class type used by this object.</typeparam>
    public abstract class FadingObjectTransitionFadingGroupBehaviour<T> : FadingGroupBehaviour<T> where T : FadingObjectTransitionFadingGroup, new() {
        #region Behaviour
        /// <inheritdoc cref="FadingObjectTransitionFadingGroup.StartFadeIn(Action, Action)"/>
        public virtual void StartFadeIn(Action _onFaded = null, Action _onComplete = null) {
            group.StartFadeIn(_onFaded, _onComplete);
        }

        /// <inheritdoc cref="FadingObjectTransitionFadingGroup.StartFadeOut(Action, Action)"/>
        public virtual void StartFadeOut(Action _onFaded = null, Action _onComplete = null) {
            group.StartFadeOut(_onFaded, _onComplete);
        }

        /// <inheritdoc cref="FadingObjectTransitionFadingGroup.CompleteFade(Action)"/>
        public virtual void CompleteFade(Action _onComplete = null) {
            group.CompleteFade(_onComplete);
        }

        // -----------------------

        /// <summary>
        /// Shows this transition group.
        /// </summary>
        /// <param name="_onFadeIn">Called once the transition group has faded in.</param>
        /// <param name="_onFadeOut">Called before the transition group fade out.</param>
        /// <param name="_onComplete">Called once fading has been completed.</param>
        public virtual void ShowTransition(Action _onFadeIn = null, Action _onFadeOut = null, Action _onComplete = null) {
            StartFadeIn(_onFadeIn, OnFaded);

            // ----- Local Method ----- \\

            void OnFaded() {

                _onFadeOut?.Invoke();
                CompleteFade(_onComplete);
            }
        }

        /// <summary>
        /// Hides this transition group.
        /// </summary>
        /// <inheritdoc cref="ShowTransition(Action, Action, Action)"/>
        public virtual void HideTransition(Action _onFadeIn = null, Action _onFadeOut = null, Action _onComplete = null) {
            StartFadeOut(_onFadeIn, OnFaded);

            // ----- Local Method ----- \\

            void OnFaded() {

                _onFadeOut?.Invoke();
                CompleteFade(_onComplete);
            }
        }
        #endregion
    }

    /// <summary>
    /// Ready-to-use <see cref="EnhancedBehaviour"/>-encapsulated <see cref="TransitionFadingGroup"/>.
    /// <br/> Use this to quickly implement fading <see cref="CanvasGroup"/> using another transition group.
    /// </summary>
    public class TransitionFadingGroupBehaviour : FadingObjectTransitionFadingGroupBehaviour<TransitionFadingGroup> { }
}
