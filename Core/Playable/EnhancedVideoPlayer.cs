// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.GameStates;
using System;
using UnityEngine;
using UnityEngine.Video;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Enhanced behaviour used to manage a <see cref="VideoPlayer"/>.
    /// <para/>
    /// Allow to preview the video in the editor, and use additional callbacks and functionalities.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    #pragma warning disable 0414
    public class EnhancedVideoPlayer : EnhancedBehaviour, ISkippableElement, IPermanentUpdate, ILoadingProcessor {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init | UpdateRegistration.Play;

        #region Loading Processor
        public override bool IsLoadingProcessor => true;

        // True while not yet prepared.
        public bool IsProcessing {
            get { return prepareOnInit && !IsPrepared; }
        }
        #endregion

        #region Global Members
        [Section("Enhanced Video Player")]

        #if UNITY_EDITOR
        [SerializeField, Enhanced, DrawMember("VideoClip"), ValidationMember("VideoClip")]
        private VideoClip videoClip = null; // Should never be used outside of the inspector.
        #endif

        [Space(5f)]

        [Tooltip("If true, the video will start playing right after the scene finished loading")]
        [SerializeField, Enhanced, ValidationMember("PlayAfterLoading")]
        private bool playAfterLoading = false;

        [Tooltip("If true, the video will initiate playaback engine preparation on initialization")]
        [SerializeField] private bool prepareOnInit = false;

        [Tooltip("If true, disables the associated VideoPlayer when it stops playing")]
        [SerializeField] private bool disableOnStop = false;

        [Space(10f)]

        [HelpBox("Whether the video can be skipped or not.\nHas no effect by itself. Should be used from another script (like an IBoundGameState).", MessageType.Info)]
        [SerializeField] private bool isSkippable = false;

        [Tooltip("The time (in seconds) where to advance the video to when being skipped.")]
        [SerializeField, Enhanced, ShowIf("isSkippable"), Range("playRange")] private double skipTime = 0d;

        [Space(10f)]

        [Tooltip("Use this to automatically show a CanvasGroup when the video start playing, and hide it when it stops")]
        [SerializeField] private bool manageRenderingCanvas = false;
        [SerializeField, Enhanced, ShowIf("manageRenderingCanvas")] private CrossSceneReference<CanvasGroup> renderingCanvas = null;

        [Space(10f)]

        [Tooltip("The total range used to play the video (from start to end)")]
        [SerializeField, Enhanced, DrawMember("PlayRange"), MinMax("TimeRange")] private Vector2 playRange = new Vector2(0f, 0f);

        #if UNITY_EDITOR
        [SerializeField, Enhanced, DrawMember("Time"), Range("playRange"), ValidationMember("Time")]
        private double time = 0d; // Should never be used outside of the inspector.
        #endif

        [Space(10f)]

        [SerializeField, Tooltip("GameState that will automatically be created when the video start playing, and destroyed when it stops")]
        private SerializedType<GameState> boundGameState = new SerializedType<GameState>(null, SerializedTypeConstraint.Null,
                                                                                         new Type[] { typeof(IBoundGameState),
                                                                                                      typeof(IBoundGameState<ISkippableElement>),
                                                                                                      typeof(IBoundGameState<EnhancedVideoPlayer>) });

        // -----------------------

        public bool IsSkippable {
            get { return isSkippable; }
            set {
                isSkippable = value;
            }
        }

        /// <summary>
        /// <inheritdoc cref="VideoPlayer.clip"/>
        /// (From <see cref="VideoPlayer.clip"/>)
        /// </summary>
        public VideoClip VideoClip {
            get {
                return videoPlayer.clip;
            } set {
                videoPlayer.clip = value;

                Stop();
                PlayRange = TimeRange;

                if (isSkippable) {
                    skipTime = PlayRange.y;
                }
            }
        }

        /// <summary>
        /// The time range used to play the video (start time as x value, end time as y value).
        /// </summary>
        public Vector2 PlayRange {
            get {
                float _time = (float)Time;
                playRange.x = Mathf.Min(_time, Mathm.CeilToDecimal(playRange.x, 2));
                playRange.y = Mathf.Max(_time, Mathm.FloorToDecimal(playRange.y, 2));

                return playRange;
            } set {
                playRange = value;
            }
        }

        /// <summary>
        /// Whether the Playable should start playing right after loading or not.
        /// </summary>
        public bool PlayAfterLoading {
            get { return playAfterLoading; }
            set {
                if (value) {
                    videoPlayer.playOnAwake = false;
                }

                playAfterLoading = value;
            }
        }

        /// <summary>
        /// The time (in seconds) where to advance the video to when being skipped.
        /// <para/>
        /// (See also <see cref="IsSkippable"/>)
        /// </summary>
        public double SkipTime {
            get { return skipTime; }
            set {
                skipTime = Mathm.Clamp(value, playRange.x, playRange.y);
            }
        }

        /// <summary>
        /// <inheritdoc cref="VideoPlayer.time"/>
        /// (From <see cref="VideoPlayer.time"/>)
        /// </summary>
        public double Time {
            get { return videoPlayer.time; }
            set {
                value = (double)Mathf.Clamp((float)value, playRange.x, playRange.y);
                videoPlayer.time = value;
            }
        }

        /// <summary>
        /// The range of the video total time (betwwen 0 and its duration).
        /// </summary>
        public Vector2 TimeRange {
            get { return new Vector2(0f, (float)Duration); }
        }

        // -----------------------

        #if UNITY_EDITOR
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, DrawMember("Duration"), ReadOnly] private double duration    = 0d;

        [Space(10f)]

        [SerializeField, Enhanced, DrawMember("IsPlaying"), ReadOnly] private bool isPlaying    = false;
        [SerializeField, Enhanced, DrawMember("IsPaused"), ReadOnly] private bool isPaused      = false;
        [SerializeField, Enhanced, DrawMember("IsPrepared"), ReadOnly] private bool isPrepared  = false;
        #endif

        /// <summary>
        /// The total duration of the video.
        /// </summary>
        public double Duration {
            get { return videoPlayer.length; }
        }

        /// <summary>
        /// Whether the video is currently playing or not.
        /// </summary>
        public bool IsPlaying {
            get { return videoPlayer.isPlaying; }
        }

        /// <summary>
        /// Whether the video is currently paused or not.
        /// </summary>
        public bool IsPaused {
            get { return videoPlayer.isPaused; }
        }

        /// <summary>
        /// Whether the video is already prepared or not.
        /// </summary>
        public bool IsPrepared {
            get { return videoPlayer.isPrepared; }
        }

        // -----------------------

        [HideInInspector] private VideoPlayer videoPlayer = null;

        // -----------------------

        /// <summary>
        /// Called when the video starts playing.
        /// </summary>
        public event Action<VideoPlayer> Started    = null;

        /// <summary>
        /// Called when the video is paused while playing.
        /// </summary>
        public event Action<VideoPlayer> Paused     = null;

        /// <summary>
        /// Called when the video is resumed after being paused.
        /// </summary>
        public event Action<VideoPlayer> Resumed    = null;

        /// <summary>
        /// Called when the video is stopped or completed.
        /// </summary>
        public event Action<VideoPlayer> Stopped    = null;
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Prepare the video.
            if (prepareOnInit) {
                Time = playRange.x;
                Prepare(OnPrepared);
            }

            // ----- Local Method ----- \\

            void OnPrepared() {
                prepareOnInit = false;
            }
        }

        protected override void OnPlay() {
            base.OnPlay();

            // Play once loading is over.
            if (playAfterLoading) {
                playAfterLoading = false;
                PlayFromStart();
            }
        }

        void IPermanentUpdate.Update() {
            OnPlayerUpdate();
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            if (isActive) {
                Stop();
            }
        }

        // -----------------------

        #if UNITY_EDITOR
        private void OnValidate() {
            if (!videoPlayer) {
                videoPlayer = GetComponent<VideoPlayer>();
            }

            // Range initialization.
            if (playRange == Vector2.zero) {
                playRange = TimeRange;
            }
        }
        #endif
        #endregion

        #region Behaviour
        private GameState gameState = null;
        private bool isActive       = false;

        // -----------------------

        /// <summary>
        /// <inheritdoc cref="VideoPlayer.Play"/>
        /// (From <see cref="VideoPlayer.Play"/>)
        /// </summary>
        [Button(SuperColor.Green)]
        public void Play() {
            // Do not execute if already playing.
            if (IsPlaying) {
                return;
            }

            if (!IsPrepared) {
                Prepare(Play);
                return;
            }

            // Enables the video, just in case.
            videoPlayer.enabled = true;

            // Ensure that the video time is correct.
            double _time = Time;
            if ((playRange.y - _time) < .1d) {
                _time = 0d;
            }

            Time = _time;
            videoPlayer.Play();

            // Active while started playing and not stopped yet.
            if (isActive) {
                Resumed?.Invoke(videoPlayer);
                return;
            }

            isActive = true;
            Started?.Invoke(videoPlayer);

            SetRenderCanvasAlpha(1f);

            // Stop callback.
            videoPlayer.loopPointReached -= OnStop;
            videoPlayer.loopPointReached += OnStop;

            #if UNITY_EDITOR
            // Editor behaviour.
            if (!Application.isPlaying) {
                videoPlayer.sendFrameReadyEvents = true;

                videoPlayer.frameReady -= OnFrameReady;
                videoPlayer.frameReady += OnFrameReady;
                return;
            }
            #endif

            // Game State.
            Type _gameStateType = boundGameState.Type;

            if ((_gameStateType != null) && !gameState.IsActive()) {
                gameState = GameState.CreateState(_gameStateType);
                
                // Bound this player.
                switch (gameState) {
                    case IBoundGameState<EnhancedVideoPlayer> _boundPlayer:
                        _boundPlayer.Bound(this);
                        break;

                    case IBoundGameState<ISkippableElement> _boundSkip:
                        _boundSkip.Bound(this);
                        break;

                    case IBoundGameState _bound:
                        _bound.Bound(this);
                        break;

                    default:
                        break;
                }
            }

            UpdateManager.Instance.Register(this, UpdateRegistration.Permanent);
        }

        /// <summary>
        /// <inheritdoc cref="VideoPlayer.Pause"/>
        /// (From <see cref="VideoPlayer.Pause"/>)
        /// </summary>
        [Button(SuperColor.Orange)]
        public void Pause() {
            if (IsPaused) {
                return;
            }

            videoPlayer.Pause();
            Paused?.Invoke(videoPlayer);
        }

        /// <summary>
        /// <inheritdoc cref="VideoPlayer.Stop"/>
        /// (From <see cref="VideoPlayer.Stop"/>)
        /// </summary>
        [Button(SuperColor.Crimson)]
        public void Stop() {
            videoPlayer.Stop();

            // Stopping the video does not call the "loopPointReached" event,
            // so call its callback manually.
            OnStop(videoPlayer);
        }

        /// <summary>
        /// Skips the video by setting its current time to <see cref="SkipTime"/>.
        /// </summary>
        /// <returns>True if this playable could be successfully skipped, false otherwise.</returns>
        [Button("IsSkippable", ConditionType.True, SuperColor.Lavender)]
        public bool Skip() {
            if (IsSkippable && (Time < SkipTime)) {
                Time = SkipTime;
                return true;
            }

            return false;
        }

        // -----------------------

        private void OnFrameReady(VideoPlayer _source, long _frameIdx) {
            OnPlayerUpdate();
        }

        private void OnPlayerUpdate() {
            if (Time >= playRange.y) {
                Stop();

                if (videoPlayer.isLooping) {
                    PlayFromStart();
                }
            }
        }

        // -----------------------

        private void OnStop(VideoPlayer _player) {
            // Not active if never being played or already stopped.
            if (!isActive) {
                return;
            }

            isActive = false;
            Stopped?.Invoke(videoPlayer);

            _player.loopPointReached -= OnStop;

            SetRenderCanvasAlpha(0f);

            // Clear the render texture to avoid displaying the last frame when playing a new video.
            if (videoPlayer.renderMode == VideoRenderMode.RenderTexture) {
                videoPlayer.targetTexture.Release();
            }

            #if UNITY_EDITOR
            // Editor behaviour.
            if (!Application.isPlaying) {
                videoPlayer.sendFrameReadyEvents = false;
                videoPlayer.frameReady -= OnFrameReady;
                return;
            }
            #endif

            // Game State.
            if (gameState.IsActive()) {
                gameState.RemoveState();
                gameState = null;
            }

            UpdateManager.Instance.Unregister(this, UpdateRegistration.Permanent);

            // Disables the video to make sure nothing is rendered anymore.
            if (disableOnStop) {
                videoPlayer.enabled = false;
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Plays this <see cref="VideoPlayer"/> from the start.
        /// </summary>
        public void PlayFromStart() {
            Time = playRange.x;
            Play();
        }

        /// <summary>
        /// <inheritdoc cref="VideoPlayer.Prepare"/>
        /// (From <see cref="VideoPlayer.Prepare"/>)
        /// </summary>
        /// <param name="_onPrepared">Called when the video has been fully prepared.</param>
        public void Prepare(Action _onPrepared = null) {
            if (IsPrepared) {
                _onPrepared?.Invoke();
                return;
            }

            if (_onPrepared != null) {
                videoPlayer.prepareCompleted -= OnPrepared;
                videoPlayer.prepareCompleted += OnPrepared;
            }

            videoPlayer.Prepare();

            // ----- Local Method ----- \\

            void OnPrepared(VideoPlayer _player){
                videoPlayer.prepareCompleted -= OnPrepared;
                _onPrepared.Invoke();
            }
        }

        // -----------------------

        /// <summary>
        /// Sets the alpha value of this object rendering <see cref="CanvasGroup"/>.
        /// </summary>
        /// <param name="_alpha">Rendering canvas new alpha value (between 0 and 1).</param>
        public void SetRenderCanvasAlpha(float _alpha) {
            if (manageRenderingCanvas) {
                renderingCanvas.GetReference().alpha = _alpha;
            }
        }

        /// <summary>
        /// Sets the clip of the associated <see cref="VideoPlayer"/>.
        /// </summary>
        /// <param name="_clip">The new clip of the video player.</param>
        /// <param name="_play">Whether to start this clip from the start or not.</param>
        /// <param name="_prepare">Whether to prepare the video or not.</param>
        public void SetClip(VideoClip _clip, bool _play = true, bool _prepare = true) {
            videoPlayer.clip = _clip;

            if (_play) {
                PlayFromStart();
            } else if (_prepare) {
                Prepare();
            } 
        }

        /// <summary>
        /// Get the <see cref="VideoPlayer"/> associated with this object.
        /// </summary>
        public VideoPlayer GetVideoPlayer() {
            return videoPlayer;
        }
        #endregion
    }
}
