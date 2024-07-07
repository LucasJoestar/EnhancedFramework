// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if UNITY_2020_3 || UNITY_2021_3 || UNITY_2022_2_OR_NEWER
#define FIND_OBJECT_BY_TYPE
#endif

using EnhancedFramework.Core;
using System;
using UnityEditor;

#if FIND_OBJECT_BY_TYPE
using UnityEngine;
#endif

using Object = UnityEngine.Object;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Contains multiple <see cref="TransformEditorController"/>-utility menus and methods.
    /// </summary>
    public static class TransformControllerUtility {
        #region Content
        private const string Path = FrameworkUtility.MenuItemPath + "Transform Utility/";

        // -----------------------

        [MenuItem(Path + "Set Editor Position", false, 101)]
        public static void SetEditorPosition() {
            GetControllers(Set);

            // ----- Local Method ----- \\

            static void Set(TransformEditorController _controller) {
                _controller.SetEditorPosition();
            }
        }

        [MenuItem(Path + "Set Runtime Position", false, 102)]
        public static void SetRuntimePosition() {
            GetControllers(Set);

            // ----- Local Method ----- \\

            static void Set(TransformEditorController _controller) {
                _controller.SetRuntimePosition();
            }
        }

        // -------------------------------------------
        // Button
        // -------------------------------------------

        private static void GetControllers(Action<TransformEditorController> _callback) {

            TransformEditorController[] _controllers;

            #if FIND_OBJECT_BY_TYPE
            _controllers = Object.FindObjectsByType<TransformEditorController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            #else
            _controllers = Object.FindObjectsOfType<TransformEditorController>(true);
            #endif

            foreach (TransformEditorController _controller in _controllers) {
                _callback(_controller);
            }
        }
        #endregion
    }
}
