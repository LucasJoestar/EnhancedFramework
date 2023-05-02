// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if ENABLE_INPUT_SYSTEM
#define NEW_INPUT_SYSTEM
#endif

#if NEW_INPUT_SYSTEM
using EnhancedEditor.Editor;
using EnhancedFramework.Inputs;
using UnityEditor;
using UnityEngine;

namespace EnhancedFramework.Editor {
    /// <summary>
    /// Custom <see cref="InputRecorder"/> editor.
    /// </summary>
    [CustomEditor(typeof(InputRecorder), true)]
    public class InputRecorderEditor : UnityObjectEditor {
        #region Styles
        public static class Styles {
            public static readonly GUIContent PlayGUI           = EditorGUIUtility.TrIconContent("PlayButton",          "Play the current input capture");
            public static readonly GUIContent PauseGUI          = EditorGUIUtility.TrIconContent("PauseButton",         "Pause the current input playback");
            public static readonly GUIContent ResumeGUI         = EditorGUIUtility.TrIconContent("PauseButton On",      "Resume the current input playback");
            public static readonly GUIContent StopGUI           = EditorGUIUtility.TrIconContent("PlayButton On",       "Stop the current input playback");
            public static readonly GUIContent RecordGUI         = EditorGUIUtility.TrIconContent("Animation.Record",    "Start recording input");
        }
        #endregion

        #region Editor GUI
        private const float PlayButtonWidth = 50f;
        private const string RecordFileExtension = InputRecorder.InputRecordFileExtension;

        private static readonly GUIContent infoGUI      = new GUIContent("Info:");
        private static readonly GUIContent devicesGUI   = new GUIContent("Devices");

        private static readonly GUIContent loadGUI  = new GUIContent("Load", "Loads an input record from a file on disk");
        private static readonly GUIContent saveGUI  = new GUIContent("Save", "Saves the current input record to a file on disk");
        private static readonly GUIContent clearGUI = new GUIContent("Clear", "Clears the current record content");

        // -----------------------

        protected override void OnAfterInspectorGUI() {
            base.OnAfterInspectorGUI();

            InputRecorder _recorder = target as InputRecorder;
            GUILayout.Space(10f);

            GUILayoutOption _width = GUILayout.Width(PlayButtonWidth);
            GUIStyle _buttonStyle = EditorStyles.miniButton;

            using (new EditorGUILayout.HorizontalScope()) {

                // Play and pause buttons.
                bool _disabled = (_recorder.EventCount == 0) || _recorder.IsRecording;
                using (new EditorGUI.DisabledGroupScope(_disabled)) {

                    bool _oldIsPlaying = _recorder.IsReplaying;
                    bool _newIsPlaying = GUILayout.Toggle(_oldIsPlaying, _oldIsPlaying ?  Styles.StopGUI : Styles.PlayGUI, _buttonStyle, _width);

                    if (_oldIsPlaying != _newIsPlaying) {

                        if (_newIsPlaying) {
                            _recorder.StartReplay();
                        } else {
                            _recorder.StopReplay();
                        }
                    }

                    if (_newIsPlaying && (_recorder.Replay != null) && GUILayout.Button(_recorder.Replay.paused ? Styles.ResumeGUI : Styles.PauseGUI, _buttonStyle, _width)) {

                        if (_recorder.Replay.paused) {
                            _recorder.StartReplay();
                        } else {
                            _recorder.PauseReplay();
                        }
                    }
                }

                using (new EditorGUI.DisabledGroupScope(_recorder.IsReplaying)) 
                using (var _scope = new EditorGUI.ChangeCheckScope()) {

                    // Record.
                    bool _isRecording = GUILayout.Toggle(_recorder.IsRecording, Styles.RecordGUI, _buttonStyle, _width);
                    if (_scope.changed) {

                        if (_isRecording) {
                            _recorder.StartRecord();
                        } else {
                            _recorder.StopRecord();
                        }
                    }

                    // Load.
                    if (GUILayout.Button(loadGUI, _buttonStyle)) {
                        string _path = EditorUtility.OpenFilePanel("Choose Input Event Trace to Load", string.Empty, RecordFileExtension);

                        if (!string.IsNullOrEmpty(_path)) {
                            _recorder.LoadRecordFromFile(_path);
                        }
                    }
                }

                _disabled = _recorder.IsReplaying || (_recorder.EventCount == 0);
                using (new EditorGUI.DisabledGroupScope(_disabled)) {

                    // Save.
                    if (GUILayout.Button(saveGUI, _buttonStyle)) {
                        string _path = EditorUtility.SaveFilePanel("Choose Where to Save Input Event Trace", string.Empty,
                                       $"{_recorder.gameObject.name}.{RecordFileExtension}", RecordFileExtension);

                        if (!string.IsNullOrEmpty(_path)) {
                            _recorder.SaveRecordToFile(_path);
                        }
                    }

                    // Clear.
                    if (GUILayout.Button(clearGUI, _buttonStyle)) {

                        _recorder.ClearCapture();
                        Repaint();
                    }
                }
            }

            // Play bar.
            EditorGUILayout.IntSlider(_recorder.ReplayPosition, 0, (int)_recorder.EventCount);
            GUILayout.Space(7f);

            // Infos.
            using (new EditorGUI.DisabledScope()) {
                EditorGUILayout.LabelField(infoGUI, EditorStyles.miniBoldLabel);

                using (new EditorGUI.IndentLevelScope()) {

                    GUIStyle _style = EditorStyles.miniLabel;

                    EditorGUILayout.LabelField($"{_recorder.EventCount} events", _style);
                    EditorGUILayout.LabelField($"{_recorder.TotalEventSizeInBytes / 1024} kb captured", _style);
                    EditorGUILayout.LabelField($"{_recorder.AllocatedSizeInBytes / 1024} kb allocated", _style);

                    if (_recorder.Capture != null) {
                        var _devices = _recorder.Capture.deviceInfos;

                        if (_devices.Count != 0) {
                            EditorGUILayout.LabelField(devicesGUI, EditorStyles.miniBoldLabel);

                            using (new EditorGUI.IndentLevelScope()) {
                                foreach (var _device in _devices) {
                                    EditorGUILayout.LabelField(_device.layout, EditorStyles.miniLabel);
                                }
                            }
                        }
                    }
                }
            }

            // Record slider display.
            if (_recorder.IsReplaying) {
                Repaint();
            }
        }
        #endregion
    }
}
#endif
