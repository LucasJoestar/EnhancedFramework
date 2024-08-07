// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Collections.Generic;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="AudioAsset"/>-related <see cref="EnhancedBehaviour"/> controller.
    /// </summary>
    [AddComponentMenu(FrameworkUtility.MenuPath + "Audio/Audio Controller"), DisallowMultipleComponent]
    public sealed class AudioAssetController : AudioWeightControllerBehaviour {
        #region Global Members
        [Section("Audio Asset Controller"), PropertyOrder(0)]

        [Tooltip("Audios to play within this area")]
        [SerializeField] private List<AudioAsset> playAudios = new List<AudioAsset>();

        [Tooltip("Audios to fade out within this area")]
        [SerializeField] private List<AudioAsset> fadeOutAudios = new List<AudioAsset>();

        [Space(10f)]

        [Tooltip("Volume coefficient applied to all playing audios")]
        [SerializeField, Enhanced, Range(0f, 1f)] private float volume = 1f;

        [Tooltip("Coefficient applied to the weight of fading out audios")]
        [SerializeField, Enhanced, Range(0f, 5f)] private float fadeOutWeightCoef = 1f;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Settings used to play this audio")]
        [SerializeField, Enhanced, ShowIf(nameof(overrideSettings)), Range(0f, 99f)] private AudioAssetSettings settings = null;

        [Tooltip("If true, overrides the default settings of this audio")]
        [SerializeField] private bool overrideSettings = false;

        [Tooltip("Audio loop override")]
        [SerializeField] private LoopOverride loopOverride = LoopOverride.None;
        #endregion

        #region Behaviour
        private const float TweenVolumeDuration = .1f;

        private readonly List<AudioHandler> handlers = new List<AudioHandler>();

        // -----------------------

        protected override void OnActivation() {
            // Play audios.
            Transform _transform = transform;

            int _count = playAudios.Count;
            for (int i = 0; i < _count; i++) {

                AudioAsset _audio = playAudios[i];
                AudioHandler _handler = AudioManager.Instance.Play(_audio, GetSettings(_audio), _transform);
                handlers.Add(_handler);

                if (_handler.GetHandle(out EnhancedAudioPlayer _player)) {
                    _player.PushVolumeModifier(AudioPlayerModifier.ControllerSource, 0f);

                    // Loop override.
                    if (loopOverride != LoopOverride.None){
                        _player.Loop = loopOverride == LoopOverride.Loop;
                    }
                }
            }

            base.OnActivation();
        }

        protected override void OnDeactivation() {
            base.OnDeactivation();

            bool _active = isActiveAndEnabled;
            int _count;

            // Stop fade out.
            _count = fadeOutAudios.Count;
            for (int i = 0; i < _count; i++) {

                if (fadeOutAudios[i].GetHandler(out AudioHandler _handler) && _handler.GetHandle(out EnhancedAudioPlayer _player)) {
                    _player.PopVolumeModifier(AudioPlayerModifier.ControllerFadeOut);
                }
            }

            // Stop audios.
            _count = handlers.Count;
            for (int i = 0; i < _count; i++) {

                AudioHandler _handler = handlers[i];
                _handler.Stop();

                // In case object is being destroyed.
                if (!_active && _handler.GetHandle(out EnhancedAudioPlayer _player)) {
                    _player.StopFollowTransform();
                }
            }

            handlers.Clear();
        }

        // -------------------------------------------
        // Controller
        // -------------------------------------------

        protected override void SetWeight(float _weight) {
            base.SetWeight(_weight);

            // Active audios.
            SetVolume(AudioPlayerModifier.ControllerSource, _weight * volume, TweenVolumeDuration);

            // Fade out.
            _weight = 1f - (_weight * fadeOutWeightCoef);

            int _count = fadeOutAudios.Count;
            for (int i = 0; i < _count; i++) {

                if (fadeOutAudios[i].GetHandler(out AudioHandler _handler) && _handler.GetHandle(out EnhancedAudioPlayer _player)) {
                    _player.TweenVolumeModifier(AudioPlayerModifier.ControllerFadeOut, _weight, TweenVolumeDuration);
                }
            }
        }

        /// <summary>
        /// Sets the volume modifier of all active audios in this controller.
        /// </summary>
        /// <param name="_modifier">Volume modifier type.</param>
        /// <param name="_volume">Volume modifier value.</param>
        /// <param name="_tweenDuration">Duration of the modifier transition (in seconds).</param>
        public void SetVolume(AudioPlayerModifier _modifier, float _volume, float _tweenDuration) {

            int _count = handlers.Count;
            for (int i = 0; i < _count; i++) {

                if (handlers[i].GetHandle(out EnhancedAudioPlayer _player)) {
                    _player.TweenVolumeModifier(_modifier, _volume, _tweenDuration);
                }
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get the settings to use to play an audio.
        /// </summary>
        /// <param name="_audio">Audio to play.</param>
        /// <returns>Settings to play this audio.</returns>
        public AudioAssetSettings GetSettings(AudioAsset _audio) {
            return overrideSettings ? settings : _audio.Settings;
        }
        #endregion
    }
}
