// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

using MessageType = EnhancedEditor.MessageType;
#endif

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="ScriptableObject"/> encapsulated audio settings applied to <see cref="AudioAsset"/>.
    /// </summary>
    [CreateAssetMenu(fileName = "AST_AudioSettings", menuName = FrameworkUtility.MenuPath + "Audio/Audio Asset Settings", order = FrameworkUtility.MenuOrder)]
    public class AudioAssetSettings : EnhancedScriptableObject {
        #region Global Members
        [Section("Audio Settings")]

        [Tooltip("Multiplier applied to the audio volume")]
        [SerializeField, Enhanced, Range(0f, 10f)] private float volumeMultiplier = 1f;

        [Space(10f, order = 0), Title("3D Sound", order = 1), Space(3f, order = 3)]

        [Tooltip("Doppler scale used on the audio")]
        [SerializeField, Enhanced, Range(0f, 5f)] private float dopplerLevel = 1f;

        [Tooltip("Spread angle (in degrees) of a 3D channel stereo or multichannel sound in speaker space")]
        [SerializeField, Enhanced, Range(0f, 360f)] private float spreadAngle = 0f;

        [Space(10f)]

        [Tooltip("Determines how the audio attenuates over distance")]
        [SerializeField] private AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic;

        [Tooltip("Minimum and maximum distance used to attenuate the audio volume (loudest at min distance, completly attenuated at max)")]
        [SerializeField, Enhanced, MinMax(0f, 500f)] private Vector2 attenuationDistance = new Vector2(1f, 30f);

        [Space(10f, order = 0), Title("3D Curves", order = 1), Space(5f, order = 3)]

        [Enhanced, HelpBox("Value as Y value, min and max distance as X value", MessageType.Info)]

        [SerializeField, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.DarkOrange)]  private AnimationCurve customRolloffCurve   = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Yellow)]      private AnimationCurve reverbZoneMixCurve   = AnimationCurve.Constant(0f, 1f, 1f);
        [SerializeField, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Green)]       private AnimationCurve spatialBlendCurve    = AnimationCurve.Constant(0f, 1f, 0f);
        [SerializeField, EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Blue)]        private AnimationCurve spreadCurve          = AnimationCurve.Constant(0f, 1f, 0f);

        // -----------------------

        /// <summary>
        /// Multiplier applied to the audio volume.
        /// </summary>
        public float VolumeMultiplier {
            get { return volumeMultiplier; }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Applies these settings value to a given <see cref="AudioSource"/>.
        /// </summary>
        /// <param name="_audio"><see cref="AudioSource"/> to apply these settings values on.</param>
        [Button(SuperColor.Green, IsDrawnOnTop = false)]
        public void ApplyValues(AudioSource _audio) {
            #if UNITY_EDITOR
            if (!Application.isPlaying) {
                //Undo.RecordObject(_audio, "Apply audio settings values");
            }
            #endif

            _audio.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve);
            _audio.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, reverbZoneMixCurve);
            _audio.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialBlendCurve);
            _audio.SetCustomCurve(AudioSourceCurveType.Spread, spreadCurve);

            _audio.dopplerLevel = dopplerLevel;
            _audio.rolloffMode = rolloffMode;
            _audio.minDistance = attenuationDistance.x;
            _audio.maxDistance = attenuationDistance.y;
            _audio.spread = spreadAngle;
        }

        /// <summary>
        /// Copies the values of a specific <see cref="AudioSource"/> in these settings.
        /// </summary>
        /// <param name="_audio"><see cref="AudioSource"/> to copy the values in these settings.</param>
        [Button(SuperColor.Crimson, IsDrawnOnTop = false)]
        public void CopyValues(AudioSource _audio) {
            #if UNITY_EDITOR
            Undo.RecordObject(this, "Copy audio settings values");
            #endif

            customRolloffCurve = _audio.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
            reverbZoneMixCurve = _audio.GetCustomCurve(AudioSourceCurveType.ReverbZoneMix);
            spatialBlendCurve = _audio.GetCustomCurve(AudioSourceCurveType.SpatialBlend);
            spreadCurve = _audio.GetCustomCurve(AudioSourceCurveType.Spread);

            dopplerLevel = _audio.dopplerLevel;
            rolloffMode = _audio.rolloffMode;
            spreadAngle = _audio.spread;
            attenuationDistance.x = _audio.minDistance;
            attenuationDistance.y = _audio.maxDistance;
        }
        #endregion
    }
}
