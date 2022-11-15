// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
using EnhancedEditor.Editor;
using EnhancedFramework.Input;
using EnhancedFramework.Core;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

using InputAsset = EnhancedFramework.Input.InputAsset;
using UnityInputActionAsset = UnityEngine.InputSystem.InputActionAsset;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Generates an <see cref="InputAsset"/> for each action setup in the first <see cref="UnityEngine.InputSystem.InputActionAsset"/> in the project.
    /// </summary>
    public static class InputAssetGenerator {
        #region Content
        private const string DefaultInputAssetPath = "Assets/Input Assets/";
        private const string InputActionAssetFormat = "INPACT_{0}_{1}";
        private const string InputActionMapAssetFormat = "INPMAP_{0}";

        // -----------------------

        [MenuItem(InternalUtility.MenuItemPath + "Refresh Input Assets", false, 20)]
        private static void RefreshInputAssets() {
            if (!EnhancedEditorUtility.LoadMainAsset(out UnityInputActionAsset _inputAsset)){
                return;
            }

            // Get new action assets path.
            List<InputAsset> _inputs = new List<InputAsset>(EnhancedEditorUtility.LoadAssets<InputAsset>());
            List<InputAsset> _inputList = new List<InputAsset>();

            string _path = GetAssetPath(_inputs);

            // Create new action assets and rename wrong named existing ones.
            foreach (InputAction _action in _inputAsset) {
                InputAsset _asset = _inputs.Find(i => i.Input.id == _action.id);
                string _name = string.Format(InputActionAssetFormat, _action.actionMap.name, _action.name);

                if (_asset) {
                    if (_asset.name != _name) {
                        string _assetPath = AssetDatabase.GetAssetPath(_asset);
                        AssetDatabase.RenameAsset(_assetPath, _name);
                    }

                    _inputs.Remove(_asset);
                } else {
                    _asset = ScriptableObject.CreateInstance<InputAsset>();
                    _asset.Initialize(_action);

                    AssetDatabase.CreateAsset(_asset, Path.Combine(_path, $"{_name}.asset"));
                }

                _inputList.Add(_asset);
            }

            // Do the same for action maps.
            List<InputMapAsset> _maps = new List<InputMapAsset>(EnhancedEditorUtility.LoadAssets<InputMapAsset>());

            _path = GetAssetPath(_maps);

            foreach (InputActionMap _map in _inputAsset.actionMaps) {
                InputMapAsset _asset = _maps.Find(i => i.Map.id == _map.id);
                string _name = string.Format(InputActionMapAssetFormat, _map.name);

                if (_asset) {
                    if (_asset.name != _name) {
                        string _assetPath = AssetDatabase.GetAssetPath(_asset);
                        AssetDatabase.RenameAsset(_assetPath, _name);
                    }

                    _maps.Remove(_asset);
                } else {
                    _asset = ScriptableObject.CreateInstance<InputMapAsset>();
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
            foreach (InputAsset _input in _inputs) {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_input));
            }

            foreach (InputMapAsset _map in _maps) {
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
