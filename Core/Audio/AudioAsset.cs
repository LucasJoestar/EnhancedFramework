// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;
using UnityEngine.Audio;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    public class AudioSettings {
        [SerializeField] private float multiplier = 1f;

        public float Multiplier {
            get { return multiplier; }
        }
    }

    /// <summary>
    /// <see cref="ScriptableObject"/> data holder for an audio asset.
    /// </summary>
    [CreateAssetMenu(fileName = "AD_AudioAsset", menuName = InternalUtility.MenuPath + "Audio", order = InternalUtility.MenuOrder)]
    public class AudioAsset : ScriptableObject {
        #region Global Members
        [Section("Audio Asset")]

        [Tooltip("Clips wrapped in this audio asset")]
        [SerializeField] private BlockArray<AudioClip> clips = new BlockArray<AudioClip>();

        [Space(5f)]

        [SerializeField] private AudioMixerGroup mixerGroup = null;
        [SerializeField] private AudioSettings settings = null;

        [Space(10f)]

        [SerializeField, Enhanced, DisplayName("Music")] private bool isMusic   = false;
        [SerializeField, Enhanced, DisplayName("Loop")] private bool isLooping  = false;

        [Tooltip("If true, ensures that only one instance is playing at a time")]
        [SerializeField] private bool avoidDuplicate = false;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, Range(0f, 256f)] private int priority        = 128;
        [SerializeField, Enhanced, Range(0f, 1f)] private float volume          = 1f;
        [SerializeField, Enhanced, Range(0f, 1.1f)] private float reverbZoneMix = .5f;
        [SerializeField, Enhanced, MinMax(-3, 3f)] private Vector2 pitch        = Vector2.one;

        [Space(5f)]

        [SerializeField, Enhanced, Range(0f, 1f)] private float fadeIn = 0f;
        [SerializeField, Enhanced, Range(0f, 1f)] private float fadeOut = 0f;

        [Space(5f)]

        [SerializeField] private bool useStartRange = false;
        [SerializeField, Enhanced, ShowIf("useStartRange"), MinMax("Range")] private Vector2 startRange = Vector2.zero;

        // -----------------------

        /// <summary>
        /// Volume of this audio.
        /// </summary>
        public float Volume {
            get { return volume * settings.Multiplier; }
        }

        /// <summary>
        /// Random pitch of this audio.
        /// </summary>
        public float Pitch {
            get { return pitch.Random(); }
        }

        /// <summary>
        /// Minimum duration of this audio, in second(s).
        /// </summary>
        public float Duration {
            get {
                float _duration = 0f;

                foreach (AudioClip _clip in clips) {
                    if (_clip != null) {
                        _duration = (_duration == 0f)
                                  ? _clip.length
                                  : Mathf.Min(_duration, _clip.length);
                    }
                }

                return _duration;
            }
        }

        /// <summary>
        /// Play range of this asset (between 0 and is duration).
        /// </summary>
        public Vector2 Range {
            get { return new Vector2(0f, Duration); }
        }
        #endregion

        #region Behaviour
        public void Play() {

        }

        public void Pause() {

        }

        public void Stop() {

        }
        #endregion

        #region TO ENHANCE
        public void SetupAudioSource(AudioSource source, AudioSettings overrideSettings) {
            source.loop = isLooping;
            source.priority = priority;
            source.volume = Volume;
            source.pitch = Pitch;
            source.reverbZoneMix = reverbZoneMix;
            source.outputAudioMixerGroup = mixerGroup;

            /*if (overrideSettings != null) {
                overrideSettings.SetupAudioSource(source);
            } else {
                settings.SetupAudioSource(source);
            }*/
        }
        #endregion

        #region Utility
        private const int RandomClipMaxIteration = 5;
        private int lastClipIndex = -1;

        // -----------------------

        /// <summary>
        /// Get a random clip from this audio.
        /// </summary>
        public AudioClip GetClip() {
            int _iteration = RandomClipMaxIteration;
            int _index;

            do {
                _index = Random.Range(0, clips.Count);
            } while ((clips.Count > 1) && (_index == lastClipIndex) && (_iteration-- != RandomClipMaxIteration));

            lastClipIndex = _index;
            return clips[_index];
        }
        #endregion
    }
}
