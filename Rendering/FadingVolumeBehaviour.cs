// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
#define TWEENING
#endif

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using UnityEngine;
using UnityEngine.Rendering;

#if TWEENING
using DG.Tweening;
#endif

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Rendering {
    /// <summary>
    /// Class wrapper for a fading <see cref="UnityEngine.Rendering.Volume"/> instance.
    /// <para/> Fading is performed using tweening, if enabled.
    /// </summary>
    [Serializable]
    public class FadingVolume : IFadingObject {
        #region Global Members
        /// <summary>
        /// This object <see cref="UnityEngine.Rendering.Volume"/>.
        /// </summary>
        [Enhanced, Required] public Volume Volume = null;

        /// <summary>
        /// The fading target weight of this volume: first value when fading out, second when fading in.
        /// </summary>
        [Tooltip("The fading target weight of this volume: first value when fading out, second when fading in")]
        [Enhanced, MinMax(0f, 1f)] public Vector2 FadeWeight = new Vector2(0f, 1f);

        [Tooltip("If true, enables / disables the associated volume component when fading this object")]
        public bool EnableVolume = true;

        [Space(10f)]

        [SerializeField, Enhanced, Range(0f, 10f)] private float fadeInDuration = .5f;
        #if TWEENING
        [SerializeField] private Ease fadeInEase = Ease.OutSine;

        [Space(5f)]
        #endif

        [SerializeField, Enhanced, Range(0f, 10f)] private float fadeOutDuration = .5f;
        #if TWEENING
        [SerializeField] private Ease fadeOutEase = Ease.InSine;
        #endif

        // -----------------------

        public virtual bool IsVisible {
            get { return Volume.weight == FadeWeight.y; }
        }

        public virtual float ShowDuration {
            get { return fadeInDuration; }
        }

        public virtual float HideDuration {
            get { return fadeOutDuration; }
        }
        #endregion

        #region Behaviour
        private const int DefaultGUID = 0;

        private TweenHandler tween = default;
        private DelayHandler delay = default;

        // -------------------------------------------
        // General
        // -------------------------------------------

        public void Show(Action _onComplete = null) {
            CancelCurrentFade();

            #if TWEENING
            Fade(FadeWeight.y, fadeInDuration, fadeInEase, _onComplete);
            #else
            Fade(FadeWeight.y, fadeInDuration, _onComplete);
            #endif
        }

        public void Hide(Action _onComplete = null) {
            CancelCurrentFade();
            #if TWEENING
            Fade(FadeWeight.x, fadeOutDuration, fadeOutEase, _onComplete);
            #else
            Fade(FadeWeight.x, fadeOutDuration, _onComplete);
            #endif
        }

        public void FadeInOut(float _duration, Action _onAfterFadeIn = null, Action _onBeforeFadeOut = null, Action _onComplete = null) {
            Show(OnShow);

            // ----- Local Methods ----- \\

            void OnShow() {
                _onAfterFadeIn?.Invoke();
                delay = Delayer.Call(_duration, OnWaitComplete, true);
            }

            void OnWaitComplete() {
                _onBeforeFadeOut?.Invoke();
                Hide(_onComplete);
            }
        }

        public void Fade(FadingMode _mode, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            switch (_mode) {
                case FadingMode.Show:
                    Show(_onComplete);
                    break;

                case FadingMode.Hide:
                    Hide(_onComplete);
                    break;

                case FadingMode.FadeInOut:
                    FadeInOut(_inOutWaitDuration, null, null, _onComplete);
                    break;

                case FadingMode.None:
                default:
                    break;
            }
        }

        public void Invert(Action _onComplete = null) {
            SetVisibility(!IsVisible, _onComplete);
        }

        public void SetVisibility(bool _isVisible, Action _onComplete = null) {
            if (_isVisible) {
                Show(_onComplete);
            } else {
                Hide(_onComplete);
            }
        }

        // -------------------------------------------
        // Instant
        // -------------------------------------------

        public void Show(bool _isInstant, Action _onComplete = null) {
            if (_isInstant) {
                Fade(FadeWeight.y, _onComplete);
            } else {
                Show(_onComplete);
            }
        }

        public void Hide(bool _isInstant, Action _onComplete = null) {
            if (_isInstant) {
                Fade(FadeWeight.x, _onComplete);
            } else {
                Hide(_onComplete);
            }
        }

        public void Fade(FadingMode _mode, bool _isInstant, Action _onComplete = null, float _inOutWaitDuration = .5f) {
            switch (_mode) {
                case FadingMode.Show:
                    Show(_isInstant, _onComplete);
                    break;

                case FadingMode.Hide:
                    Hide(_isInstant, _onComplete);
                    break;

                case FadingMode.FadeInOut:
                    if (_isInstant) {
                        Show(_isInstant, _onComplete);
                        Hide(_isInstant, _onComplete);
                    } else {
                        FadeInOut(_inOutWaitDuration, null, null, _onComplete);
                    }
                    break;

                case FadingMode.None:
                default:
                    break;
            }
        }

        public void Invert(bool _isInstant, Action _onComplete = null) {
            SetVisibility(!IsVisible, _isInstant, _onComplete);
        }

        public void SetVisibility(bool _isVisible, bool _isInstant, Action _onComplete = null) {
            if (_isVisible) {
                Show(_isInstant, _onComplete);
            } else {
                Hide(_isInstant, _onComplete);
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        public void Evaluate(float _time, bool _show) {
            float _value;

            if (_show) {
                _value = (fadeInDuration != 0f) ? (_time / fadeInDuration) : 0f;
            } else {
                _value = (fadeOutDuration != 0f) ? (_time / fadeOutDuration) : 0f;
            }

            SetFadeValue(Mathf.Clamp(_value, 0f, 1f), _show);
        }

        public void SetFadeValue(float _value, bool _show) {
            CancelCurrentFade();
            float _weight;

            #if TWEENING
            if (_show) {
                _weight = DOVirtual.EasedValue(FadeWeight.x, FadeWeight.y, _value, fadeInEase);
            } else {
                _weight = DOVirtual.EasedValue(FadeWeight.y, FadeWeight.x, _value, fadeOutEase);
            }
            #endif

            SetWeight(_weight);
        }

        protected void CancelCurrentFade() {
            delay.Cancel();

            #if EDITOR_COROUTINE
            if (!Application.isPlaying && (coroutine != null)) {
                EditorCoroutineUtility.StopCoroutine(coroutine);
            }
            #endif

            tween.Stop();
        }

        private void SetWeight(float _weight) {

            if (Volume.weight == _weight) {
                return;
            }

            Volume.weight = _weight;

            if (EnableVolume) {
                Volume.enabled = !Mathf.Approximately(_weight, FadeWeight.x);
            }
        }

        // -------------------------------------------
        // Fade
        // -------------------------------------------

        private void Fade(float _weight, Action _onComplete) {
            CancelCurrentFade();
            SetWeight(_weight);

            _onComplete?.Invoke();
        }

        private void Fade(float _weight, float _duration, Action _onComplete) {
            DoFade(_weight, CreateTween, _onComplete);

            // ----- Local Method ----- \\

            TweenHandler CreateTween(Action<float> _setter, Action<bool> _onStopped) {
                return Core.Tweener.Tween(Volume.weight, _weight, _setter, _duration, true, _onStopped);
            }
        }

        #if TWEENING
        private void Fade(float _weight, float _duration, Ease _ease, Action _onComplete) {
            DoFade(_weight, CreateTween, _onComplete);

            // ----- Local Method ----- \\

            TweenHandler CreateTween(Action<float> _setter, Action<bool> _onStopped) {
                return Core.Tweener.Tween(Volume.weight, _weight, _setter, _duration, _ease, true, _onStopped);
            }
        }
        #endif

        // -----------------------

        private void DoFade(float _weight, Func<Action<float>, Action<bool>, TweenHandler> _tweener, Action _onComplete) {
            CancelCurrentFade();

            if (Volume.weight == _weight) {
                OnStopped();
                return;
            }

            // Tween.
            tween = _tweener(Set, OnStopped);

            // ----- Local Methods ----- \\

            void Set(float _value) {
                SetWeight(_value);
            }

            void OnStopped(bool _completed = false) {
                _onComplete?.Invoke();
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------
        #endregion
    }

    /// <summary>
    /// Ready-to-use <see cref="EnhancedBehaviour"/>-encapsulated <see cref="FadingVolume"/>.
    /// <br/> Use this to quickly implement instantly fading <see cref="UnityEngine.Rendering.Volume"/> objects.
    /// </summary>
    [ScriptGizmos(false, true)]
    [RequireComponent(typeof(Volume))]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Rendering/Fading Volume"), DisallowMultipleComponent]
    public class FadingVolumeBehaviour : FadingObjectBehaviour {
        #region Global Members
        [Section("Fading Volume")]

        [SerializeField, Enhanced, Block] private FadingVolume volume = default;

        [Space(10f)]

        [SerializeField] private FadingMode initMode = FadingMode.Hide;

        // -----------------------

        public FadingVolume Volume {
            get { return volume; }
        }

        public override IFadingObject FadingObject {
            get { return volume; }
        }

        public override FadingMode InitMode {
            get { return initMode; }
        }
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            // References.
            if (!volume.Volume) {
                volume.Volume = GetComponent<Volume>();
            }
        }
#endif
        #endregion

        #region Play Mode Data
        public override bool CanSavePlayModeData {
            get { return true; }
        }

        // -----------------------

        public override void SavePlayModeData(PlayModeEnhancedObjectData _data) {

            // Save as json.
            _data.Strings.Add(JsonUtility.ToJson(volume));
        }

        public override void LoadPlayModeData(PlayModeEnhancedObjectData _data) {

            // Load from json.
            FadingVolume _volume = JsonUtility.FromJson<FadingVolume>(_data.Strings[0]);
            _volume.Volume = volume.Volume;

            volume = _volume;
        }
        #endregion
    }
}
