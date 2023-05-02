// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.UI {
    /// <summary>
    /// <see cref="FadingGroupEffect"/> controlling various sub groups according to a source <see cref="FadingObjectBehaviour"/> state.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(MenuPath + "Sub Fading Group Effect")]
    #pragma warning disable
    public class SubFadingGroupEffect : FadingGroupEffect {
        /// <summary>
        /// Wrapper for a sub group and its parameters.
        /// </summary>
        [Serializable]
        public class SubGroup {
            #region Global Members
            #if UNITY_EDITOR
            [SerializeField] private string name = "Group";
            #endif

            [Tooltip("Fading object wrapped in this sub group")]
            public FadingObjectBehaviour Group = null;

            [Space(5f)]

            [Tooltip("If true, show and hide delays will not be affected by the game time scale")]
            public bool RealTime = false;

            [Tooltip("If true, hides this group when the source is being shown, and shows it when the source is being hidden")]
            public bool Inverse = false;

            [Space(5f)]

            [Tooltip("Delay before showing this group (in seconds)")]
            [Enhanced, Range(0f, 5f)] public float ShowDelay = 0f;

            [Tooltip("Delay before hidding this group (in seconds)")]
            [Enhanced, Range(0f, 5f)] public float HideDelay = 0f;

            // -------------------------------------------
            // Constructor(s)
            // -------------------------------------------

            /// <inheritdoc cref="SubGroup(FadingObjectBehaviour)"/>
            public SubGroup() { }

            /// <inheritdoc cref="SubGroup"/>
            public SubGroup(FadingObjectBehaviour _group) {
                Group = _group;
            }
            #endregion

            #region Utility
            private DelayHandler delay;

            // -----------------------

            /// <summary>
            /// Updates this group state.
            /// </summary>
            /// <inheritdoc cref="SubFadingGroupEffect.OnSetVisibility(bool, bool)"/>
            public void Update(bool _visible, bool _instant) {

                if (Inverse) {
                    _visible = !_visible;
                }

                float _delay = _visible ? ShowDelay : HideDelay;

                delay.Cancel();
                delay = Delayer.Call(ShowDelay, OnComplete, RealTime);

                // ----- Local Method ----- \\

                void OnComplete() {
                    Group.SetVisibility(_visible, _instant);
                }
            }
            #endregion
        }

        #region Global Members
        [Section("Sub Fading Group(s)"), PropertyOrder(0)]

        [Tooltip("All sub groups controlled by the source")]
        [SerializeField] private SubGroup[] subGroups = new SubGroup[] { new SubGroup() };
        #endregion

        #region Effect
        internal protected override void OnSetVisibility(bool _visible, bool _instant) {

            // Sub group(s).
            foreach (SubGroup _group in subGroups) {
                _group.Update(_visible, _instant);
            }
        }
        #endregion
    }
}
