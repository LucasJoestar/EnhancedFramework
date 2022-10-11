// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Base class for <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingGroup"/>.
    /// <para/>
    /// Inherited by <see cref="FadingGroupBehaviour"/> and <see cref="TweeningFadingGroupBehaviour"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="FadingGroup"/> class type used by this object.</typeparam>
    public abstract class FadingGroupBehaviour<T> : EnhancedBehaviour, IFadingObject where T : FadingGroup {
        #region Global Members
        [Section("Fading Group")]

        [SerializeField, Enhanced, Block] protected T group = default;

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
        public virtual void Show(Action _onComplete = null) {
            group.Show(_onComplete);
        }

        public virtual void Hide(Action _onComplete = null) {
            group.Hide(_onComplete);
        }

        public virtual void Invert(Action _onComplete = null) {
            group.Invert(_onComplete);
        }

        public virtual void SetVisibility(bool _isVisible, Action _onComplete = null) {
            group.SetVisibility(_isVisible, _onComplete);
        }

        // -----------------------

        [Button(ActivationMode.Play, SuperColor.Green)]
        protected void ShowGroup() {
            Show();
        }

        [Button(ActivationMode.Play, SuperColor.Crimson)]
        protected void HideGroup() {
            Hide();
        }
        #endregion
    }

    /// <summary>
    /// Ready-to-use <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingGroup"/>.
    /// <br/> Use this to quickly implement instantly fading <see cref="CanvasGroup"/> objects.
    /// </summary>
    public class FadingGroupBehaviour : FadingGroupBehaviour<FadingGroup> { }
}
