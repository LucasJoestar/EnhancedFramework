// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;

using ArrayUtility = EnhancedEditor.ArrayUtility;
using Object = UnityEngine.Object;
#endif

namespace EnhancedFramework.Core.Settings {
    #if UNITY_EDITOR
    /// <summary>
    /// Implements the interface on another class to avoid Unity creating a new instance of the console window.
    /// </summary>
    [InitializeOnLoad]
    internal class GameSettingsProcessor : IPreprocessBuildWithReport {
        #region Global Members
        int IOrderedCallback.callbackOrder {
            get { return 99; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        static GameSettingsProcessor() {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        #endregion

        #region Callbacks
        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport _report) {

            BuildTargetGroup _target = _report.summary.platformGroup;
            RegisterScriptingSymbols(_target);
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange _playMode) {

            switch (_playMode) {

                // Ignore.
                case PlayModeStateChange.EnteredEditMode:
                case PlayModeStateChange.EnteredPlayMode:
                case PlayModeStateChange.ExitingPlayMode:
                default:
                    return;

                case PlayModeStateChange.ExitingEditMode:
                    break;
            }

            BuildTargetGroup _target = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            RegisterScriptingSymbols(_target);
        }

        /// <summary>
        /// Registers all active defined scripting symbols for a group in the <see cref="GameSettings"/>.
        /// </summary>
        /// <param name="_target">Group to register the associated symbols.</param>
        private static void RegisterScriptingSymbols(BuildTargetGroup _target) {

            string[] _guids = AssetDatabase.FindAssets($"t:{typeof(GameSettings).Name}");

            // No settings.
            if (_guids.Length == 0) {
                return;
            }

            GameSettings _settings = AssetDatabase.LoadAssetAtPath<GameSettings>(AssetDatabase.GUIDToAssetPath(_guids[0]));
            PlayerSettings.GetScriptingDefineSymbolsForGroup(_target, out string[] _defines);

            _settings.scriptingSymbols = _defines;
            EditorUtility.SetDirty(_settings);
        }
        #endregion
    }
    #endif

    /// <summary>
    /// Global game settings, referencing each and every <see cref="BaseSettings{T}"/> shared across the game.
    /// </summary>
    [CreateAssetMenu(fileName = "GGS_GameSettings", menuName = MenuPath + "GameSettings", order = MenuOrder - 50)]
    public class GameSettings : BaseSettings<GameSettings> {
        #region Global Members
        [Section("Game Settings")]

        [SerializeField] protected BaseSettings[] settings = new BaseSettings[] { };

        [Space(10f)]

        [SerializeField, Enhanced, Required] protected BuildSceneDatabase buildSceneDatabase    = null;
        [SerializeField, Enhanced, Required] protected FlagDatabase flagDatabase                = null;
        [SerializeField, Enhanced, Required] protected TagDatabase tagDatabase                  = null;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("All scripting symbols currently defined in the game")]
        [SerializeField, Enhanced, ReadOnly] internal string[] scriptingSymbols = new string[0];
        #endregion

        #region Initialization
        internal protected override void Init() {
            base.Init();

            // Settings and databases assignement.
            foreach (BaseSettings _settings in settings) {
                _settings.Init();
            }

            BuildSceneDatabase.Database = buildSceneDatabase;
            FlagDatabase.Database = flagDatabase;
            MultiTags.Database = tagDatabase;
        }
        #endregion

        #region Scripting Symbols
        /// <summary>
        /// Get all defined scripting symbols.
        /// </summary>
        /// <returns>An array of all defined scripting symbols.</returns>
        public string[] GetScriptingSymbols() {
            return scriptingSymbols;
        }

        /// <summary>
        /// Get if a specific scripting symbol is defined.
        /// </summary>
        /// <param name="_symbol">Scripting symbol to check definition.</param>
        /// <returns>True if the given symbol is defined, false otherwise.</returns>
        public bool IsScriptingSymbolDefined(string _symbol) {
            return ArrayUtility.Contains(scriptingSymbols, _symbol);
        }
        #endregion

        #region Editor Tool
        /// <summary>
        /// Editor utility, retrieving all inputs assets from the project.
        /// </summary>
        [Button(ActivationMode.Editor, SuperColor.Green, IsDrawnOnTop = false)]
        private void Setup() {
            #if UNITY_EDITOR

            // Databases.
            if (!buildSceneDatabase) {
                buildSceneDatabase = LoadAsset<BuildSceneDatabase>();
            }

            if (!flagDatabase) {
                flagDatabase = LoadAsset<FlagDatabase>();
            }

            if (!tagDatabase) {
                tagDatabase = LoadAsset<TagDatabase>();
            }

            // Settings.
            var _types = TypeCache.GetTypesDerivedFrom<BaseSettings>();
            ArrayUtility.RemoveNulls(ref settings);

            foreach (Type _type in _types) {

                if (_type.IsAbstract || _type.IsGenericType || (_type == GetType())) {
                    continue;
                }

                if (Array.Find(settings, s => s.GetType() == _type)) {
                    continue;
                }

                Object _asset = LoadObject(_type);
                if (_asset == null) {

                    // Create setting.
                    string _prefix = _type.IsSubclassOfGeneric(typeof(BaseDatabase<>)) ? "DT_" : "GS_";
                    string _path = Path.Combine(GetProjectFolder(), $"{_prefix}{_type.Name}.asset");

                    _path = AssetDatabase.GenerateUniqueAssetPath(_path);
                    _asset = CreateInstance(_type);

                    AssetDatabase.CreateAsset(_asset, _path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                ArrayUtility.Add(ref settings, _asset as BaseSettings);
            }

            EditorUtility.SetDirty(this);

            // ----- Local Method ----- \\

            T LoadAsset<T>() where T : Object {
                return LoadObject(typeof(T)) as T;
            }

            Object LoadObject(Type _type) {
                if (AssetDatabase.FindAssets($"t:{_type.Name}").SafeFirst(out string _path)) {
                    return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(_path), _type);
                }

                return null;
            }

            string GetProjectFolder() {

                foreach (Object _object in Selection.GetFiltered<Object>(SelectionMode.Assets)) {
                    string _path = AssetDatabase.GetAssetPath(_object);

                    if (string.IsNullOrEmpty(_path)) {
                        continue;
                    }

                    if (Directory.Exists(_path)) {
                        return _path;
                    }
                    
                    if (File.Exists(_path)) {
                        return Path.GetDirectoryName(_path);
                    }
                }

                return UnityEditorInternal.InternalEditorUtility.GetAssetsFolder();
            }
            #endif
        }
        #endregion
    }
}
