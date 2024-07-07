// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.GameStates;
using EnhancedFramework.Core.Option;
using System;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Utility <see cref="Component"/> used to easily mute / unmute an audio group.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Audio/Mute Audio Group"), DisallowMultipleComponent]
    public sealed class MuteAudioGroup : EnhancedBehaviour {
        #region Global Members
        [Section("Mute Audio Group")]

        [SerializeField, Enhanced, Required] private ScriptableGameOption audioOption = null;

        [Space(5f)]

        [SerializeField] private bool useFadingObject = false;
        [SerializeField, Enhanced, ShowIf(nameof(useFadingObject)), Required] private FadingObjectBehaviour fadingObject = null;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private bool isActive = false; 
        #endregion

        #region Behaviour
        [NonSerialized] private MuteAudioGameState gameState = null;

        // -----------------------

        /// <summary>
        /// Mutes this object associated audio group.
        /// </summary>
        /// <returns>True if the group could be successfully muted, false otherwise.</returns>
        [Button(ActivationMode.Play, SuperColor.Green)]
        public bool Mute() {

            if (gameState.IsActive())
                return false;

            gameState = GameState.CreateState<MuteAudioGameState>();
            gameState.Bound(this);

            return true;
        }

        /// <summary>
        /// Unmutes this object associated audio group.
        /// </summary>
        /// <returns>True if the group could be successfully unmuted, false otherwise.</returns>
        [Button(ActivationMode.Play, SuperColor.Crimson)]
        public bool Unmute() {

            if (!gameState.IsActive())
                return false;

            gameState.RemoveState();
            return true;
        }

        /// <summary>
        /// Invert this object associated audio current mute state.
        /// </summary>
        public void ToggleMute() {

            if (isActive) {
                Unmute();
            } else {
                Mute();
            }
        }

        // -------------------------------------------
        // Button
        // -------------------------------------------

        internal void OnInit(MuteAudioGameState _gameState) {

            gameState = _gameState;
            isActive  = true;

            Mute(true);
        }

        internal void OnTerminate(MuteAudioGameState _gameState) {

            if (_gameState != gameState)
                return;

            gameState = null;
            isActive  = false;

            Mute(false);
        }

        // -----------------------

        private void Mute(bool _isMute) {

            // Only if audio option.
            if (audioOption.Option is not AudioVolumeOption _audioOption) {

                this.LogErrorMessage($"Option {audioOption.name} must be of type {typeof(AudioVolumeOption).Name} in order to be muted");
                return;
            }

            _audioOption.Mute(_isMute);

            // Feedback object.
            if (useFadingObject) {
                fadingObject.Fade(_isMute ? FadingMode.Show : FadingMode.Hide);
            }
        }
        #endregion
    }
}
