// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Editor class used to get each <see cref="SceneResourceManager"/> instance resources without action from the user.
    /// </summary>
    [InitializeOnLoad]
    internal static class EditorSceneResourceManager {
        #region Content
        static EditorSceneResourceManager() {
            EditorSceneManager.sceneSaving += OnSavingScene;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        // -----------------------

        private static void OnSavingScene(Scene _scene, string _path) {
            SceneResourceManager[] _instances = Object.FindObjectsOfType<SceneResourceManager>();
            int _instanceCount = 0;

            // Register resources when a scene is being saved.
            foreach (SceneResourceManager _instance in _instances) {
                if (_scene != _instance.gameObject.scene) {
                    continue;
                }

                _instance.GetSceneResources();
                _instanceCount++;

                EditorUtility.SetDirty(_instance);

                // Log for when multiple instances are found in a scene.
                if (_instanceCount != 1) {
                    _instance.LogWarning($"Multiple \'{typeof(SceneResourceManager).Name}\' instances ({_instanceCount}) found in the scene \'{_scene.name}\'. " +
                                         $"Duplicate instances should be removed from the scene.");
                }
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange _state) {
            if (_state != PlayModeStateChange.ExitingEditMode) {
                return;
            }

            // Get resources before entering play mode.
            SceneResourceManager[] _instances = Object.FindObjectsOfType<SceneResourceManager>();
            foreach (SceneResourceManager _instance in _instances) {
                _instance.GetSceneResources();
            }
        }
        #endregion
    }
}
