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
    /// UI utility class used to set a canvas render camera at runtime.
    /// </summary>
    public class CanvasCameraReference : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Canvas Camera Reference")]

        [SerializeField, Enhanced, Required] private Canvas canvas = null;
        [SerializeField, Enhanced, Required] private new CrossSceneReference<Camera> camera = new CrossSceneReference<Camera>();
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Initialize this canvas render mode.
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
        }

        #if UNITY_EDITOR
        private void OnValidate() {
            if (!canvas) {
                canvas = GetComponent<Canvas>();
            }
        }
        #endif
        #endregion
    }
}
