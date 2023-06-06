// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

using ArrayUtility = EnhancedEditor.ArrayUtility;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Editor class used to get each <see cref="SceneResourceManager"/> instance resources without action from the user.
    /// </summary>
    [InitializeOnLoad]
    internal static class LevelTriggerAreaDrawer {
        #region Content
        private static LevelTriggerArea[] triggerAreas = new LevelTriggerArea[0];

        // -----------------------

        static LevelTriggerAreaDrawer() {

            EditorSceneManager.sceneOpened  += OnOpenedScene;
            EditorSceneManager.sceneSaved   += OnSavedScene;

            SceneManager.sceneLoaded        += OnLoadedScene;
            SceneView.duringSceneGui        += OnSceneGUI;

            GetTriggerAras();
        }

        // -------------------------------------------
        // Callbacks
        // -------------------------------------------

        private static void OnOpenedScene(Scene _scene, OpenSceneMode _mode) {
            GetTriggerAras();
        }

        private static void OnSavedScene(Scene _scene) {
            GetTriggerAras();
        }

        private static void OnLoadedScene(Scene _scene, LoadSceneMode _mode) {
            GetTriggerAras();
        }

        private static void OnSceneGUI(SceneView _sceneView) {

            for (int i = triggerAreas.Length; i-- > 0;) {

                if (triggerAreas[i] == null) {
                    ArrayUtility.RemoveAt(ref triggerAreas, i);
                    continue;
                }

                triggerAreas[i].DrawArea();
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        private static void GetTriggerAras() {
            triggerAreas = Object.FindObjectsOfType<LevelTriggerArea>();
        }
        #endregion
    }
}
