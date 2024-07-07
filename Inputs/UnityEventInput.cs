// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.Events;

namespace EnhancedFramework.Inputs {
    /// <summary>
    /// Utility component used to automatically called a specific <see cref="UnityEvent"/> oon input.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Input/Unity Event Input")]
    public sealed class UnityEventInput : EnhancedBehaviour {
        #region Global Members
        [Section("Unity Event Input")]

        [SerializeField, Enhanced, Required] private InputActionEnhancedAsset input = null;

        [Space(5f)]

        [SerializeField] private UnityEvent onPerformed = new UnityEvent();
        [SerializeField] private UnityEvent onCanceled  = new UnityEvent();
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            input.OnPerformed += OnPerformed;
            input.OnCanceled  += OnCanceled;
            input.Enable();
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            input.OnPerformed -= OnPerformed;
            input.OnCanceled  -= OnCanceled;
            input.Disable();
        }

        // -----------------------

        private void OnPerformed(InputActionEnhancedAsset _) {
            onPerformed.Invoke();
        }

        private void OnCanceled(InputActionEnhancedAsset _) {
            onCanceled.Invoke();
        }
        #endregion
    }
}
