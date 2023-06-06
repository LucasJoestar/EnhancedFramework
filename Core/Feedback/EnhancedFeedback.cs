// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedAssetFeedback"/>-related extension methods.
    /// </summary>
    public static class EnhancedFeedbackExtensions {
        #region Asset
        /// <summary>
        /// Safely plays this feedback using a specific transform, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to play.</param>
        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform, FeedbackPlayOptions)"/>
        public static void PlaySafe(this EnhancedAssetFeedback _feedback, Transform _transform, FeedbackPlayOptions _options = FeedbackPlayOptions.PlayAtPosition) {
            if (_feedback.IsValid()) {
                _feedback.Play(_transform, _options);
            }
        }

        /// <summary>
        /// Safely plays this feedback, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to play.</param>
        public static void PlaySafe(this EnhancedAssetFeedback _feedback) {
            if (_feedback.IsValid()) {
                _feedback.Play();
            }
        }

        /// <summary>
        /// Safely stops this feedback, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to stop.</param>
        public static void StopSafe(this EnhancedAssetFeedback _feedback) {
            if (_feedback.IsValid()) {
                _feedback.Stop();
            }
        }
        #endregion

        #region Scene
        /// <inheritdoc cref="PlaySafe(EnhancedAssetFeedback, Transform, FeedbackPlayOptions)"/>
        public static void PlaySafe(this EnhancedSceneFeedback _feedback, Transform _transform, FeedbackPlayOptions _options = FeedbackPlayOptions.PlayAtPosition) {
            if (_feedback.IsValid()) {
                _feedback.Play(_transform, _options);
            }
        }

        /// <inheritdoc cref="PlaySafe(EnhancedAssetFeedback)"/>
        public static void PlaySafe(this EnhancedSceneFeedback _feedback) {
            if (_feedback.IsValid()) {
                _feedback.Play();
            }
        }

        /// <inheritdoc cref="StopSafe(EnhancedAssetFeedback)"/>
        public static void StopSafe(this EnhancedSceneFeedback _feedback) {
            if (_feedback.IsValid()) {
                _feedback.Stop();
            }
        }
        #endregion
    }

    /// <summary>
    /// Various options used to play an <see cref="IEnhancedFeedback"/>.
    /// </summary>
    public enum FeedbackPlayOptions {
        /// <summary>
        /// Plays the feedback with no option.
        /// </summary>
        None = 0,

        /// <summary>
        /// Plays the feedback at a specific position.
        /// </summary>
        PlayAtPosition = 1,

        /// <summary>
        /// Plays the feedback and parent it to a specific Transform.
        /// </summary>
        FollowTransform = 2,
    }

    /// <summary>
    /// Base interface for enhanced easy-to-implement feedbacks.
    /// </summary>
    public interface IEnhancedFeedback {
        #region Content
        /// <summary>
        /// Plays this feedback using a specific transform.
        /// </summary>
        /// <param name="_transform"><see cref="Transform"/> used to play this feedback.</param>
        /// <param name="_options">Options used to play this feedback.</param>
        void Play(Transform _transform, FeedbackPlayOptions _options);

        /// <summary>
        /// Plays this feedback.
        /// </summary>
        void Play();

        /// <summary>
        /// Stops playing this feedback.
        /// </summary>
        void Stop();
        #endregion
    }

    /// <summary>
    /// Holder for multiple <see cref="EnhancedAssetFeedback"/> and <see cref="EnhancedSceneFeedback"/>.
    /// </summary>
    [Serializable]
    public class EnhancedFeedbacks : IEnhancedFeedback, ISerializationCallbackReceiver {
        #region Global Members
        [SerializeField] private EnhancedAssetFeedback[] assetFeedbacks = new EnhancedAssetFeedback[] { };
        [SerializeField] private EnhancedSceneFeedback[] sceneFeedbacks = new EnhancedSceneFeedback[] { };
        #endregion

        #region Serialization
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            RemoveNullReferences();
        }

        // -----------------------

        private void RemoveNullReferences() {
            #if UNITY_EDITOR
            // Remove null references to avoid any error.
            ArrayUtility.RemoveNulls(ref assetFeedbacks);
            ArrayUtility.RemoveNulls(ref sceneFeedbacks);
            #endif
        }
        #endregion

        #region Enhanced Feedback
        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform, FeedbackPlayOptions)"/>
        public void Play(Transform _transform, FeedbackPlayOptions _options = FeedbackPlayOptions.PlayAtPosition) {
            foreach (EnhancedAssetFeedback _feedback in assetFeedbacks) {
                _feedback.Play(_transform, _options);
            }

            foreach (EnhancedSceneFeedback _feedback in sceneFeedbacks) {
                _feedback.Play(_transform, _options);
            }
        }

        /// <inheritdoc cref="IEnhancedFeedback.Play"/>
        public void Play() {
            foreach (EnhancedAssetFeedback _feedback in assetFeedbacks) {
                _feedback.Play();
            }

            foreach (EnhancedSceneFeedback _feedback in sceneFeedbacks) {
                _feedback.Play();
            }
        }

        /// <inheritdoc cref="IEnhancedFeedback.Stop"/>
        public void Stop() {
            foreach (EnhancedAssetFeedback _feedback in assetFeedbacks) {
                _feedback.Stop();
            }

            foreach (EnhancedSceneFeedback _feedback in sceneFeedbacks) {
                _feedback.Stop();
            }
        }
        #endregion
    }

    /// <summary>
    /// Base class for any <see cref="EnhancedScriptableObject"/> enhanced feedback.
    /// </summary>
    public abstract class EnhancedAssetFeedback : EnhancedScriptableObject, IEnhancedFeedback {
        #region Global Members
        public const string FilePrefix  = "FBK_";
        public const string MenuPath    = FrameworkUtility.MenuPath + "Feedback/";
        public const int MenuOrder      = FrameworkUtility.MenuOrder;

        // -----------------------

        [Tooltip("Delay before playing this feedback")]
        [Enhanced, Range(0f, 10f)] public float Delay = 0f;

        [Tooltip("If true, delay will not be affected by time scale")]
		[SerializeField, Enhanced, DisplayName("Real Time")] private bool useRealTime = false;
        #endregion

        #region Enhanced Feedback
        private DelayHandler delay = default;

        // -----------------------

        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform, FeedbackPlayOptions)"/>
        [Button(ActivationMode.Play, SuperColor.Green)]
        public virtual void Play(Transform _transform, FeedbackPlayOptions _options = FeedbackPlayOptions.PlayAtPosition) {
            if (Delay != 0f) {
                delay = Delayer.Call(Delay, DoPlay, useRealTime);
            } else {
                DoPlay();
            }

            // ----- Local Method ----- \\

            void DoPlay() {
                OnPlay(_transform, _options);
            }
        }

        /// <inheritdoc cref="IEnhancedFeedback.Play"/>
        public void Play() {
            Play(null, FeedbackPlayOptions.None);
        }

        /// <inheritdoc cref="IEnhancedFeedback.Stop"/>
        [Button(ActivationMode.Play, SuperColor.Crimson)]
        public virtual void Stop() {
            delay.Cancel();
            OnStop();
        }
        #endregion

        #region Behaviour
        /// <summary>
        /// Called when starting to play this feedback.
        /// <br/> Implements this feedback behaviour here.
        /// </summary>
        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform, FeedbackPlayOptions)"/>
        protected abstract void OnPlay(Transform _transform, FeedbackPlayOptions _options);

        /// <summary>
        /// Called when stopping to play this feedback.
        /// <br/> Stops this feedback behaviour here.
        /// </summary>
        /// <inheritdoc cref="IEnhancedFeedback.Stop"/>
        protected abstract void OnStop();
        #endregion
    }

    /// <summary>
    /// Base class for any <see cref="EnhancedBehaviour"/> enhanced feedback (using scene reference(s)).
    /// </summary>
    public abstract class EnhancedSceneFeedback : EnhancedBehaviour, IEnhancedFeedback {
        #region Global Members
        [Section("Enhanced Scene Feedback")]

        [Tooltip("Delay before playing this feedback")]
        [Enhanced, Range(0f, 5f)] public float Delay = 0f;

        [Tooltip("If true, delay will not be affected by time scale")]
        [SerializeField, Enhanced, DisplayName("Real Time")] private bool useRealTime = false;
        #endregion

        #region Enhanced Feedback
        private DelayHandler delay = default;

        // -----------------------

        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform, FeedbackPlayOptions)"/>
        public virtual void Play(Transform _transform, FeedbackPlayOptions _options = FeedbackPlayOptions.PlayAtPosition) {
            if (Delay != 0f) {
                delay = Delayer.Call(Delay, DoPlay, useRealTime);
            } else {
                DoPlay();
            }

            // ----- Local Method ----- \\

            void DoPlay() {
                OnPlayFeedback(_transform, _options);
            }
        }

        /// <inheritdoc cref="IEnhancedFeedback.Play"/>
        public void Play() {
            Play(null, FeedbackPlayOptions.None);
        }

        /// <inheritdoc cref="IEnhancedFeedback.Stop"/>
        public virtual void Stop() {
            delay.Cancel();
            OnStopFeedback();
        }
        #endregion

        #region Behaviour
        /// <summary>
        /// Called when starting to play this feedback.
        /// <br/> Implements this feedback behaviour here.
        /// </summary>
        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform, FeedbackPlayOptions)"/>
        protected abstract void OnPlayFeedback(Transform _transform, FeedbackPlayOptions _options);

        /// <summary>
        /// Called when stopping to play this feedback.
        /// <br/> Stops this feedback behaviour here.
        /// </summary>
        /// <inheritdoc cref="IEnhancedFeedback.Stop"/>
        protected abstract void OnStopFeedback();
        #endregion
    }
}
