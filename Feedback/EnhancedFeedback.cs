// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using DG.Tweening;
using EnhancedEditor;
using System;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedAssetFeedback"/>-related extension methods.
    /// </summary>
    public static class EnhancedAssetFeedbackExtensions {
        #region Content
        /// <summary>
        /// Safely plays this feedback using a specific transform, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to play.</param>
        /// <inheritdoc cref="IEnhancedFeedback.Play(Transform)"/>
        public static void PlaySafe(this EnhancedAssetFeedback _feedback, Transform _transform) {
            if (!ReferenceEquals(_feedback, null)) {
                _feedback.Play(_transform);
            }
        }

        /// <summary>
        /// Safely plays this feedback, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to play.</param>
        public static void PlaySafe(this EnhancedAssetFeedback _feedback) {
            if (!ReferenceEquals(_feedback, null)) {
                _feedback.Play();
            }
        }

        /// <summary>
        /// Safely stops this feedback using a specific transform, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to stop.</param>
        /// <inheritdoc cref="IEnhancedFeedback.Stop(Transform)"/>
        public static void StopSafe(this EnhancedAssetFeedback _feedback, Transform _transform) {
            if (!ReferenceEquals(_feedback, null)) {
                _feedback.Stop(_transform);
            }
        }

        /// <summary>
        /// Safely stops this feedback, if not null.
        /// </summary>
        /// <param name="_feedback">The feedback to stop.</param>
        public static void StopSafe(this EnhancedAssetFeedback _feedback) {
            if (!ReferenceEquals(_feedback, null)) {
                _feedback.Stop();
            }
        }
        #endregion
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
        void Play(Transform _transform);

        /// <summary>
        /// Plays this feedback.
        /// </summary>
        void Play();

        /// <summary>
        /// Stops playing this feedback using a specific transform.
        /// </summary>
        /// <param name="_transform"><see cref="Transform"/> used to stop this feedback.</param>
        void Stop(Transform _transform);

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
        public void Play(Transform _transform) {
            foreach (EnhancedAssetFeedback _feedback in assetFeedbacks) {
                _feedback.Play(_transform);
            }

            foreach (EnhancedSceneFeedback _feedback in sceneFeedbacks) {
                _feedback.Play(_transform);
            }
        }

        public void Play() {
            foreach (EnhancedAssetFeedback _feedback in assetFeedbacks) {
                _feedback.Play();
            }

            foreach (EnhancedSceneFeedback _feedback in sceneFeedbacks) {
                _feedback.Play();
            }
        }

        public void Stop(Transform _transform) {
            foreach (EnhancedAssetFeedback _feedback in assetFeedbacks) {
                _feedback.Stop(_transform);
            }

            foreach (EnhancedSceneFeedback _feedback in sceneFeedbacks) {
                _feedback.Stop(_transform);
            }
        }

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
    /// Base class for <see cref="ScriptableObject"/> enhanced feedback.
    /// </summary>
    public abstract class EnhancedAssetFeedback : ScriptableObject, IEnhancedFeedback {
        #region Global Members
        public const string FilePrefix = "FBK_";
        public const string MenuPath    = FrameworkUtility.MenuPath + "Feedback/";
        public const int MenuOrder      = FrameworkUtility.MenuOrder;

        // -----------------------

        [Section("Enhanced Feedback")]

		[Enhanced, Range(0f, 5f)] public float Delay = 0f;
		[SerializeField] private bool doIgnoreTimeScale = false;
        #endregion

        #region Enhanced Feedback
        public void Play(Transform _transform) {
            if (Delay != 0f) {
                DOVirtual.DelayedCall(Delay, DoPlay, doIgnoreTimeScale);
            } else {
                OnPlay(_transform);
            }

            // ----- Local Method ----- \\

            void DoPlay() {
                OnPlay(_transform);
            }
        }

        public void Play() {
            if (Delay != 0f) {
                DOVirtual.DelayedCall(Delay, OnPlay, doIgnoreTimeScale);
            } else {
                OnPlay();
            }
        }

        public void Stop(Transform _transform) {
            OnStop(_transform);
        }

        public void Stop() {
            OnStop();
        }
        #endregion

        #region Behaviour
        protected virtual void OnPlay(Transform _transform) { }

        protected virtual void OnPlay() { }

        protected virtual void OnStop(Transform _transform) { }

        protected virtual void OnStop() { }
        #endregion
    }

    /// <summary>
    /// Base class for <see cref="MonoBehaviour"/> enhanced feedback (using scene reference(s)).
    /// </summary>
    public abstract class EnhancedSceneFeedback : EnhancedBehaviour, IEnhancedFeedback {
        #region Global Members
        [Section("Enhanced Scene Feedback")]

        [Enhanced, Range(0f, 5f)] public float Delay = 0f;
        [SerializeField] private bool doIgnoreTimeScale = false;
        #endregion

        #region Enhanced Feedback
        public void Play(Transform _transform) {
            if (Delay != 0f) {
                DOVirtual.DelayedCall(Delay, DoPlay, doIgnoreTimeScale);
            } else {
                OnPlay(_transform);
            }

            // ----- Local Method ----- \\

            void DoPlay() {
                OnPlay(_transform);
            }
        }

        public void Play() {
            if (Delay != 0f) {
                DOVirtual.DelayedCall(Delay, OnPlay2, doIgnoreTimeScale);
            } else {
                OnPlay2();
            }
        }

        public void Stop(Transform _transform) {
            OnStop(_transform);
        }

        public void Stop() {
            OnStop();
        }
        #endregion

        #region Behaviour
        protected virtual void OnPlay(Transform _transform) { }

        protected virtual void OnPlay2() { }

        protected virtual void OnStop(Transform _transform) { }

        protected virtual void OnStop() { }
        #endregion
    }
}
#endif
