// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace EnhancedFramework.Inputs {
    public class InputRecorder : EnhancedBehaviour {
        #region Global Members
        [Section("Input Recorder")]

        [SerializeField] private InputActionEnhancedAsset recordInput = null;
        [SerializeField] private InputActionEnhancedAsset replayInput = null;

        [SerializeField] private bool m_StartRecordingWhenEnabled = false;

        [SerializeField] private int m_CaptureMemoryDefaultSize = 2 * 1024 * 1024;
        [SerializeField] private int m_CaptureMemoryMaxSize = 10 * 1024 * 1024;

        // -----------------------

        private InputEventTrace eventTrace = null;
        private InputEventTrace.ReplayController replayController = null;
        #endregion

        #region Property
        /// <summary>
        /// Whether a capture is currently in progress.
        /// </summary>
        /// <value>True if a capture is in progress.</value>
        public bool IsCapturing {
            get { return (eventTrace != null) && eventTrace.enabled; }
        }

        /// <summary>
        /// Whether a replay is currently being run by the component.
        /// </summary>
        /// <value>True if replay is running.</value>
        /// <seealso cref="replay"/>
        /// <seealso cref="StartReplay"/>
        /// <seealso cref="StopReplay"/>
        public bool IsReplaying {
            get { return (replayController != null) && !replayController.finished; }
        }

        /// <summary>
        /// Total number of events captured.
        /// </summary>
        /// <value>Number of captured events.</value>
        public long eventCount => eventTrace?.eventCount ?? 0;

        /// <summary>
        /// Total size of captured events.
        /// </summary>
        /// <value>Size of captured events in bytes.</value>
        public long totalEventSizeInBytes => eventTrace?.totalEventSizeInBytes ?? 0;

        /// <summary>
        /// Total size of capture memory currently allocated.
        /// </summary>
        /// <value>Size of memory allocated for capture.</value>
        public long allocatedSizeInBytes => eventTrace?.allocatedSizeInBytes ?? 0;

        /// <summary>
        /// The underlying event trace that contains the captured input events.
        /// </summary>
        /// <value>Underlying event trace.</value>
        /// <remarks>
        /// This will be null if no capture is currently associated with the recorder.
        /// </remarks>
        public InputEventTrace capture => eventTrace;

        /// <summary>
        /// The replay controller for when a replay is running.
        /// </summary>
        /// <value>Replay controller for the event trace while replay is running.</value>
        /// <seealso cref="IsReplaying"/>
        /// <seealso cref="StartReplay"/>
        public InputEventTrace.ReplayController replay => replayController;

        public int replayPosition {
            get {
                if (replayController != null) {
                    return replayController.position;
                }

                return 0;
            }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            if (m_StartRecordingWhenEnabled) {
                StartCapture();
            }
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            StopCapture();
            StopReplay();
        }

        private void OnDestroy() {
            replayController?.Dispose();
            eventTrace?.Dispose();
        }
        #endregion

        #region Behaviour
        private void CreateEventTrace() {
            if (eventTrace != null) {
                eventTrace.Dispose();
            }

            eventTrace = new InputEventTrace(m_CaptureMemoryDefaultSize, growBuffer: true, maxBufferSizeInBytes: m_CaptureMemoryMaxSize);
            eventTrace.recordFrameMarkers = true;
        }
        #endregion

        #region Capture
        public void StartCapture() {
            if (eventTrace != null && eventTrace.enabled)
                return;

            CreateEventTrace();
            eventTrace.Enable();
        }

        public void StopCapture() {
            if (eventTrace != null && eventTrace.enabled) {
                eventTrace.Disable();
            }
        }

        public void StartReplay() {
            if (eventTrace == null)
                return;

            if (IsReplaying && replay.paused) {
                replay.paused = false;
                return;
            }

            StopCapture();

            // Configure replay controller.
            replayController = eventTrace.Replay().OnFinished(StopReplay);
            replayController.WithAllDevicesMappedToNewInstances();

            replayController.PlayAllEventsAccordingToTimestamps();
        }

        public void StopReplay() {
            if (replayController != null) {
                replayController.Dispose();
                replayController = null;
            }
        }

        public void PauseReplay() {
            if (replayController != null)
                replayController.paused = true;
        }

        public void ClearCapture() {
            eventTrace?.Clear();
        }

        public void LoadCaptureFromFile(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentNullException(nameof(fileName));
            }

            CreateEventTrace();
            eventTrace.ReadFrom(fileName);
        }

        public void SaveCaptureToFile(string fileName) {
            if (string.IsNullOrEmpty(fileName)) {
                throw new ArgumentNullException(nameof(fileName));
            }

            eventTrace?.WriteTo(fileName);
        }
        #endregion
    }
}
