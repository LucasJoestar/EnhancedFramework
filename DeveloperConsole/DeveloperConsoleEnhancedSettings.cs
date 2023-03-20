// ===== Enhanced Editor - https://github.com/LucasJoestar/EnhancedEditor ===== //
// 
// Notes:
//
// ============================================================================ //

#if UNITY_EDITOR
#define EDITOR_BEHAVIOUR
#endif

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

#if EDITOR_BEHAVIOUR
using UnityEditor;
#endif

namespace EnhancedFramework.DeveloperConsoleSystem {
    /// <summary>
    /// <see cref="DeveloperConsole"/>-related settings.
    /// <para/>
    /// Cannot inherit from EnhancedSettings, as it needs to be accessible at runtime.
    /// </summary>
    [Serializable]
    public class DeveloperConsoleSettings : ScriptableObject {
        #region Global Members
        /// <summary>
        /// Determines the Unity console log types to be displayed in the developer console.
        /// </summary>
        [Tooltip("Determines the Unity console log types to be displayed in the developer console")]
        public FlagLogType DisplayedLogs = ~FlagLogType.None;

        [Space(10f)]

        /// <summary>
        /// All base included using statements.
        /// </summary>
        [Tooltip("All base included using statements")]
        public Set<string> IncludedUsing = new Set<string>() {
            "EnhancedEditor",
            "EnhancedFramework.Core",
            "EnhancedFramework.Core.GameStates",
            "EnhancedFramework.DeveloperConsoleSystem",
            "System",
            "System.Linq",
            "UnityEngine",
            "UnityEngine.SceneManagement",
            "UnityEngine.UI"
        };

        [Space(10f, order = -3), Title("Command Bindings", order = -2), Space(3f, order = -1)]

        /// <summary>
        /// Whether developer console bindings or currently enabled or not.
        /// </summary>
        [Tooltip("Whether developer console bindings or currently enabled or not")]
        public bool EnableBindings = true;

        /// <summary>
        /// All registered bindings of the developer console.
        /// </summary>
        [Tooltip("All registered bindings of the developer console")]
        public EnhancedCollection<DeveloperConsoleBinding> Bindings = new EnhancedCollection<DeveloperConsoleBinding>();
        #endregion

        #region Settings
        public const string UndoRecordTitle     = "Developer Console Preferences change";
        public const string PreferencesKey      = "EnhancedFramework.DeveloperConsole";

        public const string PreferencesPath     = "Preferences/Enhanced Engine/Developer Console";
        public const string PreferencesLabel    = "Developer Console";

        public static readonly string[] PreferencesKeywords = new string[] {
                                                                "Enhanced",
                                                                "Framework",
                                                                "Engine",
                                                                "Developer",
                                                                "Console",
                                                                "Log",
                                                            };


        private static DeveloperConsoleSettings settings = null;

        #if EDITOR_BEHAVIOUR
        private static SerializedProperty settingsProperty = null;
        #endif

        /// <inheritdoc cref="DeveloperConsoleSettings"/>
        public static DeveloperConsoleSettings Settings {
            get {
                if (settings == null) {
                    settings = CreateInstance<DeveloperConsoleSettings>();
                    string _json = PlayerPrefs.GetString(PreferencesKey, string.Empty);

                    if (!string.IsNullOrEmpty(_json)) {
                        JsonUtility.FromJsonOverwrite(_json, settings);
                    }
                }

                #if EDITOR_BEHAVIOUR
                if ((settingsProperty == null) || (settingsProperty.serializedObject.targetObject == null)) {
                    settingsProperty = new SerializedObject(settings).GetIterator();
                    settingsProperty.NextVisible(true);
                }
                #endif

                return settings;
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        /// <summary>
        /// Saves these settings.
        /// </summary>
        public void Save() {
            string _json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(PreferencesKey, _json);
        }

        // -------------------------------------------
        // Editor
        // -------------------------------------------

        #if EDITOR_BEHAVIOUR
        public static EditorWindow OpenUserSettings() {
            EditorWindow _preferences = SettingsService.OpenUserPreferences(PreferencesPath);
            return _preferences;
        }

        [SettingsProvider]
        private static SettingsProvider CreateUserSettingsProvider() {
            SettingsProvider _provider = new SettingsProvider(PreferencesPath, SettingsScope.User) {
                label = PreferencesLabel,
                keywords = PreferencesKeywords,
                guiHandler = DrawSettings,
            };

            return _provider;
        }

        private static void DrawSettings(string _searchContext) {
            GUILayout.Space(10f);

            DeveloperConsoleSettings _setting = Settings;
            settingsProperty.serializedObject.Update();

            SerializedProperty _property = settingsProperty.Copy();

            using (var _scope = new GUILayout.HorizontalScope()) {
                GUILayout.Space(15f);

                using (var _verticalScope = new GUILayout.VerticalScope())
                using (var _changeCheck = new EditorGUI.ChangeCheckScope()) {

                    // Draw each scriptable property.
                    while (_property.NextVisible(false)) {
                        EditorGUILayout.PropertyField(_property);
                    }

                    settingsProperty.serializedObject.ApplyModifiedProperties();

                    // Save on change.
                    if (_changeCheck.changed) {
                        _setting.Save();
                    }
                }
            }
        }
        #endif
        #endregion
    }
}
