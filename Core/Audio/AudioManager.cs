// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using Object = UnityEngine.Object;
using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// 
    /// </summary>
    public class AudioManager : EnhancedSingleton<AudioManager> {
        #region Global Members
        [Section("Audio Manager")]

        [SerializeField, Enhanced, Required] private AudioListener listener = null;
        [SerializeField, Enhanced, Required] private AudioMixer mixer       = null;
        #endregion

        #region Enhanced Behaviour

        #endregion

        #region Music
        private AudioPlayer musicPlayer = null;

        // -----------------------

        public void PlayMusic(AudioAsset _asset) {
            musicPlayer.Play(_asset);
        }
        #endregion
    }
}
