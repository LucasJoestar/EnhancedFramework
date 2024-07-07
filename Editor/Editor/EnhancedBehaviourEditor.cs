// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="EnhancedBehaviour"/> editor.
    /// </summary>
    [CustomEditor(typeof(EnhancedBehaviour), true), CanEditMultipleObjects]
    public class EnhancedBehaviourEditor : UnityObjectEditor {
        #region Editor GUI
        private static readonly PlayModeEnhancedObjectData data = new PlayModeEnhancedObjectData();

        protected override bool CanSaveData {
            get {
                if (target is EnhancedBehaviour _behaviour) {
                    return _behaviour.CanSavePlayModeData;
                }

                return false;
            }
        }

        // -----------------------

        protected override bool DrawInspectorGUI() {

            if (CanSaveData) {
                GUILayout.Space(5f);
                SaveLoadButtonGUILayout();
            }

            return base.DrawInspectorGUI();
        }

        protected virtual void OnSceneGUI() {

            // Behaviour handles.
            EnhancedBehaviour _behaviour = target as EnhancedBehaviour;
            _behaviour.OnDrawHandles();
        }

        // -------------------------------------------
        // Callback
        // -------------------------------------------

        protected override PlayModeObjectData SaveData(Object _object) {
            data.Save(_object);
            return data;
        }
        #endregion
    }
}
