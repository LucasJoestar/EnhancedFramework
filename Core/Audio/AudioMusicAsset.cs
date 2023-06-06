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
    /// Determines how a music is interrupted while playing.
    /// </summary>
    public enum MusicInterruption {
        None    = 0,
        Pause   = 1,
        Stop    = 2,
    }

    /// <summary>
    /// <see cref="MusicPlayer"/>-related wrapper for a single music operation.
    /// </summary>
    public struct MusicHandler : IHandler<MusicPlayer> {
        #region Global Members
        private Handler<MusicPlayer> handler;

        // -----------------------

        public int ID {
            get { return handler.ID; }
        }

        public bool IsValid {
            get { return GetHandle(out _); }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="MusicHandler(MusicPlayer, int)"/>
        public MusicHandler(MusicPlayer _music) {
            handler = new Handler<MusicPlayer>(_music);
        }

        /// <param name="_music"><see cref="MusicPlayer"/> to handle.</param>
        /// <param name="_id">ID of the associated operation.</param>
        /// <inheritdoc cref="MusicHandler"/>
        public MusicHandler(MusicPlayer _music, int _id) {
            handler = new Handler<MusicPlayer>(_music, _id);
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="IHandler{T}.GetHandle(out T)"/>
        public bool GetHandle(out MusicPlayer _music) {
            return handler.GetHandle(out _music) && (_music.Status != MusicPlayer.State.Inactive);
        }

        /// <summary>
        /// Asks the player to resume this music.
        /// </summary>
        /// <returns>True if this music is valid, false otherwise.</returns>
        public bool Resume() {
            if (!GetHandle(out MusicPlayer _music)) {
                return false;
            }

            _music.ResumeQuery();
            return true;
        }

        /// <summary>
        /// Asks the player to pause this music.
        /// </summary>
        /// <param name="_instant">Whether to instantly pause the music or not.</param>
        /// <returns>True if this music is valid, false otherwise.</returns>
        public bool Pause(bool _instant) {
            if (!GetHandle(out MusicPlayer _music)) {
                return false;
            }

            _music.PauseQuery(_instant);
            return true;
        }

        /// <summary>
        /// Asks the player to stop this music.
        /// </summary>
        /// <param name="_instant">Whether to instantly stop the music or not.</param>
        /// <param name="_fallAsleep">If true, do not remove this music from its layer, but simply make it fall asleep (can be resumed later).</param>
        /// <returns>True if this music is valid, false otherwise.</returns>
        public bool Stop(bool _instant, bool _fallAsleep) {
            if (!GetHandle(out MusicPlayer _music)) {
                return false;
            }
            
            if (_fallAsleep) {
                _music.StopQuery(_instant);
            } else {
                _music.StopMusic(_instant);
            }

            return true;
        }
        #endregion
    }

    /// <summary>
    /// <see cref="AudioMusicAsset"/>-related transition class,
    /// used to stop and play the music while applying volume fade effects.
    /// </summary>
    [Serializable]
    public class MusicPlayer : IHandle, IPoolableObject {
        #region States
        /// <summary>
        /// References all available states for this music.
        /// </summary>
        public enum State {
            Inactive    = 0,
            Prepared    = 1,

            Active      = 2,
            Pause       = 3,
            Asleep      = 4,
            Stop        = 5,
        }

        /// <summary>
        /// References all available states for this music transition.
        /// </summary>
        public enum TransitionState {
            Inactive    = 0,
            FadeIn      = 1,
            FadeOut     = 2,
        }
        #endregion

        #region Global Members
        private int id = 0;

        private MusicInterruption interruptionMode = MusicInterruption.None;
        private LoopOverride loopOverride = LoopOverride.None;
        private AudioLayer layer = AudioLayer.Default;
        private AudioMusicAsset music = null;

        private AudioHandler audioHandler = default;
        private bool isOverridden = false;

        // -----------------------

        /// <inheritdoc cref="IHandle.ID"/>
        public int ID {
            get { return id; }
        }

        /// <summary>
        /// Current state of this transition.
        /// </summary>
        public State Status {
            get { return state; }
        }

        /// <summary>
        /// <see cref="AudioMusicAsset"/> of this player music.
        /// </summary>
        public AudioMusicAsset Music {
            get { return music; }
        }

        /// <summary>
        /// Layer on which this music is played.
        /// </summary>
        public AudioLayer Layer {
            get { return layer; }
        }

        /// <summary>
        /// Get if this music is currently playing.
        /// </summary>
        public bool IsPlaying {
            get {
                return audioHandler.GetHandle(out EnhancedAudioPlayer _player) && (_player.Status == EnhancedAudioPlayer.State.Playing);
            }
        }

        /// <summary>
        /// Whether this music is looping or not.
        /// </summary>
        public LoopOverride Loop {
            get { return loopOverride; }
            set {
                loopOverride = value;
                UpdateLoopOverride();
            }
        }
        #endregion

        #region Behaviour
        private static int lastID = 0;

        private TransitionState transitionState = TransitionState.Inactive;
        private State state = State.Inactive;

        private EnhancedAudioPlayer.State playerState = EnhancedAudioPlayer.State.Inactive;

        // -----------------------

        /// <summary>
        /// Setups this music player.
        /// </summary>
        /// <param name="_music"><see cref="AudioMusicAsset"/> to play.</param>
        /// <param name="_layer"><see cref="AudioLayer"/> on which to play this music.</param>
        /// <param name="_interruption">Determines how to interrupt musics playing on a lower layer.</param>
        /// <returns>This player object.</returns>
        internal MusicPlayer Setup(AudioMusicAsset _music, AudioLayer _layer, MusicInterruption _interruption) {

            // Setup.
            StopMusic(true, false);
            SetState(State.Prepared);

            music = _music;
            layer = _layer;
            loopOverride = LoopOverride.None;

            interruptionMode = _interruption;

            id = ++lastID;

            _music.SetHandler(new MusicHandler(this));
            return this;
        }

        // -------------------------------------------
        // Behaviour
        // -------------------------------------------

        /// <summary>
        /// Plays or resumes this music.
        /// </summary>
        private void PlayMusic() {

            // Override.
            if (isOverridden) {

                StopMusic(true);
                return;
            }

            bool _fadeIn = false;

            switch (state) {

                // Ignore.
                case State.Inactive:
                    return;

                // Only play if not active.
                case State.Active:

                    switch (playerState) {

                        case EnhancedAudioPlayer.State.Playing:
                        case EnhancedAudioPlayer.State.Delay:
                        default:

                            return;

                        case EnhancedAudioPlayer.State.Inactive:
                        case EnhancedAudioPlayer.State.Paused:
                            break;
                    }

                    break;

                // Fade in.
                case State.Prepared:
                    _fadeIn = true;
                    break;

                case State.Stop:

                    _fadeIn = transitionState == TransitionState.FadeOut;
                    break;

                // Play.
                case State.Pause:
                case State.Asleep:
                default:
                    break;
            }

            SetState(State.Active);

            // Fade in.
            if (_fadeIn) {

                FadeIn();
                return;
            }

            // Play audio.
            if (!audioHandler.Play()) {

                audioHandler = music.Music.PlayAudio();
                UpdateLoopOverride();
            }

            // Volume and state setup.
            if (transitionState == TransitionState.Inactive) {

                SetMusicVolume(1f);
                SetInterruptionVolume(0f);
                SetFadeInterruption(interruptionMode, true);

            } else {
                ResumeFade();
            }
        }

        /// <summary>
        /// Pauses this music.
        /// </summary>
        /// <param name="_instant">If true, instantly pauses this music.</param>
        private void PauseMusic(bool _instant = false) {

            // Override.
            if (isOverridden) {

                StopMusic(_instant);
                return;
            }

            switch (state) {

                // Ignore.
                case State.Inactive:
                    return;

                case State.Pause:
                case State.Asleep:
                case State.Stop:

                    // Only pause if instant, or player is active.
                    if (!_instant || (playerState != EnhancedAudioPlayer.State.Playing)) {
                        return;
                    }

                    break;

                case State.Prepared:
                case State.Active:
                default:
                    break;
            }

            SetState(State.Pause);
            audioHandler.Pause(_instant);
        }

        /// <summary>
        /// Puts the music to sleep and tamporarily stops it.
        /// </summary>
        private void FallAsleep(bool _instant = false) {

            // Override.
            if (isOverridden) {

                StopMusic(_instant);
                return;
            }

            switch (state) {

                // Ignore.
                case State.Inactive:
                    return;

                // Only stop if instant, or player is active.
                case State.Asleep:
                case State.Stop:

                    if (!_instant || (playerState == EnhancedAudioPlayer.State.Inactive)) {
                        return;
                    }

                    break;

                case State.Prepared:
                case State.Active:
                case State.Pause:
                default:
                    break;
            }

            SetState(State.Asleep);
            audioHandler.Stop(_instant);
        }

        /// <summary>
        /// Stops playing this music, fade it out and remove it from its layer.
        /// </summary>
        /// <param name="_instant">If true, instantly stops this music.</param>
        internal void StopMusic(bool _instant, bool _sendToPool = true) {

            switch (state) {

                // Ignore.
                case State.Inactive:
                    return;

                // Only stop if instant.
                case State.Stop:

                    if (!_instant) {
                        return;
                    }

                    break;

                case State.Prepared:
                case State.Active:
                case State.Pause:
                case State.Asleep:
                default:
                    break;
            }

            SetState(State.Stop);

            // Stop operation.
            if (_instant || isOverridden) {
                audioHandler.Stop(_instant, OnStopped);
            } else {
                FadeOut();
            }

            // Override is stopping object definitely.
            if (_sendToPool && (state != State.Inactive)) {
                SetOverridden(true);
            }

            // ----- Local Method ----- \\

            void OnStopped() {

                Reset();

                if (_sendToPool) {
                    AudioManager.Instance.OnMusicStopped(this);
                }
            }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        /// <summary>
        /// Resets this player state and configuration.
        /// </summary>
        private void Reset() {

            // Operations.
            StopFade();
            SetOverridden(false);

            // State.
            SetState(State.Inactive);
            SetTransitionState(TransitionState.Inactive);
            SetPlayerState(EnhancedAudioPlayer.State.Inactive);

            SetQuery(MusicInterruption.None, false);
            SetFadeInterruption(MusicInterruption.None, false);

            // Volume.
            SetMusicVolume(0f);
            SetInterruptionVolume(1f);
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates this music player.
        /// </summary>
        /// <param name="_interruption"><see cref="MusicInterruption"/> to apply on this music.</param>
        /// <param name="_instant">Whether to instantly apply transition or not.</param>
        /// <param name="_interruptionVolume">Interruption music modifier value..</param>
        /// <returns>Volume modifier to apply on all musics playing on a lower layer.</returns>
        internal float Update(ref MusicInterruption _interruption, ref bool _instant, float _interruptionVolume) {

            UpdateAudioPlayer(_interruptionVolume);
            UpdateState(_interruption, _instant);
            UpdateFade();

            // Don't override anything.
            if (state == State.Inactive) {
                return 1f;
            }

            // Update lower music interruption.
            if (fadeInterruption > _interruption) {
                _interruption = fadeInterruption;
            }

            if (fadeInterruption != MusicInterruption.None) {
                _instant = instantFade;
            }

            // Interruption volume.
            return InterruptionVolume;
        }

        // -------------------------------------------
        // Updates
        // -------------------------------------------

        /// <summary>
        /// <see cref="EnhancedAudioPlayer"/>-related update.
        /// </summary>
        private void UpdateAudioPlayer(float _interruptionVolume) {

            EnhancedAudioPlayer.State _state;

            // Audio player.
            if (audioHandler.GetHandle(out EnhancedAudioPlayer _player)) {

                _player.PushVolumeModifier(AudioPlayerModifier.MusicSource, MusicVolume);
                _player.PushVolumeModifier(AudioPlayerModifier.MusicInterruption, _interruptionVolume);

                _state = _player.Status;
            } else {
                _state = EnhancedAudioPlayer.State.Inactive;
            }

            // Update this music state according to its Audio Player state.
            if (_state != playerState) {

                switch (_state) {

                    // Resume.
                    case EnhancedAudioPlayer.State.Playing:
                    case EnhancedAudioPlayer.State.Delay:
                        PlayMusic();
                        break;

                    // Pause.
                    case EnhancedAudioPlayer.State.Paused:
                        PauseMusic(true);
                        break;

                    // Stop.
                    case EnhancedAudioPlayer.State.Inactive:
                    default:

                        switch (state) {

                            // Stop when the music ends (if not looping).
                            case State.Active:
                                StopMusic(true);
                                break;

                            case State.Stop:

                                if (transitionState == TransitionState.Inactive) {
                                    StopMusic(true);
                                }

                                break;

                            // Already processing, so ignore.
                            case State.Inactive:
                            case State.Prepared:
                            case State.Pause:
                            case State.Asleep:
                            default:
                                break;
                        }

                        break;
                }

                SetPlayerState(_state);
            }
        }

        /// <summary>
        /// Global state and interruption update.
        /// </summary>
        private void UpdateState(MusicInterruption _interruption, bool _instant) {

            // Query.
            if (_interruption == MusicInterruption.None) {
                _interruption = queryInterruption;
            }

            if (!_instant && (_interruption == queryInterruption)) {
                _instant = instantQuery;
            }

            switch (_interruption) {

                // Pause.
                case MusicInterruption.Pause:
                    PauseMusic(_instant);
                    return;

                // Stop.
                case MusicInterruption.Stop:
                    FallAsleep(_instant);
                    return;

                case MusicInterruption.None:
                default:
                    break;
            }

            switch (state) {

                // Ignore.
                case State.Inactive:
                    return;

                case State.Prepared:
                case State.Active:
                case State.Pause:
                case State.Asleep:
                case State.Stop:
                default:
                    break;
            }

            // Play.
            if (!isOverridden) {
                PlayMusic();
            }
        }

        /// <summary>
        /// Transition fade update.
        /// </summary>
        private void UpdateFade() {

            // Ignore if inactive.
            if (transitionState == TransitionState.Inactive) {
                return;
            }

            // Ignore if there is any processing operation.
            if (delay.IsValid || sourceVolumeTween.IsValid || transitionVolumeTween.IsValid) {
                return;
            }

            switch (transitionState) {

                // Interruption after fade in.
                case TransitionState.FadeIn:
                    SetFadeInterruption(interruptionMode, true);
                    break;

                // Stop music after fade out.
                case TransitionState.FadeOut:
                    StopMusic(true);
                    break;

                case TransitionState.Inactive:
                default:
                    break;
            }

            SetTransitionState(TransitionState.Inactive);
        }
        #endregion

        #region Transitions
        private TweenHandler transitionVolumeTween = default;
        private TweenHandler sourceVolumeTween = default;
        private DelayHandler delay = default;

        private MusicInterruption fadeInterruption = MusicInterruption.None;
        private bool instantFade = false;

        // -----------------------

        /// <summary>
        /// Plays this music fade in transition.
        /// <br/> Applied when played immediatly after being prepared, or when interrupting a fade out.
        /// </summary>
        private void FadeIn() {

            AudioMusicAsset.TransitionSettings _settings = music.PlaySettings;
            float _delay = _settings.PlayDelay;

            // Setup.
            StopFade();
            SetTransitionState(TransitionState.FadeIn);

            // Transition interruption.
            if (AudioManager.Instance.IsMusicPlayingOnLowerLayer(this)) {

                MusicInterruption _interruption;
                if (_settings.OverrideFadeOut) {

                    // Fade tween.
                    transitionVolumeTween = Tweener.Tween(0f, interruptionVolume, SetInterruptionVolume, _settings.FadeOutDuration, _settings.FadeOutCurve, true, OnTweenComplete);

                    if (_settings.WaitForFadeOut) {
                        _delay += _settings.FadeOutDuration;
                    }

                    // Transition fade.
                    _interruption = MusicInterruption.None;
                } else {

                    // Standard music fade.
                    _interruption = interruptionMode;
                }

                SetFadeInterruption(_interruption, false);
            } else {

                // Instant transition if there is no music to interrupt.
                OnTweenComplete(true);
            }

            // Play.
            delay = Delayer.Call(_delay, OnPlay, true);

            // ----- Local Methods ----- \\

            void OnTweenComplete(bool _complete) {

                if (!_complete) {
                    return;
                }

                SetFadeInterruption(interruptionMode, true);
            }

            void OnPlay() {

                PlayMusic();
                AudioMusicAsset.TransitionSettings _settings = music.PlaySettings;

                // Fade in.
                if (audioHandler.GetHandle(out EnhancedAudioPlayer _player)) {

                    AnimationCurve _fadeInCurve = _settings.OverrideFadeIn ? _settings.FadeInCurve      : _player.AudioAsset.FadeInCurve;
                    float _fadeInDuration       = _settings.OverrideFadeIn ? _settings.FadeInDuration   : _player.AudioAsset.FadeInDuration;

                    sourceVolumeTween = Tweener.Tween(sourceVolume, 1f, SetMusicVolume, _fadeInDuration, _fadeInCurve, true, OnComplete);
                } else {
                    SetMusicVolume(1f);
                }
            }

            void OnComplete(bool _complete) { }
        }

        /// <summary>
        /// Plays this music fade out transition.
        /// <br/> Applied when stopping playing this music, but not when overridden or being interrupted.
        /// </summary>
        private void FadeOut() {

            AudioMusicAsset.TransitionSettings _settings = music.StopSettings;
            float _delay = _settings.PlayDelay;

            // Setup.
            StopFade();
            SetTransitionState(TransitionState.FadeOut);

            // We do not want to stop this music, as it would also stop the transition and remove if from the Audio Manager.
            // Instead, simply fade out its volume.
            if (audioHandler.GetHandle(out EnhancedAudioPlayer _player)) {

                AnimationCurve _fadeOutCurve = _settings.OverrideFadeOut ? _settings.FadeOutCurve       : _player.AudioAsset.FadeOutCurve;
                float _fadeOutDuration       = _settings.OverrideFadeOut ? _settings.FadeOutDuration    : _player.AudioAsset.FadeOutDuration;

                sourceVolumeTween = Tweener.Tween(0f, sourceVolume, SetMusicVolume, _fadeOutDuration, _fadeOutCurve, true, OnTweenComplete);

                if (_settings.WaitForFadeOut) {
                    _delay += _fadeOutDuration;
                }
            } else {
                SetMusicVolume(0f);
            }

            // Resume other musics.
            delay = Delayer.Call(_delay, OnResume, true);

            // ----- Local Methods ----- \\

            void OnTweenComplete(bool _complete) { }

            void OnResume() {

                // We cannot resume the other audio for fade in,
                // as we don't know which one should be playing and that they might all have a different fade duration.
                //
                // So instead, use a fixed but quick fade in tween.
                AudioMusicAsset.TransitionSettings _settings = music.StopSettings;
                const float DefaultFadeInDuration = .5f;

                AnimationCurve _fadeInCurve = _settings.OverrideFadeIn ? _settings.FadeInCurve       : AnimationCurve.Linear(0f, 0f, 1f, 1f);
                float _fadeInDuration       = _settings.OverrideFadeIn ? _settings.FadeInDuration    : DefaultFadeInDuration;

                transitionVolumeTween = Tweener.Tween(interruptionVolume, 1f, SetInterruptionVolume, _fadeInDuration, _fadeInCurve, true, OnComplete);
                SetFadeInterruption(MusicInterruption.None, false);
            }

            void OnComplete(bool _complete) { }
        }

        // -------------------------------------------
        // Utility
        // -------------------------------------------

        /// <summary>
        /// Resumes the current fade operation.
        /// </summary>
        private void ResumeFade() {

            // Ignore if inactive.
            if (transitionState == TransitionState.Inactive) {
                return;
            }

            delay.Resume();
            sourceVolumeTween.Resume();
            transitionVolumeTween.Resume();
        }

        /// <summary>
        /// Pauses the current fade operation.
        /// </summary>
        private void PauseFade() {

            // Ignore if inactive.
            if (transitionState == TransitionState.Inactive) {
                return;
            }

            delay.Pause();
            sourceVolumeTween.Pause();
            transitionVolumeTween.Pause();
        }

        /// <summary>
        /// Stops the current fade operation.
        /// </summary>
        private void StopFade() {

            // Ignore if inactive.
            if (transitionState == TransitionState.Inactive) {
                return;
            }

            delay.Cancel();
            sourceVolumeTween.Stop();
            transitionVolumeTween.Stop();
        }

        /// <summary>
        /// Completes the current fade operation.
        /// </summary>
        private void CompleteFade() {

            // Ignore if inactive.
            if (transitionState == TransitionState.Inactive) {
                return;
            }

            delay.Complete();
            sourceVolumeTween.Complete();
            transitionVolumeTween.Complete();
        }

        // -----------------------

        /// <summary>
        /// Sets the current interruption state to be applied on musics played on a lower layer.
        /// </summary>
        /// <param name="_interruption">Interruption to be applied.</param>
        /// <param name="_instant">Whether to instantly apply interruption or not.</param>
        private void SetFadeInterruption(MusicInterruption _interruption, bool _instant) {
            fadeInterruption = _interruption;
            instantFade = _instant;
        }
        #endregion

        #region Query
        private MusicInterruption queryInterruption = MusicInterruption.None;
        private bool instantQuery = false;

        // -----------------------

        /// <summary>
        /// Sends a query to play this music.
        /// </summary>
        public void ResumeQuery() {
            SetQuery(MusicInterruption.None, true);
        }

        /// <summary>
        /// Sends a query to pause this music.
        /// </summary>
        /// <param name="_instant">Whether to instantly pause the music or not.</param>
        public void PauseQuery(bool _instant) {
            SetQuery(MusicInterruption.Pause, _instant);
        }

        /// <summary>
        /// Sends a query to stop this music.
        /// </summary>
        /// <param name="_instant">Whether to instantly stop the music or not.</param>
        public void StopQuery(bool _instant) {
            SetQuery(MusicInterruption.Stop, _instant);
        }

        // -----------------------

        private void SetQuery(MusicInterruption _query, bool _instant) {
            queryInterruption = _query;
            instantQuery = _instant;
        }
        #endregion

        #region Volume
        private float sourceVolume = 0f;
        private float interruptionVolume = 0f;

        /// <summary>
        /// Volume modifier of this music.
        /// </summary>
        public float MusicVolume {
            get { return sourceVolume; }
        }

        /// <summary>
        /// Volume modifier of all musics interrupted by this one.
        /// </summary>
        public float InterruptionVolume {
            get { return interruptionVolume; }
        }

        // -----------------------

        /// <summary>
        /// Sets the volume modifier of this music.
        /// </summary>
        /// <param name="_volume">Modifier volume value.</param>
        private void SetMusicVolume(float _volume) {
            sourceVolume = _volume;
        }

        /// <summary>
        /// Sets the volume modifier of all musics interrupted by this one.
        /// </summary>
        /// <param name="_volume">Modifier volume value.</param>
        private void SetInterruptionVolume(float _volume) {
            interruptionVolume = _volume;
        }
        #endregion

        #region Pool
        void IPoolableObject.OnCreated() {

            // Initialization.
            Reset();
        }

        void IPoolableObject.OnRemovedFromPool() { }

        void IPoolableObject.OnSentToPool() {

            // Security.
            StopMusic(true, false);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Set whether this music is overridden or not.
        /// <br/> Overridden musics should be definitely killed and removed whenever interrupted.
        /// </summary>
        /// <param name="_isOverridden">True if this music is overridden, false otherwise.</param>
        public void SetOverridden(bool _isOverridden) {
            isOverridden = _isOverridden;
        }

        /// <summary>
        /// Update this music loop override.
        /// </summary>
        public void UpdateLoopOverride() {

            if ((loopOverride == LoopOverride.None) || !audioHandler.GetHandle(out EnhancedAudioPlayer _player)) {
                return;
            }

            _player.Loop = loopOverride == LoopOverride.Loop;
        }

        // -------------------------------------------
        // States
        // -------------------------------------------

        /// <summary>
        /// Sets the state of this music.
        /// </summary>
        /// <param name="_state">New state of this music.</param>
        private void SetState(State _state) {
            state = _state;
        }

        /// <summary>
        /// Sets the state of this music transition.
        /// </summary>
        /// <param name="_state">New state of this music transition.</param>
        private void SetTransitionState(TransitionState _state) {
            transitionState = _state;
        }

        /// <summary>
        /// Sets the state of this music <see cref="EnhancedAudioPlayer"/>.
        /// </summary>
        /// <param name="_state">New state of this music player.</param>
        private void SetPlayerState(EnhancedAudioPlayer.State _state) {
            playerState = _state;
        }
        #endregion
    }

    /// <summary>
    /// <see cref="ScriptableObject"/> data holder for an audio-related music asset.
    /// </summary>
    [CreateAssetMenu(fileName = "MSC_MusicAsset", menuName = FrameworkUtility.MenuPath + "Audio/Music", order = FrameworkUtility.MenuOrder)]
    public class AudioMusicAsset : EnhancedScriptableObject {
        /// <summary>
        /// Settings used to perform a music transition.
        /// </summary>
        [Serializable]
        public class TransitionSettings {
            #region Global Members
            [Tooltip("If true, overrides transition fade out parameters")]
            public bool OverrideFadeOut = false;

            [Tooltip("Transition fade out duration (in seconds)")]
            [Enhanced, ShowIf("OverrideFadeOut"), Range(0f, 60f)] public float FadeOutDuration = 1f;

            [Tooltip("Transition fade out evaluation curve")]
            [Enhanced, ShowIf("OverrideFadeOut"), EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Crimson)]
            public AnimationCurve FadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

            [Space(15f)]

            [Tooltip("If true, waits for the current music fade out before fading in the new one")]
            public bool WaitForFadeOut = true;

            [Tooltip("Delay before playing and fading in the music")]
            [Enhanced, Range(0f, 60f)] public float PlayDelay = 0f;

            [Space(15f)]

            [Tooltip("If true, overrides transition fade in parameters")]
            public bool OverrideFadeIn = false;

            [Tooltip("Transition fade in duration (in seconds)")]
            [Enhanced, ShowIf("OverrideFadeIn"), Range(0f, 60f)] public float FadeInDuration = 1f;

            [Tooltip("Transition fade in evaluation curve")]
            [Enhanced, ShowIf("OverrideFadeIn"), EnhancedCurve(0f, 0f, 1f, 1f, SuperColor.Green)]
            public AnimationCurve FadeInCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            #endregion
        }

        #region Global Members
        [Section("Music Asset")]

        [Tooltip("Music audio asset to play")]
        [SerializeField, Enhanced, Required] private AudioAsset music = null;

        [Space(5f)]

        [Tooltip("Default mode used to determines how current music(s) are interrupted when starting to play this one")]
        [SerializeField] private MusicInterruption interruptionMode = MusicInterruption.Pause;

        [Tooltip("Default layer on which to play this music.\nOnly the music on the highest layer priority is actively played")]
        [SerializeField] private AudioLayer layer = AudioLayer.Default;

        [Space(15f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Space(10f, order = 0), Title("Play Transition", order = 1), Space(5f, order = 2)]

        [HelpBox("Transition settings used when interrupting the current music (not whenever played)\nFades out the current music, and fades in this one", MessageType.Info, false)]
        [SerializeField, Enhanced, Block] private TransitionSettings playSettings = new TransitionSettings();

        [Space(15f), HorizontalLine(SuperColor.Grey, 1f), Space(5f)]

        [Space(10f, order = 0), Title("Stop Transition", order = 1), Space(5f, order = 2)]

        [HelpBox("Transition settings used when stopping this music (not when interrupted)\nFades out this music, and fades in this new one", MessageType.Info, false)]
        [SerializeField, Enhanced, Block] private TransitionSettings stopSettings = new TransitionSettings();

        // -----------------------

        /// <summary>
        /// <see cref="AudioAsset"/> wrapped in this music.
        /// </summary>
        public AudioAsset Music {
            get { return music; }
        }

        /// <summary>
        /// Audio layer on which to play this music.
        /// </summary>
        public AudioLayer Layer {
            get { return layer; }
        }

        /// <summary>
        /// Mode used to interrupt the current music(s) when playing this one.
        /// </summary>
        public MusicInterruption InterruptionMode {
            get { return interruptionMode; }
        }

        /// <summary>
        /// Transition settings used when playing this music.
        /// </summary>
        public TransitionSettings PlaySettings {
            get { return playSettings; }
        }

        /// <summary>
        /// Transition settings used when stopping playing this music.
        /// </summary>
        public TransitionSettings StopSettings {
            get { return stopSettings; }
        }
        #endregion

        #region Behaviour
        [NonSerialized] private MusicHandler handler = default;

        // -----------------------

        /// <inheritdoc cref="PlayMusic(AudioLayer, MusicInterruption)"/>
        public MusicHandler PlayMusic() {
            return PlayMusic(layer, interruptionMode);
        }

        /// <inheritdoc cref="PlayMusic(AudioLayer, MusicInterruption)"/>
        public MusicHandler PlayMusic(AudioLayer _layer) {
            return PlayMusic(_layer, interruptionMode);
        }

        /// <summary>
        /// Plays this music.
        /// </summary>
        /// <inheritdoc cref="AudioManager.PlayMusic(AudioMusicAsset, AudioLayer, MusicInterruption)"/>
        public MusicHandler PlayMusic(AudioLayer _layer, MusicInterruption _interruptionMode) {
            return AudioManager.Instance.PlayMusic(this, _layer, _interruptionMode);
        }

        // -------------------------------------------
        // Handler
        // -------------------------------------------

        /// <summary>
        /// Sets this music current handler.
        /// </summary>
        /// <param name="_handler">This music current handler.</param>
        internal void SetHandler(MusicHandler _handler) {
            handler = _handler;
        }

        /// <summary>
        /// Get the last created <see cref="MusicHandler"/> to play this music.
        /// </summary>
        /// <param name="_handler">Last created <see cref="MusicHandler"/> for this music.</param>
        /// <returns>True if this music handler is valid, false otherwise.</returns>
        public bool GetHandler(out MusicHandler _handler) {
            _handler = handler;
            return _handler.IsValid;
        }

        // -------------------------------------------
        // Buttons
        // -------------------------------------------

        /// <summary>
        /// Plays this music (for editor use).
        /// </summary>
        [Button(ActivationMode.Play, SuperColor.Green, IsDrawnOnTop = false), DisplayName("Play")]
        private void PlayDebug(bool _resume) {

            if (_resume && GetHandler(out MusicHandler _handler)) {
                _handler.Resume();
            } else {
                PlayMusic();
            }
        }

        /// <summary>
        /// Pauses this music (for editor use).
        /// </summary>
        [Button(ActivationMode.Play, SuperColor.Orange, IsDrawnOnTop = false), DisplayName("Pause")]
        private void PauseDebug(bool _instant) {
            if (GetHandler(out MusicHandler _handler)) {
                _handler.Pause(_instant);
            }
        }

        /// <summary>
        /// Stops this music (for editor use).
        /// </summary>
        [Button(ActivationMode.Play, SuperColor.Crimson, IsDrawnOnTop = false), DisplayName("Stop")]
        private void StopDebug(bool _instant, bool _fallAsleep) {
            if (GetHandler(out MusicHandler _handler)) {
                _handler.Stop(_instant, _fallAsleep);
            }
        }
        #endregion
    }
}
