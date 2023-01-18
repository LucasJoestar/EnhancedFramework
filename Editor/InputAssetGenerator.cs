// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if ENABLE_INPUT_SYSTEM
#define NEW_INPUT_SYSTEM
#endif

#if NEW_INPUT_SYSTEM
using EnhancedEditor.Editor;
using EnhancedFramework.Inputs;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

using UnityInputActionAsset = UnityEngine.InputSystem.InputActionAsset;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Generates an <see cref="SingleInputActionEnhancedAsset"/> for each action setup in the first <see cref="UnityEngine.InputSystem.InputActionAsset"/> in the project.
    /// </summary>
    public static class InputAssetGenerator {
        #region Content
        private const string DefaultInputAssetPath      = "Assets/Input Assets/";
        private const string InputMapAssetFormat        = InputMapEnhancedAsset.FilePrefix + "{0}";
        private const string InputActionAssetFormat     = SingleInputActionEnhancedAsset.FilePrefix + "{0}_{1}";

        // -----------------------

        [MenuItem(FrameworkUtility.MenuItemPath + "Refresh Input Assets", false, 20)]
        private static void RefreshInputAssets() {
            if (!EnhancedEditorUtility.LoadMainAsset(out UnityInputActionAsset _inputAsset)){
                return;
            }

            // Get new action assets path.
            List<SingleInputActionEnhancedAsset> _inputs = new List<SingleInputActionEnhancedAsset>(EnhancedEditorUtility.LoadAssets<SingleInputActionEnhancedAsset>());
            List<SingleInputActionEnhancedAsset> _inputList = new List<SingleInputActionEnhancedAsset>();

            string _path = GetAssetPath(_inputs);

            // Database.
            if (!EnhancedEditorUtility.LoadMainAsset(out InputDatabase _database)) {
                _database = ScriptableObject.CreateInstance<InputDatabase>();
                _database.ActionDatabase = _inputAsset;

                AssetDatabase.CreateAsset(_database, Path.Combine(_path, $"{InputDatabase.FileName}.asset"));
            }

            // Create new action assets and rename wrong named existing ones.
            foreach (InputAction _action in _inputAsset) {
                SingleInputActionEnhancedAsset _asset = _inputs.Find(i => i.input.id == _action.id);
                string _name = string.Format(InputActionAssetFormat, _action.actionMap.name, _action.name).Replace('/', '.');

                if (_asset) {
                    if (_asset.name != _name) {
                        string _assetPath = AssetDatabase.GetAssetPath(_asset);
                        AssetDatabase.RenameAsset(_assetPath, _name);
                    }

                    _inputs.Remove(_asset);
                } else {
                    _asset = ScriptableObject.CreateInstance<SingleInputActionEnhancedAsset>();
                    _asset.Initialize(_action);

                    AssetDatabase.CreateAsset(_asset, Path.Combine(_path, $"{_name}.asset"));
                }

                _inputList.Add(_asset);
            }

            // Do the same for action maps.
            List<InputMapEnhancedAsset> _maps = new List<InputMapEnhancedAsset>(EnhancedEditorUtility.LoadAssets<InputMapEnhancedAsset>());

            _path = GetAssetPath(_maps);

            foreach (InputActionMap _map in _inputAsset.actionMaps) {
                InputMapEnhancedAsset _asset = _maps.Find(i => i.map.id == _map.id);
                string _name = string.Format(InputMapAssetFormat, _map.name.Replace('/', '.'));

                if (_asset) {
                    if (_asset.name != _name) {
                        string _assetPath = AssetDatabase.GetAssetPath(_asset);
                        AssetDatabase.RenameAsset(_assetPath, _name);
                    }

                    _maps.Remove(_asset);
                } else {
                    _asset = ScriptableObject.CreateInstance<InputMapEnhancedAsset>();
                    _asset.Initialize(_map);

                    AssetDatabase.CreateAsset(_asset, Path.Combine(_path, $"{_name}.asset"));
                }

                // Map setup.
                for (int i = _inputList.Count; i-- > 0;) {
                    if (_inputList[i].SetMap(_asset)) {
                        _inputList.RemoveAt(i);
                    }
                }
            }

            // Delete obsolete assets.
            foreach (SingleInputActionEnhancedAsset _input in _inputs) {
                if (_input.isOrphan) {
                    continue;
                }

                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_input));
            }

            foreach (InputMapEnhancedAsset _map in _maps) {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_map));
            }

            AssetDatabase.Refresh();

            // ----- Local Method ----- \\

            string GetAssetPath<T>(List<T> _objects) where T : ScriptableObject {
                string _assetPath;

                if (_objects.Count == 0) {
                    _assetPath = DefaultInputAssetPath;

                    if (!Directory.Exists(_assetPath)) {
                        Directory.CreateDirectory(_assetPath);
                    }
                } else {
                    _assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_objects[0]));
                }

                return _assetPath;
            }
        }
        #endregion
    }
}
#endif
