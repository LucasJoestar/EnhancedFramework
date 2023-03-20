// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

using Random = UnityEngine.Random;
using Range  = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="AudioAmbientAsset"/>-related one shot sound asset.
    /// </summary>
    [CreateAssetMenu(fileName = "AMS_AmbientSound", menuName = FrameworkUtility.MenuPath + "Audio/Ambient Sound", order = FrameworkUtility.MenuOrder)]
    public class AudioAmbientSoundAsset : EnhancedScriptableObject {
        #region Play Mode
        /// <summary>
        /// Mode used to determine the position where to play a sound from an ambient.
        /// </summary>
        public enum PlayMode {
            /// <summary>
            /// Plays relative to the ambient center position.
            /// </summary>
            FromAmbientCenter,

            /// <summary>
            /// Play relative to the audio listener curent position.
            /// </summary>
            FromListener,

            /// <summary>
            /// Play relative to the ambient current actor position.
            /// </summary>
            FromActor,
        }
        #endregion

        #region Global Members
        [Section("Ambient Sound")]

        [Tooltip("Audio sound to play")]
        [SerializeField, Enhanced, Required] private AudioAsset sound = null;

        [Space(10f)]

        [Tooltip("Determines from which reference position to play this sound")]
        [SerializeField] private PlayMode playMode = PlayMode.FromAmbientCenter;

        [Tooltip("Min max delay interval between each playing of this sound (in seconds)")]
        [SerializeField, Enhanced, MinMax(0f, 60f)] private Vector2 playInterval = new Vector2(5f, 10f);

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("The min max normalized distance between the actor and the ambient center required to play this sound")]
        [SerializeField, Enhanced, MinMax(0f, 1f)] private Vector2 activationRange = new Vector2(0f, 1f);

        [Space(10f)]

        [Tooltip("Min max flat distance (X & Z axises) used to play this sound from the reference position")]
        [SerializeField, Enhanced, MinMax("PlayFlatRange")] private Vector2 playFlatDistance = new Vector2(1f, 3f);

        [Tooltip("Min max vertical distance (Y axis) used to play this sound from the reference position")]
        [SerializeField, Enhanced, MinMax("PlayVerticalRange")] private Vector2 playVerticalDistance = new Vector2(0f, 1f);

        [Space(10f)]

        [Tooltip("Play range max slider value")]
        [SerializeField, Enhanced, Range(5f, 99f)] private float playMaxRange = 10f;

        // -----------------------

        /// <summary>
        /// <see cref="AudioAsset"/> sound to play.
        /// </summary>
        public AudioAsset Sound {
            get { return sound; }
        }

        /// <summary>
        /// <see cref="PlayMode"/> used to determine from which reference position to play this sound.
        /// </summary>
        public PlayMode PlaySoundMode {
            get { return playMode; }
        }

        /// <summary>
        /// Random time interval used to play this sound.
        /// </summary>
        public float Interval {
            get { return playInterval.Random(); }
        }

        /// <summary>
        /// Get a random local position to play this sound from (using the game listener or the ambient center).
        /// </summary>
        public Vector3 PlayPosition {
            get { return (Random.onUnitSphere * playFlatDistance.Random()).SetY(playVerticalDistance.Random()); }
        }

        // -----------------------

        /// <summary>
        /// Play flat distance min max slider range.
        /// </summary>
        private Vector2 PlayFlatRange {
            get { return new Vector2(0f, playMaxRange); }
        }

        /// <summary>
        /// Play vertical distance min max slider range.
        /// </summary>
        private Vector2 PlayVerticalRange {
            get { return new Vector2(playMaxRange * -.5f, playMaxRange * .5f); }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get a random world position to play this sound from.
        /// </summary>
        /// <param name="_position">Reference world position to get a random play distance from.</param>
        /// <returns>World position to play this sound from.</returns>
        public Vector3 GetPlayPosition(Vector3 _position) {
            return _position + PlayPosition;
        }

        /// <summary>
        /// Get if the ambient actor is in the required range to play this sound.
        /// </summary>
        /// <param name="_actorDistance">Actor normalized distance from the ambient area center.</param>
        /// <returns>True if the actor is in this sound required range, false otherwise.</returns>
        public bool IsInRange(float _actorDistance) {
            return activationRange.Contains(_actorDistance);
        }
        #endregion
    }
}
