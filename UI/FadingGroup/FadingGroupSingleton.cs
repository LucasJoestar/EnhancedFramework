// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Inherit from this to quickly implement <see cref="EnhancedSingleton{T}"/>-encapsulated <see cref="FadingGroup"/>.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="EnhancedSingleton{T}" path="/typeparam[@name='T']"/></typeparam>
    /// <typeparam name="U"><inheritdoc cref="FadingGroupBehaviour{T}{T}" path="/typeparam[@name='T']"/>.</typeparam>
    public abstract class FadingGroupSingleton<T, U> : EnhancedSingleton<T>, IFadingObject where T : FadingGroupSingleton<T, U> where U : FadingGroup {
        #region Global Members
        [Section("Fading Group")]

        [SerializeField, Enhanced, Block(false)] protected U group = default;

        /// <inheritdoc cref="FadingGroup.Group"/>
        public CanvasGroup Group {
            get { return group.Group; }
        }

        // -----------------------

        public bool IsVisible {
            get { return group.IsVisible; }
        }
        #endregion

        #region Visiblity
        [Button(SuperColor.Green)]
        public virtual void Show(Action _onComplete = null) {
            group.Show(_onComplete);
        }

        [Button(SuperColor.Crimson)]
        public virtual void Hide(Action _onComplete = null) {
            group.Hide(_onComplete);
        }

        public virtual void Invert(Action _onComplete = null) {
            group.Invert(_onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, Action _onComplete = null) {
            group.SetVisibility(_isVisible, _onComplete);
        }
        #endregion
    }
}
