// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if TEXT_MESH_PRO_PACKAGE && DOTWEEN_ENABLED
#define TEXT_DISPLAYER
#endif

#if TEXT_DISPLAYER
using DG.Tweening;
using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using TMPro;
using UnityEngine;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Utility component used to display text over time.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "UI/Text/Text Displayer"), DisallowMultipleComponent]
    public class TextDisplayer : EnhancedBehaviour {
        #region Global Members
        [Section("Text Displayer")]

        [Tooltip("Text to display content")]
        [SerializeField, Enhanced, Required] private TextMeshProUGUI text = null;

        [Tooltip("If true, display animation will not be affected by the game time scale")]
        [SerializeField] private bool useRealTime = false;

        // -----------------------

        /// <summary>
        /// Indicates if this text is currently active and displaying its content on screen.
        /// </summary>
        public bool IsActive {
            get { return !string.IsNullOrEmpty(Text); }
        }

        /// <summary>
        /// Current displayed text of this object.
        /// </summary>
        public string Text {
            get { return text.text; }
        }
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            if (text == null) {
                text = GetComponent<TextMeshProUGUI>();
            }
        }
        #endif
        #endregion

        #region Behaviour
        private Sequence sequence = null;
        private Tween tween = null;

        // -----------------------

        /// <summary>
        /// Displays the given text over time during a specific duration.
        /// </summary>
        /// <param name="_text">Text to display.</param>
        /// <param name="_duration">Total display duration (in seconds).</param>
        /// <param name="_onComplete">Called once display is completed.</param>
        /// <param name="_isInstant">If true, instantly displays the text.</param>
        /// <param name="_append">If true, appends this text content at the end of the current text value.</param>
        public void Display(string _text, float _duration, Action _onComplete = null, bool _isInstant = false, bool _append = false) {
            CompleteDisplay(false);
            Display(_text, _onComplete, _isInstant, _append, GetDuration);
            
            // ----- Local Method ----- \\

            float GetDuration(int _characterCount) {
                return _duration;
            }
        }

        /// <summary>
        /// Displays the given text over time during a specific duration per character.
        /// </summary>
        /// <param name="_text">Text to display.</param>
        /// <param name="_characterDuration">Display duration per character (in seconds).</param>
        /// <param name="_onComplete">Called once display is completed.</param>
        /// <param name="_isInstant">If true, instantly displays the text.</param>
        /// <param name="_append">If true, appends this text content at the end of the current text value.</param>
        public void DisplayPerCharacter(string _text, float _characterDuration, Action _onComplete = null, bool _isInstant = false, bool _append = false) {
            CompleteDisplay(false);
            Display(_text, _onComplete, _isInstant, _append, GetDuration);

            // ----- Local Method ----- \\

            float GetDuration(int _characterCount) {
                return _characterCount * _characterDuration;
            }
        }

        /// <summary>
        /// Displays multiple texts over time during a specific duration.
        /// </summary>
        /// <param name="_texts">Texts to display (string as first, delay as second).</param>
        /// <param name="_characterDuration">Display duration per character (in seconds).</param>
        /// <param name="_onComplete">Called once display is completed.</param>
        /// <param name="_isInstant">If true, instantly displays the text.</param>
        /// <param name="_append">If true, appends this text content at the end of the current text value.</param>
        public void DisplayPerCharacter(PairCollection<string, float> _texts, float _characterDuration, Action _onComplete = null, bool _isInstant = false, bool _append = false) {
            CompleteDisplay(false);

            if (_texts.Count == 0) {
                return;
            }

            // Avoid empty sequence.
            if (!_isInstant) {
                sequence = DOTween.Sequence();
            }

            foreach (var _text in _texts) {
                Display(_text.First, null, _isInstant, _append, GetDuration);

                if (!_isInstant) {
                    sequence.Append(tween);
                    sequence.AppendInterval(_text.Second);
                }

                tween = null;
            }

            // Sequence.
            if (!_isInstant) {
                sequence.SetUpdate(useRealTime).SetRecyclable(true).SetAutoKill(true).OnKill(OnKilled);
            } else {
                OnKilled();
            }

            // ----- Local Methods ----- \\

            float GetDuration(int _characterCount) {
                return _characterCount * _characterDuration;
            }

            void OnKilled() {
                sequence = null;
                _onComplete?.Invoke();
            }
        }

        // -----------------------

        private void Display(string _text, Action _onComplete, bool _isInstant, bool _append, Func<int, float> _getDuration) {
            int _visibleCount = 0;

            _text = _text.Replace(@"\n", "\n").Replace(@"\t", "\t");

            // Set text.
            if (_append) {
                _text = text.text + _text;
                _visibleCount = text.textInfo.characterCount;
            } else {
                text.maxVisibleCharacters = 0;
            }

            int _visibleCharacter = text.maxVisibleCharacters;

            text.text = _text;
            text.maxVisibleCharacters = _visibleCharacter;
            text.ForceMeshUpdate();

            int _count = text.textInfo.characterCount;

            // Instant.
            if (_isInstant) {
                text.maxVisibleCharacters = _count;
                OnKilled();

                return;
            }

            // Tween.
            tween = text.DOMaxVisibleCharacters(_count, _getDuration(_count - _visibleCount)).SetEase(Ease.Linear)
                        .SetUpdate(useRealTime).SetRecyclable(true).SetAutoKill(true).OnKill(OnKilled);

            // ----- Local Method ----- \\

            void OnKilled() {
                tween = null;
                _onComplete?.Invoke();
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        /// <summary>
        /// Completes the current display operation.
        /// </summary>
        public void CompleteDisplay(bool _completeDisplay = true) {
            sequence.DoKill(_completeDisplay);
            tween.DoKill(_completeDisplay);
        }

        /// <summary>
        /// Clears this text content.
        /// </summary>
        public void Clear() {

            text.text = string.Empty;
            text.maxVisibleCharacters = 0;

            CompleteDisplay(false);
        }
        #endregion
    }
}
#endif
