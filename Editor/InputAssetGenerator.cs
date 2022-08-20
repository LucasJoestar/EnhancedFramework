// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using EnhancedFramework.Input;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Generates an <see cref="InputAsset"/> for each action setup in the first <see cref="InputActionAsset"/> in the project.
    /// </summary>
    public static class InputAssetGenerator {
        #region Content
        private const string DefaultInputAssetPath = "Assets/Input Assets/";
        private const string AssetFormat = "INPT_{0}_{1}";

        // -----------------------

        [MenuItem(InternalUtility.MenuItemPath + "Refresh Input Assets", false, 20)]
        private static void RefreshInputAssets() {
            if (!EnhancedEditorUtility.LoadMainAsset(out InputActionAsset _map)){
                return;
            }

            // Get new assets path.
            string _path;
            List<InputAsset> _inputs = new List<InputAsset>(EnhancedEditorUtility.LoadAssets<InputAsset>());

            if (_inputs.Count == 0) {
                _path = DefaultInputAssetPath;

                if (!Directory.Exists(_path)) {
                    Directory.CreateDirectory(_path);
                }
            } else {
                _path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(_inputs[0]));
            }

            // Create new assets and rename wrong named existing ones.
            foreach (InputAction _action in _map) {
                InputAsset _asset = _inputs.Find(i => i.Action.id == _action.id);
                string _name = string.Format(AssetFormat, _action.actionMap.name, _action.name);

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
            }

            // Delete obsolete assets.
            foreach (InputAsset _input in _inputs) {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(_input));
            }

            AssetDatabase.Refresh();
        }
        #endregion
    }
}
