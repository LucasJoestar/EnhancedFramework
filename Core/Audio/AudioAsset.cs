// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;
using UnityEngine.Audio;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="ScriptableObject"/> data holder for an audio asset.
    /// </summary>
    [CreateAssetMenu(fileName = "ADA_AudioAsset", menuName = FrameworkUtility.MenuPath + "Audio/Audio Asset", order = FrameworkUtility.MenuOrder)]
    public class AudioAsset : EnhancedAssetFeedback {
        #region Global Members
        [Section("Audio Asset")]

        [Tooltip("Clips wrapped in this audio asset")]
        [SerializeField] private BlockArray<AudioClip> clips = new BlockArray<AudioClip>();

        [Space(5f)]

        [Tooltip("Mixer group to plug this audio in")]
        [SerializeField] private AudioMixerGroup mixerGroup = null;

        [Tooltip("Settings used to play this audio")]
        [SerializeField] private AudioAssetSettings settings = null;

        [Space(10f)]

        [Tooltip("If true, automatically loops when reaching the end")]
        [SerializeField, Enhanced, DisplayName("Loop")] private bool isLooping  = false;

        [Space(5f)]

        [Tooltip("If true, keeps this audio playing while performing a scene loading operation")]
        [SerializeField, Enhanced, DisplayName("Persistent")] private bool isPersistent = false;

        [Tooltip("If true, ensures that only one instance is playing at a time")]
        [SerializeField] private bool avoidDuplicate = false;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Priority of this audio. Note that a sound with a high priority is more likely to be stolen by sounds with a lower priority")]
        [SerializeField, Enhanced, Range(0f, 256f)] private int priority        = 128;

        [Tooltip("Overall default volume of this audio")]
        [SerializeField, Enhanced, Range(0f, 1f)] private float volume          = 1f;

        [Tooltip("Frequency range of this audio; use this to slow down or speed it up")]
        [SerializeField, Enhanced, MinMax(-3, 3f)] private Vector2 pitch        = Vector2.one;

        [Tooltip("Determines how much this audio is affected by 3D spatialisation calculations (attenuation, doppler etc); 0.0 makes it full 2D, 1.0 makes it full 3D")]
        [SerializeField, Enhanced, Range(0f, 1f)] private float spatialBlend    = 1f;

        [Tooltip("Determines how much of the signal this audio is mixing into the global reverb associated with zones." +
                 "[0, 1] is a linear range (like volume), while [1, 1.1] lets you boost the reverb mix by 10 dB")]
        [SerializeField, Enhanced, Range(0f, 1.1f)] private float reverbZoneMix = .5f;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Duration used for fading in this audio (use 0 for instant)")]
        [SerializeField, Enhanced, Range(0f, 5f)] private float fadeInDuration = 0f;

        [Tooltip("Duration used for fading out this audio (use 0 for instant)")]
        [SerializeField, Enhanced, Range(0f, 5f)] private float fadeOutDuration = 0f;

        [Space(5f)]

        [Tooltip("Curve used for fading in this audio")]
        [SerializeField, Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Green)]
        private AnimationCurve fadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Tooltip("Curve used for fading out this audio")]
        [SerializeField, Enhanced, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Crimson)]
        private AnimationCurve fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        [Space(10f)]

        [Tooltip("If true, use a random range where to start playing this audio")]
        [SerializeField, Enhanced, DisplayName("Start Range")] private bool useStartRange = false;

        [Tooltip("Start range used to play this audio at")]
        [SerializeField, Enhanced, ShowIf("useStartRange"), MinMax("Range")] private Vector2 startRange = Vector2.zero;

        // -----------------------

        /// <summary>
        /// Volume of this audio.
        /// </summary>
        public float Volume {
            get { return volume * settings.VolumeMultiplier; }
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
        /// Maximum sample duration of this audio, in second(s).
        /// </summary>
        public float SampleDuration {
            get {
                if (clips.Count == 0) {
                    return 0f;
                }

                AudioClip _audio = null;

                foreach (AudioClip _clip in clips) {
                    if ((_clip != null) && (_audio.IsNull() || (_audio.length < _clip.length))) {
                        _audio = _clip;
                    }
                }

                return _audio.samples / _audio.frequency;
            }
        }

        /// <summary>
        /// Whether to make the audio looping or not.
        /// </summary>
        public bool Loop {
            get { return isLooping; }
        }

        /// <summary>
        /// Play range of this asset (between 0 and is duration).
        /// </summary>
        public Vector2 Range {
            get { return new Vector2(0f, Duration); }
        }

        /// <summary>
        /// Determines if this audio should keeps playing while performing a scene loading operation.
        /// </summary>
        public bool IsPersistent {
            get { return isPersistent; }
        }

        /// <summary>
        /// Determines if only one instance of this audio should be played at a time.
        /// </summary>
        public bool AvoidDuplicate {
            get { return avoidDuplicate; }
        }

        /// <summary>
        /// Default settings used to play this audio.
        /// </summary>
        public AudioAssetSettings Settings {
            get { return settings; }
        }

        /// <inheritdoc cref="IWeightControl.FadeInDuration"/>
        public float FadeInDuration {
            get { return fadeInDuration; }
        }

        /// <inheritdoc cref="IWeightControl.FadeOutDuration"/>
        public float FadeOutDuration {
            get { return fadeOutDuration; }
        }

        /// <inheritdoc cref="IWeightControl.FadeInCurve"/>
        public AnimationCurve FadeInCurve {
            get { return fadeInCurve; }
        }

        /// <inheritdoc cref="IWeightControl.FadeOutCurve"/>
        public AnimationCurve FadeOutCurve {
            get { return fadeOutCurve; }
        }
        #endregion

        #region Behaviour
        [NonSerialized] private AudioHandler handler = default;

        // -----------------------

        /// <inheritdoc cref="PlayAudio(Vector3, AudioAssetSettings)"/>
        public AudioHandler PlayAudio() {
            return AudioManager.Instance.Play(this, settings);
        }

        /// <inheritdoc cref="PlayAudio(Vector3, AudioAssetSettings)"/>
        public AudioHandler PlayAudio(Vector3 _position) {
            return PlayAudio(_position, settings);
        }

        /// <param name="_position"><inheritdoc cref="AudioManager.Play(AudioAsset, AudioAssetSettings, Vector3)" path="/param[@name='_position']"/></param>
        /// <inheritdoc cref="PlayAudio(Transform, AudioAssetSettings)"/>
        public AudioHandler PlayAudio(Vector3 _position, AudioAssetSettings _settings) {
            return AudioManager.Instance.Play(this, _settings, _position);
        }

        /// <inheritdoc cref="PlayAudio(Transform, AudioAssetSettings)"/>
        public AudioHandler PlayAudio(Transform _transform) {
            return PlayAudio(_transform, settings);
        }

        /// <summary>
        /// Plays this audio.
        /// </summary>
        /// <inheritdoc cref="AudioManager.Play(AudioAsset, AudioAssetSettings, Transform)"/>
        public AudioHandler PlayAudio(Transform _transform, AudioAssetSettings _settings) {
            return AudioManager.Instance.Play(this, _settings, _transform);
        }

        // -------------------------------------------
        // Handler
        // -------------------------------------------

        /// <summary>
        /// Sets this audio current handler.
        /// </summary>
        /// <param name="_handler">This audio current handler.</param>
        internal void SetHandler(AudioHandler _handler) {
            handler = _handler;
        }

        /// <summary>
        /// Get the last created <see cref="AudioHandler"/> to play this audio.
        /// </summary>
        /// <param name="_handler">Last created <see cref="AudioHandler"/> for this audio.</param>
        /// <returns>True if this audio handler is valid, false otherwise.</returns>
        public bool GetHandler(out AudioHandler _handler) {
            _handler = handler;
            return _handler.IsValid;
        }

        // -------------------------------------------
        // Feedback
        // -------------------------------------------

        public override void Play(Transform _transform, FeedbackPlayOptions _options) {
            // Instant play (delay managed in the player).
            OnPlay(_transform, _options);
        }

        protected override void OnPlay(Transform _transform, FeedbackPlayOptions _options) {
            switch (_options) {

                // Play at position.
                case FeedbackPlayOptions.PlayAtPosition:
                    PlayAudio(_transform.position);
                    break;

                // Follow transform.
                case FeedbackPlayOptions.FollowTransform:
                    PlayAudio(_transform);
                    break;

                // Vanilla.
                case FeedbackPlayOptions.None:
                default:
                    PlayAudio();
                    break;
            }
        }

        protected override void OnStop() {
            handler.Stop();
        }

        // -------------------------------------------
        // Buttons
        // -------------------------------------------

        /// <summary>
        /// Previews this audio asset (for editor use).
        /// </summary>
        [Button(SuperColor.Green, IsDrawnOnTop = false), DisplayName("Preview (2D)")]
        private void PlayPreview() {
            AudioManager.Instance.PlayPreview(this);
        }

        /// <summary>
        /// Pauses this audio asset preview (for editor use).
        /// </summary>
        [Button(SuperColor.Orange, IsDrawnOnTop = false), DisplayName("Pause")]
        private void PausePreview() {
            AudioManager.Instance.PausePreview(this);
        }

        /// <summary>
        /// Stops this audio asset preview (for editor use).
        /// </summary>
        [Button(SuperColor.Crimson, IsDrawnOnTop = false), DisplayName("Stop")]
        private void StopPreview() {
            AudioManager.Instance.StopPreview(this);
        }
        #endregion

        #region Utility
        private int lastRandomIndex = -1;

        // -----------------------

        /// <inheritdoc cref="SetupAudioSource(AudioSource, AudioAssetSettings)"/>
        public void SetupAudioSource(AudioSource _source) {
            SetupAudioSource(_source, settings);
        }

        /// <summary>
        /// Setups a given <see cref="AudioSource"/> for this audio.
        /// </summary>
        /// <param name="_source"><see cref="AudioSource"/> instance to setup.</param>
        /// <param name="_settings"><see cref="AudioAssetSettings"/> used for setup.</param>
        public void SetupAudioSource(AudioSource _source, AudioAssetSettings _settings) {
            _source.outputAudioMixerGroup   = mixerGroup;
            _source.reverbZoneMix           = reverbZoneMix;
            _source.spatialBlend            = spatialBlend;

            _source.priority    = priority;
            _source.volume      = Volume;
            _source.pitch       = Pitch;
            _source.loop        = isLooping;

            _source.clip = GetClip();

            _source.ignoreListenerPause = AudioManager.Instance.IgnorePause(mixerGroup);
            _source.time = useStartRange ? startRange.Random() : 0f;

            _settings.ApplyValues(_source);
        }

        /// <summary>
        /// Get a random clip from this audio.
        /// </summary>
        public AudioClip GetClip() {
            return clips.Array.Random(ref lastRandomIndex);
        }
        #endregion
    }
}
