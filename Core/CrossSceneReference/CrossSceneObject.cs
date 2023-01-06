// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnhancedFramework.Core {
    /// <summary>
    /// Class instance used to retrieve a specific <see cref="Component"/> instance from another scene using this object guid.
    /// </summary>
    #pragma warning disable IDE0051
    public class CrossSceneObject : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Cross Scene Object")]

        [SerializeField, Enhanced, ReadOnly] private int guid = EnhancedEditor.EnhancedUtility.GenerateGUID();

        /// <summary>
        /// The unique GUID this object, acting as an identifier.
        /// </summary>
        public int GUID {
            get { return guid; }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            CrossSceneReferenceManager.Instance.Register(this);
        }

        private void OnDestroy() {
            CrossSceneReferenceManager.Instance.Unregister(this);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Use this to generate a new GUID in case of duplicate.
        /// </summary>
        [Button(SuperColor.Pumpkin)]
        private void GenerateGUID() {
            #if UNITY_EDITOR
            if (!EditorUtility.DisplayDialog("Generate GUID", "Are you sure you want to generate a new GUID for this instance?\n\n" +
                                             "All references will be lost.", "Confirm", "Cancel")) {
                return;
            }
#endif

            guid = EnhancedEditor.EnhancedUtility.GenerateGUID();
        }
        #endregion
    }
}
