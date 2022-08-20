// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Utility methods for root motion related animations.
    /// </summary>
	internal static class RootMotionUtility {
        #region Content
        private static readonly EditorCurveBinding[] transformBindings = new EditorCurveBinding[] {
                                                                            new EditorCurveBinding() {
                                                                                path = "", propertyName = "m_LocalPosition.x", type = typeof(Transform),
                                                                            },
                                                                            new EditorCurveBinding() {
                                                                                path = "", propertyName = "m_LocalPosition.y", type = typeof(Transform),
                                                                            },
                                                                            new EditorCurveBinding() {
                                                                                path = "", propertyName = "m_LocalPosition.z", type = typeof(Transform),
                                                                            }
                                                                        };

        private static readonly EditorCurveBinding[] rootMotionBindings = new EditorCurveBinding[] {
                                                                            new EditorCurveBinding() {
                                                                                path = "RootMotion", propertyName = "m_LocalPosition.x", type = typeof(Transform),
                                                                            },
                                                                            new EditorCurveBinding() {
                                                                                path = "RootMotion", propertyName = "m_LocalPosition.y", type = typeof(Transform),
                                                                            },
                                                                            new EditorCurveBinding() {
                                                                                path = "RootMotion", propertyName = "m_LocalPosition.z", type = typeof(Transform),
                                                                            }
                                                                        };

        // -----------------------

        [MenuItem("Tools/Movable Utility/Preview Root Motion %m", false, 101)]
        public static void PreviewRootMotion() {
            OverrideRootMotionBindings(transformBindings, rootMotionBindings);
        }

        [MenuItem("Tools/Movable Utility/Apply Root Motion %#m", false, 102)]
        public static void ApplyRootMotion() {
            OverrideRootMotionBindings(rootMotionBindings, transformBindings);
        }

        private static void OverrideRootMotionBindings(EditorCurveBinding[] _overrideBindings, EditorCurveBinding[] _sourceBindings) {
            AnimationWindow _window = EditorWindow.GetWindow<AnimationWindow>();
            AnimationClip _clip = _window.animationClip;

            Undo.RecordObject(_clip, "animation root motion override");

            for (int _i = 0; _i < _sourceBindings.Length; _i++) {
                AnimationCurve _curve = AnimationUtility.GetEditorCurve(_clip, _sourceBindings[_i]);

                AnimationUtility.SetEditorCurve(_clip, _overrideBindings[_i], _curve);
                AnimationUtility.SetEditorCurve(_clip, _sourceBindings[_i], null);
            }
        }
        #endregion
    }
}
