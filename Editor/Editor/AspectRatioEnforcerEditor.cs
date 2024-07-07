// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="AspectRatioEnforcer"/> editor.
    /// </summary>
    [CustomEditor(typeof(AspectRatioEnforcer), true)]
    public sealed class AspectRatioEnforcerEditor : UnityObjectEditor {
        #region Editor GUI
        private const float Ratio_16to9     = 16f / 9f;
        private const float Ratio_16to10    = 16f / 10f;
        private const float Ratio_21to9     = 21f / 9f;
        private const float Ratio_4to3      = 4f / 3f;
        private const float Ratio_5to4      = 5f / 4f;
        private const float Ratio_1to1      = 1f;

        private static readonly GUIContent headerGUI        = new GUIContent("Aspect Ratio Presets:");

        private static readonly GUIContent ratio_16to9GUI   = new GUIContent("16:9");
        private static readonly GUIContent ratio_16to10GUI  = new GUIContent("16:10");
        private static readonly GUIContent ratio_21to9GUI   = new GUIContent("21:9");
        private static readonly GUIContent ratio_4to3GUI    = new GUIContent("4:3");
        private static readonly GUIContent ratio_5to4GUI    = new GUIContent("5:4");
        private static readonly GUIContent ratio_1to1GUI    = new GUIContent("1:1");

        private SerializedProperty aspectRatio = null;

        // -----------------------

        protected override void OnEnable() {
            base.OnEnable();

            // Property init.
            aspectRatio = serializedObject.FindProperty("aspectRatio");
        }

        protected override void OnAfterInspectorGUI() {
            base.OnAfterInspectorGUI();

            GUILayout.Space(10f);
            GUILayout.Label(headerGUI);

            serializedObject.Update();

            using (var _scope = new GUILayout.HorizontalScope()) {

                if (GUILayout.Button(ratio_16to9GUI)) {
                    aspectRatio.floatValue = Ratio_16to9;
                }
                if (GUILayout.Button(ratio_16to10GUI)) {
                    aspectRatio.floatValue = Ratio_16to10;
                }
                if (GUILayout.Button(ratio_21to9GUI)) {
                    aspectRatio.floatValue = Ratio_21to9;
                }
                if (GUILayout.Button(ratio_4to3GUI)) {
                    aspectRatio.floatValue = Ratio_4to3;
                }
                if (GUILayout.Button(ratio_5to4GUI)) {
                    aspectRatio.floatValue = Ratio_5to4;
                }
                if (GUILayout.Button(ratio_1to1GUI)) {
                    aspectRatio.floatValue = Ratio_1to1;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        #endregion
    }
}
