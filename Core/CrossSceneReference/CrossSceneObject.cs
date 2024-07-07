// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Class instance used to retrieve a specific <see cref="Component"/> instance from another scene using this object guid.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-300)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Cross Scene/Cross Scene Object"), DisallowMultipleComponent]
    public sealed class CrossSceneObject : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            CrossSceneReferenceManager.Instance.Register(this);
        }

        private void OnDestroy() {
            if (!GameManager.IsQuittingApplication) {
                CrossSceneReferenceManager.Instance.Unregister(this);
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Copies this object id to the buffer.
        /// </summary>
        /// <returns>This object id.</returns>
        [Button(SuperColor.Green)]
        [ContextMenu("Copy ID", false, 30)]
        public string CopyID() {
            string _id = ID.ToString();

            GUIUtility.systemCopyBuffer = _id;
            return _id;
        }
        #endregion
    }
}
