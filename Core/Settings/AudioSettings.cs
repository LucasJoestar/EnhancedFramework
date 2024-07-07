// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace EnhancedFramework.Core.Settings {
    /// <summary>
    /// Audio related global settings.
    /// </summary>
    [CreateAssetMenu(fileName = MenuPrefix + "AudioSettings", menuName = MenuPath + "Audio", order = MenuOrder)]
    public sealed class AudioSettings : BaseSettings<AudioSettings> {
        #region Global Members
        [Section("Audio Settings")]

        [Tooltip("Displayed name of each audio layer")]
        public EnumValues<AudioLayer, string> AudioLayers = new EnumValues<AudioLayer, string>();
        #endregion

        #region Utility
        /// <summary>
        /// Converts a layer name to its associated <see cref="AudioLayer"/>.
        /// </summary>
        /// <param name="_name">Name to get the associated <see cref="AudioLayer"/>.</param>
        /// <param name="_layer"> <see cref="AudioLayer"/> associated with the given name (default if none).</param>
        /// <returns>True if the given name could be successfully converted, false otherwise.</returns>
        public bool NameToAudioLayer(string _name, out AudioLayer _layer) {

            if (!AudioLayers.GetEnum(_name, out int _enumValue)) {
                return Enum.TryParse(_name, out _layer);
            }

            _layer = (AudioLayer)_enumValue;
            return true;
        }

        /// <summary>
        /// Converts a given <see cref="AudioLayer"/> to its pre-configured name.
        /// </summary>
        /// <param name="_layer"><see cref="AudioLayer"/> to get the associated name.</param>
        /// <returns>Name associated with the given layer.</returns>
        public string AudioLayerToName(AudioLayer _layer) {

            if (!AudioLayers.GetValue(_layer, out string _name) || string.IsNullOrEmpty(_name)) {
                _name = _layer.ToString();
            }

            return _name;
        }
        #endregion
    }
}
