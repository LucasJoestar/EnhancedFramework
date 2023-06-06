// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if UNITY_2020_3 || UNITY_2022_2_OR_NEWER
#define FIND_OBJECT_BY_TYPE
#endif

using EnhancedFramework.Core;
using System;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Contains multiple <see cref="TransformEditorController"/>-utility menus and methods.
    /// </summary>
    public class TransformControllerUtility : MonoBehaviour {
        #region Content
        [MenuItem("Tools/Transform Utility/Set Editor Position", false, 101)]
        public static void SetEditorPosition() {
            GetControllers(Set);

            // ----- Local Method ----- \\

            void Set(TransformEditorController _controller) {
                _controller.SetEditorPosition();
            }
        }

        [MenuItem("Tools/Transform Utility/Set Runtime Position", false, 102)]
        public static void SetRuntimePosition() {
            GetControllers(Set);

            // ----- Local Method ----- \\

            void Set(TransformEditorController _controller) {
                _controller.SetRuntimePosition();
            }
        }

        // -----------------------

        private static void GetControllers(Action<TransformEditorController> _onController) {

            TransformEditorController[] _controllers;

            #if FIND_OBJECT_BY_TYPE
            _controllers = FindObjectsByType<TransformEditorController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            #else
            _controllers = FindObjectsOfType<TransformEditorController>(true);
            #endif

            foreach (TransformEditorController _controller in _controllers) {
                _onController(_controller);
            }
        }
        #endregion
    }
}
