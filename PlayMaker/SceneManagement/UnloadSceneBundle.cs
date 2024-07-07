// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to unload a <see cref="EnhancedEditor.SceneBundle"/>.
    /// </summary>
    [Tooltip("Unloads a Scene Bundle")]
    [ActionCategory("SceneBundle")]
    public sealed class UnloadSceneBundle : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable
        // -------------------------------------------

        [Tooltip("The Scene Bundle to unload.")]
        [RequiredField, ObjectType(typeof(SceneBundle))]
        public FsmObject SceneBundle = null;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            SceneBundle = null;
        }

        public override void OnEnter() {
            base.OnEnter();

            if (SceneBundle.Value is SceneBundle _bundle) {
                EnhancedSceneManager.Instance.UnloadSceneBundle(_bundle, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            }

            Finish();
        }
        #endregion
    }
}
