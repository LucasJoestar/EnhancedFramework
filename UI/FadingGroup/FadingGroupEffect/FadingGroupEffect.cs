// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Base class to derive any <see cref="FadingGroup"/>-related effect from.
    /// </summary>
    public abstract class FadingGroupEffect : EnhancedBehaviour {
        public const string MenuPath = FrameworkUtility.MenuPath + "UI/Fading Group/Effect/";

        #region Global Members
        [PropertyOrder(1)]

        [Tooltip("Source fading object to trigger this effect from")]
        [SerializeField, Enhanced, Required] protected FadingObjectBehaviour source = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Registration.
            if (source.FadingObject is FadingGroup _group) {
                _group.RegisterEffect(this);
            }
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Unregistration.
            if (source.FadingObject is FadingGroup _group) {
                _group.UnregisterEffect(this);
            }
        }

        #if UNITY_EDITOR
        // -------------------------------------------
        // Editor
        // -------------------------------------------

        protected override void OnValidate() {
            
            // Reference.
            if (!source) {
                source = GetComponent<FadingObjectBehaviour>();
            }
        }
        #endif
        #endregion

        #region Effect
        /// <summary>
        /// Called when the source visibility has changed.
        /// </summary>
        /// <param name="_visible">Group visibility.</param>
        /// <param name="_instant">Whether visibility was instantly changed or not.</param>
        internal protected abstract void OnSetVisibility(bool _visible, bool _instant);
        #endregion
    }
}
