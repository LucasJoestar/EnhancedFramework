// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor.Editor;
using EnhancedFramework.Core;
using UnityEditor;
using UnityEngine;

using AudioSettings = EnhancedFramework.Core.Settings.AudioSettings;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="AudioLayer"/> drawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(AudioLayer), true)]
    public sealed class AudioLayerPropertyDrawer : EnumPropertyDrawer {
        #region Drawer Content
        protected override void DrawPopup(Rect _position, SerializedProperty _property, EnumInfos _infos, GUIContent _label) {

            AudioSettings _settings = AudioEnhancedSettings.Resource.GetResource();

            for (int i = 0; i < _infos.Names.Length; i++) {
                GUIContent _name = _infos.Names[i];
                int _value = _infos.Values[i];

                if (_settings.AudioLayers.GetValue((AudioLayer)_value, out string _userName) && !string.IsNullOrEmpty(_userName)) {
                    _name.text = _userName;
                }
            }

            base.DrawPopup(_position, _property, _infos, _label);
        }
        #endregion
    }
}
