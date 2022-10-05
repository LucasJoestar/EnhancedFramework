// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="CrossSceneReference{T}"/> drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(CrossSceneReference<>), true)]
    public class CrossSceneReferencePropertyDrawer : EnhancedPropertyEditor {
        #region Drawer Content
        private const int CacheLimit = 25;

        private static readonly GUIContent lockGUI = new GUIContent(EditorGUIUtility.IconContent("IN LockButton on").image, "Toggles the reference GUID edit mode.");
        private static readonly GUIContent unlockGUI = new GUIContent(EditorGUIUtility.IconContent("IN LockButton").image, "Toggles the reference GUID edit mode.");

        private static readonly GUIContent notLoadedGUI = new GUIContent(EditorGUIUtility.IconContent("console.infoicon.sml").image, "The reference object scene is currently not loaded.");
        private static readonly GUIContent missingGUI = new GUIContent(EditorGUIUtility.IconContent("console.erroricon.sml").image, "The reference object has been destroyed or could not be found.");

        private static readonly Dictionary<int, CrossSceneObject> references = new Dictionary<int, CrossSceneObject>();

        // -----------------------

        protected override float OnEnhancedGUI(Rect _position, SerializedProperty _property, GUIContent _label) {
            // Variable init.
            Rect _propertyPosition = new Rect(_position) {
                width = _position.width - (EnhancedEditorGUIUtility.IconWidth + 2f)
            };

            Rect _buttonPosition = new Rect(_position) {
                xMin = _position.xMax - EnhancedEditorGUIUtility.IconWidth
            };

            SerializedProperty _guidProperty = _property.FindPropertyRelative("GUID");
            SerializedProperty _sceneGUIDProperty = _property.FindPropertyRelative("sceneGUID");
            SerializedProperty _isSceneLoadedProperty = _property.FindPropertyRelative("isSceneLoaded");

            bool _isLock = _property.isExpanded;
            int _guid = _guidProperty.intValue;
            string _sceneGUID = _sceneGUIDProperty.stringValue;

            // Cache limit.
            if (references.Count > CacheLimit) {
                references.Clear();
            }

            // Cache the reference object for performance.
            if (!references.TryGetValue(_guid, out CrossSceneObject _reference)) {
                _reference = null;
                references.Add(_guid, _reference);
            }

            bool _isReference = _reference != null;

            if (!_isReference && !string.IsNullOrEmpty(_sceneGUID)) {
                Scene _scene = SceneManager.GetSceneByPath(AssetDatabase.GUIDToAssetPath(_sceneGUID));
                bool _wasLoaded = _isSceneLoadedProperty.boolValue;
                bool _isLoaded = _scene.isLoaded;

                if (_isLoaded) {
                    CrossSceneObject[] _objects = Object.FindObjectsOfType<CrossSceneObject>(true);

                    foreach (CrossSceneObject _object in _objects) {
                        if (_object.GUID == _guidProperty.intValue) {
                            // Found.
                            references[_guid] = _reference = _object;

                            _isReference = true;
                            _isLock = _property.isExpanded = false;
                            break;
                        }
                    }

                    // If the reference object is missing, clear its associated scene info.
                    if (!_isReference) {
                        _sceneGUIDProperty.stringValue = string.Empty;
                        _isLock = _property.isExpanded = true;

                        GUI.changed = true;
                    }
                } else if (_wasLoaded) {
                    _isLock = _property.isExpanded = true;
                }

                if (_wasLoaded != _isLoaded) {
                    _isSceneLoadedProperty.boolValue = _isLoaded;
                    GUI.changed = true;
                }
            }

            // Get the editing field generic type as the component reference type.
            Type _type = fieldInfo.FieldType;
            while (!_type.IsGenericType) {
                _type = _type.BaseType;
            }

            _type = _type.GetGenericArguments()[0];

            using (var _scope = new EditorGUI.PropertyScope(_position, _label, _property)) {
                // To avoid the prefix label being affected by the GUIEnabled scope below,
                // draw an empty field outside of the scope.
                _propertyPosition = EditorGUI.PrefixLabel(_propertyPosition, _label);
                EditorGUI.LabelField(_propertyPosition, GUIContent.none);

                if (_isLock && !_isReference && (_guid != 0)) {
                    // When the reference could not be found, display an informative help box.
                    GUIContent _gui;

                    if (string.IsNullOrEmpty(_sceneGUID)) {
                        _gui = missingGUI;
                        _gui.text = $"Missing Reference ({_type.Name})";
                    } else {
                        _gui = notLoadedGUI;
                        _gui.text = $"Not Loaded ({_type.Name})";
                    }

                    EditorGUI.LabelField(_propertyPosition, GUIContent.none, EditorStyles.helpBox);

                    _propertyPosition.xMin += 1f;
                    _propertyPosition.y += 1f;
                    _propertyPosition.height -= 2f;

                    EditorGUI.LabelField(_propertyPosition, _gui);
                } else {
                    // Object reference field.
                    using (var _enabled = EnhancedGUI.GUIEnabled.Scope(!_isLock))
                    using (var _changeCheck = new EditorGUI.ChangeCheckScope()) {
                        Component _component = EditorGUI.ObjectField(_propertyPosition, GUIContent.none, _reference, _type, true) as Component;

                        if (_changeCheck.changed) {
                            if (_component == null) {
                                // Null reference.
                                references[_guid] = null;
                                _guidProperty.intValue = 0;
                                _sceneGUIDProperty.stringValue = string.Empty;
                            } else {
                                // Valid reference.
                                GameObject _object = _component.gameObject;
                                references[_guid] = _reference = _object.AddComponentIfNone<CrossSceneObject>();
                                EditorUtility.SetDirty(_object);

                                _guidProperty.intValue = _reference.GUID;
                                _sceneGUIDProperty.stringValue = AssetDatabase.AssetPathToGUID(_object.scene.path);
                            }
                        }
                    }
                }
            }

            // Lock toggle.
            if (EnhancedEditorGUI.IconButton(_buttonPosition, _isLock ? lockGUI : unlockGUI)) {
                _property.isExpanded = !_isLock;
            }

            return _position.height;
        }
        #endregion
    }
}
