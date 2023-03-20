// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework-Conversations ===== //
// 
// Notes:
//
// ================================================================================================ //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;

using AudioAssetSettings = EnhancedFramework.Core.AudioAssetSettings;
using DisplayName = System.ComponentModel.DisplayNameAttribute;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// Plays an <see cref="AudioAsset"/> for the duration of the clip.
    /// </summary>
    [DisplayName("Audio/Audio Asset")]
    public class AudioAssetClip : AudioEnhancedPlayableAsset, ITimelineClipAsset {
        #region Global Members
        [Section("Audio Clip")]

        [Tooltip("Audio asset to play")]
        [Enhanced, Required] public AudioAsset Audio = null;

        [Tooltip("Override audio settings used to play this asset")]
        [Enhanced, ShowIf("OverrideSettings"), Required] public AudioAssetSettings Settings = null;

        [Space(5f)]

        [Tooltip("Whether to override or not the audio settings used to play this asset")]
        public bool OverrideSettings = false;

        // -----------------------

        /// <inheritdoc cref="PlayableAsset.duration"/>
        public override double duration {
            get {
                if (Audio == null) {
                    return base.duration;
                }

                return Audio.SampleDuration;
            }
        }

        /// <inheritdoc cref="PlayableAsset.outputs"/>
        public override IEnumerable<PlayableBinding> outputs {
            get { yield return AudioPlayableBinding.Create(name, this); }
        }

        /// <inheritdoc cref="ITimelineClipAsset.clipCaps"/>
        public ClipCaps clipCaps {
            get { return ClipCaps.ClipIn | ClipCaps.SpeedMultiplier | ClipCaps.Blending | ((Audio != null && Audio.Loop) ? ClipCaps.Looping : ClipCaps.None); }
        }
        #endregion

        #region Behaviour
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly Type propertiesType = typeof(TrackAsset).Assembly.GetType("UnityEngine.Timeline.AudioClipProperties");
        private static readonly MethodInfo setScriptInstanceMethod = typeof(PlayableHandle).GetMethod("SetScriptInstance", Flags);

        private static object[] setScritInstanceParameters = new object[0];

        // -----------------------

        public override Playable CreatePlayable(PlayableGraph _graph, GameObject _go) {
            if (Audio == null) {
                return Playable.Null;
            }

            // Audio Source setup.
            if (audioSource != null) {

                if (OverrideSettings) {
                    Audio.SetupAudioSource(audioSource, Settings);
                } else {
                    Audio.SetupAudioSource(audioSource);
                }
            }

            // To properly play an audio clip in Timeline, we use the built-in Audio Track which can access internal and other non accessible members.
            // Unfortunatly, it instantly throws an error if no AudioClipProperties is accessible in its playing clip, but the class itself has been made internal.
            // So in order to create an instance of the class and set it up, we use Reflection.

            if (setScritInstanceParameters.Length == 0) {

                object _properties = System.Activator.CreateInstance(propertiesType);
                setScritInstanceParameters = new object[] { _properties };
            }

            AudioClipPlayable _audioClipPlayable = AudioClipPlayable.Create(_graph, Audio.GetClip(), Audio.Loop);
            setScriptInstanceMethod.Invoke(_audioClipPlayable.GetHandle(), setScritInstanceParameters);

            return _audioClipPlayable;
        }
        #endregion

        #region Utility
        public override string ClipDefaultName {
            get { return "Audio Clip"; }
        }

        // -----------------------

        internal protected override void OnCreated(TimelineClip _clip) {
            base.OnCreated(_clip);

            OverrideSettings = Settings != null;

            // Default values.
            if (Audio == null) {
                return;
            }

            // Clip properties.
            _clip.duration      = Audio.Duration;
            _clip.displayName   = Audio.name.RemovePrefix();

            _clip.easeInDuration    = Audio.FadeInDuration;
            _clip.easeOutDuration   = Audio.FadeOutDuration;

            _clip.mixInCurve    = Audio.FadeInCurve;
            _clip.mixOutCurve   = Audio.FadeOutCurve;
        }
        #endregion
    }
}
