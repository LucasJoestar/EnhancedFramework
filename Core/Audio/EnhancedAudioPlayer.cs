// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Enhanced behaviour used to manage a <see cref="VideoPlayer"/>.
    /// <para/>
    /// Allow to preview the video in the editor, and use additional callbacks and functionalities.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    #pragma warning disable 0414
    public class EnhancedAudioPlayer : EnhancedBehaviour {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Play;

        #region Global Members
        [Section("Enhanced Audio Player")]

        #if UNITY_EDITOR
        [SerializeField, Enhanced, DrawMember("AudioClip"), ValidationMember("AudioClip")]
        private AudioClip audioClip = null; // Should never be used outside of the inspector.
        #endif

        [Space(5f)]

        [Tooltip("If true, the video will start playing right after the scene finished loading")]
        [SerializeField, Enhanced, ValidationMember("PlayAfterLoading")]
        private bool playAfterLoading = false;

        [Space(10f)]

        #if UNITY_EDITOR
        [SerializeField, Enhanced, DrawMember("Time"), Range("TimeRange"), ValidationMember("Time")]
        private float time = 0f; // Should never be used outside of the inspector.
        #endif

        // -----------------------

        /// <summary>
        /// <inheritdoc cref="AudioSource.clip"/>
        /// (From <see cref="AudioSource.clip"/>)
        /// </summary>
        public AudioClip AudioClip {
            get {
                return audioSource.clip;
            }
            set {
                audioSource.clip = value;
                Stop();
            }
        }

        /// <summary>
        /// Whether the <see cref="AudioSource"/> should start playing right after loading or not.
        /// </summary>
        public bool PlayAfterLoading {
            get { return playAfterLoading; }
            set {
                if (value) {
                    audioSource.playOnAwake = false;
                }

                playAfterLoading = value;
            }
        }

        /// <summary>
        /// <inheritdoc cref="AudioSource.time"/>
        /// (From <see cref="AudioSource.time"/>)
        /// </summary>
        public float Time {
            get { return audioSource.time; }
            set {
                value = Mathf.Clamp(value, TimeRange.x, TimeRange.y);
                audioSource.time = value;
            }
        }

        /// <summary>
        /// The range of the <see cref="AudioSource"/> current clip total time (betwwen 0 and its duration).
        /// </summary>
        public Vector2 TimeRange {
            get { return new Vector2(0f, Duration); }
        }

        // -----------------------

        #if UNITY_EDITOR
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, DrawMember("Duration"), ReadOnly] private float duration    = 0f;
        [SerializeField, Enhanced, DrawMember("IsPlaying"), ReadOnly] private bool isPlaying    = false;
        #endif

        /// <summary>
        /// The total duration of the <see cref="AudioSource"/> clip (in seconds).
        /// </summary>
        public float Duration {
            get {
                AudioClip _clip = AudioClip;
                return _clip.IsValid()
                     ? _clip.length
                     : 0f;
            }
        }

        /// <summary>
        /// Whether the <see cref="AudioSource"/> is currently playing or not.
        /// </summary>
        public bool IsPlaying {
            get { return audioSource.isPlaying; }
        }

        // -----------------------

        [HideInInspector] private AudioSource audioSource = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnPlay() {
            base.OnPlay();

            // Play once loading is over.
            if (playAfterLoading) {
                playAfterLoading = false;
                PlayFromStart();
            }
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            Stop();
        }

        // -----------------------

        #if UNITY_EDITOR
        private void OnValidate() {
            if (!audioSource) {
                audioSource = GetComponent<AudioSource>();
            }
        }
        #endif
        #endregion

        #region Behaviour
        /// <summary>
        /// <inheritdoc cref="AudioSource.Play"/>
        /// (From <see cref="AudioSource.Play"/>)
        /// </summary>
        [Button(SuperColor.Green)]
        public void Play() {
            audioSource.Play();
        }

        /// <summary>
        /// <inheritdoc cref="AudioSource.Pause"/>
        /// (From <see cref="AudioSource.Pause"/>)
        /// </summary>
        [Button(SuperColor.Orange)]
        public void Pause() {
            audioSource.Pause();
        }

        /// <summary>
        /// <inheritdoc cref="AudioSource.Stop"/>
        /// (From <see cref="AudioSource.Stop"/>)
        /// </summary>
        [Button(SuperColor.Crimson)]
        public void Stop() {
            audioSource.Stop();
        }
        #endregion

        #region Utility
        /// <summary>
        /// Plays this <see cref="AudioSource"/> from the start.
        /// </summary>
        public void PlayFromStart() {
            Time = 0f;
            Play();
        }

        /// <summary>
        /// Sets the clip of the associated <see cref="AudioSource"/>.
        /// </summary>
        /// <param name="_clip">The new clip of the <see cref="AudioSource"/>.</param>
        /// <param name="_play">Whether to start this clip from the start or not.</param>
        public void SetClip(AudioClip _clip, bool _play = true) {
            audioSource.clip = _clip;

            if (_play) {
                PlayFromStart();
            }
        }

        /// <summary>
        /// Get the <see cref="AudioSource"/> associated with this object.
        /// </summary>
        public AudioSource GetAudioSource() {
            return audioSource;
        }
        #endregion
    }
}
