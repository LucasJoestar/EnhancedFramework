// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using System;
using UnityEditor;
using UnityEngine;

using AudioSettings = EnhancedFramework.Core.Settings.AudioSettings;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Audio-related <see cref="EnhancedSettings"/> data wrapper.//
    /// </summary>
    [Serializable]
    public sealed class AudioEnhancedSettings : EnhancedSettings {
        #region Global Members
        /// <summary>
        /// <see cref="AudioSettings"/>-related <see cref="AutoManagedResource{T}"/>.
        /// </summary>
        public static AutoManagedResource<AudioSettings> Resource = new AutoManagedResource<AudioSettings>($"{AudioSettings.MenuPrefix}{typeof(AudioSettings).Name}", false);

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="AudioEnhancedSettings"/>
        public AudioEnhancedSettings(int _guid) : base(_guid) { }
        #endregion

        #region Project Settings
        public const string UndoRecordTitle = "Enhanced Audio Settings change";

        public const string SettingsPath  = EnhancedEditorSettings.ProjectSettingsPath + "/Audio";
        public const string SettingsLabel = "Audio";

        public static readonly string[] PreferencesKeywords = new string[] {
                                                                "Enhanced",
                                                                "Framework",
                                                                "Audio",
                                                                "Layer",
                                                            };

        private static SerializedProperty audioProperty = null;

        // -----------------------

        [SettingsProvider]
        private static SettingsProvider CreateProjectSettingsProvider() {
            SettingsProvider _provider = new SettingsProvider(SettingsPath, SettingsScope.Project) {
                label       = SettingsLabel,
                keywords    = PreferencesKeywords,
                guiHandler  = DrawSettings,
            };

            return _provider;
        }

        private static void DrawSettings(string _searchContext) {
            if (audioProperty == null) {

                ScriptableObject _settings = Resource.GetResource();
                SerializedObject _object   = new SerializedObject(_settings);

                audioProperty = _object.GetIterator();
                audioProperty.NextVisible(true);
            }

            GUILayout.Space(10f);

            audioProperty.serializedObject.Update();

            SerializedProperty _property = audioProperty.Copy();

            using (var _changeCheck = new EditorGUI.ChangeCheckScope()) {

                // Draw each scriptable property.
                while (_property.NextVisible(false)) {
                    EnhancedEditorGUILayout.EnhancedPropertyField(_property);
                }

                audioProperty.serializedObject.ApplyModifiedProperties();
            }
        }
        #endregion
    }
}
