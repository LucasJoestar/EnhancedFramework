// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Utility <see cref="Component"/> used to open a specific URL on game quit (only from build).
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Utility/Open URL On Quit"), DisallowMultipleComponent]
    #pragma warning disable
    public class OpenURLOnQuit : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Open URL On Quit")]

        [Tooltip("URL to open when quitting the game (only from build)")]
        [SerializeField] private string url = string.Empty;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            #if UNITY_EDITOR
            return;
            #endif

            Application.quitting += OnQuit;
        }

        // -----------------------

        private void OnQuit() {
            Application.OpenURL(url);
        }
        #endregion
    }
}
