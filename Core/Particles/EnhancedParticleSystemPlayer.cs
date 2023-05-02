// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EnhancedParticleSystemPlayer"/>-related wrapper for a single play operation.
    /// </summary>
    public struct ParticleHandler : IHandler<EnhancedParticleSystemPlayer> {
        #region Global Members
        private Handler<EnhancedParticleSystemPlayer> handler;

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

        /// <inheritdoc cref="ParticleHandler(EnhancedParticleSystemPlayer, int)"/>
        public ParticleHandler(EnhancedParticleSystemPlayer _player) {
            handler = new Handler<EnhancedParticleSystemPlayer>(_player);
        }

        /// <param name="_player"><see cref="EnhancedParticleSystemPlayer"/> used for playing particles.</param>
        /// <param name="_id">ID of the associated particle play operation.</param>
        /// <inheritdoc cref="ParticleHandler"/>
        public ParticleHandler(EnhancedParticleSystemPlayer _player, int _id) {
            handler = new Handler<EnhancedParticleSystemPlayer>(_player, _id);
        }
        #endregion

        #region Utility
        public bool GetHandle(out EnhancedParticleSystemPlayer _player) {
            return handler.GetHandle(out _player) && (_player.Status != EnhancedParticleSystemPlayer.State.Inactive);
        }

        /// <summary>
        /// Plays or resumes this handle associated <see cref="EnhancedParticleSystemPlayer"/>.
        /// </summary>
        /// <inheritdoc cref="EnhancedParticleSystemPlayer.Play()"/>
        public bool Play() {
            if (!GetHandle(out EnhancedParticleSystemPlayer _player)) {
                return false;
            }

            return _player.Play();
        }

        /// <summary>
        /// Pauses this handle associated <see cref="EnhancedParticleSystemPlayer"/>.
        /// </summary>
        /// <inheritdoc cref="EnhancedParticleSystemPlayer.Pause"/>
        public bool Pause() {
            if (!GetHandle(out EnhancedParticleSystemPlayer _player)) {
                return false;
            }

            return _player.Pause();
        }

        /// <summary>
        /// Stops this handle associated <see cref="EnhancedParticleSystemPlayer"/>.
        /// </summary>
        /// <inheritdoc cref="EnhancedParticleSystemPlayer.Stop"/>
        public bool Stop(ParticleSystemStopBehavior _behaviour = ParticleSystemStopBehavior.StopEmitting) {
            if (!GetHandle(out EnhancedParticleSystemPlayer _player)) {
                return false;
            }

            return _player.Stop(_behaviour);
        }
        #endregion
    }

    /// <summary>
    /// <see cref="IPoolableObject"/> utility component used to play any <see cref="ParticleSystemAsset"/>.
    /// </summary>
    [AddComponentMenu(FrameworkUtility.MenuPath + "Particles/Particle System Player"), DisallowMultipleComponent]
    #pragma warning disable CS0414
    public class EnhancedParticleSystemPlayer : EnhancedPoolableObject, IHandle {
        #region State
        /// <summary>
        /// References all available states for an <see cref="EnhancedParticleSystemPlayer"/>.
        /// </summary>
        public enum State {
            Inactive    = 0,
            Playing     = 1,
            Paused      = 2,

            Delay       = 5,
        }
        #endregion

        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Play;

        #region Global Members
        [Section("Enhanced Particle Player")]

        [SerializeField, Enhanced, Required] private ParticleSystemAsset particleAsset = null;

        [Space(5f)]

        [Tooltip("If true, automatically starts playing this particle right after the scene finished loading")]
        [SerializeField] private bool playAfterLoading = false;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private State state = State.Inactive;
        [SerializeField, Enhanced, ReadOnly] private int playID = 0;

        [Space(10f)]

        [SerializeField, Enhanced, ReadOnly] private bool followTransform = false;
        [SerializeField, Enhanced, ReadOnly] private Transform referenceTransform = null;

        // -----------------------

        [HideInInspector] private ParticleSystem[] particleSystems = new ParticleSystem[0];

        // -----------------------

        /// <summary>
        /// Current state of this player.
        /// </summary>
        public State Status {
            get { return state; }
        }

        /// <summary>
        /// ID of the current particle play operation, especially used for <see cref="ParticleHandler"/> references.
        /// </summary>
        int IHandle.ID {
            get { return playID; }
        }

        /// <summary>
        /// The <see cref="ParticleSystemAsset"/> configured in this player.
        /// </summary>
        public ParticleSystemAsset ParticleAsset {
            get { return particleAsset;  }
        }

        /// <summary>
        /// Determines if this particle pool should be cleared when performing a loading operation.
        /// </summary>
        public bool ClearOnLoading {
            get { return particleAsset.ClearOnLoading; }
        }

        /// <summary>
        /// Get if this particle players should keep playing while paused.
        /// </summary>
        public bool IgnorePause {
            get { return particleAsset.IgnorePause; }
        }

        // -----------------------

        #if UNITY_EDITOR
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [SerializeField, Enhanced, DrawMember("Time"), Range("TimeRange"), ValidationMember("Time")]
        private float time = 0f;

        [Space(10f)]

        [SerializeField, Enhanced, DrawMember("Duration"), ReadOnly] private float duration = 0f;
        [SerializeField, Enhanced, DrawMember("IsParticlePlaying"), ReadOnly] private bool isPlaying = false;
        #endif

        /// <summary>
        /// <inheritdoc cref="ParticleSystem.time"/>
        /// (From <see cref="ParticleSystem.time"/>)
        /// </summary>
        public float Time {
            get {
                float _time = 0f;
                foreach (var _particle in particleSystems) {
                    _time = Mathf.Max(_time, _particle.time);
                }

                return _time;
            }
            set {
                foreach (var _particle in particleSystems) {
                    _particle.time = value;
                }
            }
        }

        /// <summary>
        /// The range of all associated <see cref="ParticleSystem"/> global time (betwwen 0 and there duration).
        /// </summary>
        public Vector2 TimeRange {
            get { return new Vector2(0f, Duration); }
        }

        /// <summary>
        /// The total duration of all associated <see cref="ParticleSystem"/> (in seconds).
        /// </summary>
        public float Duration {
            get {
                float _duration = 0f;
                foreach (var _particle in particleSystems) {
                    _duration = Mathf.Max(_duration, _particle.main.duration);
                }

                return _duration;
            }
        }

        /// <summary>
        /// Get if any <see cref="ParticleSystem"/> is currently playing or not.
        /// </summary>
        public bool IsParticlePlaying {
            get {
                foreach (var _particle in particleSystems) {
                    if (_particle.isPlaying) {
                        return true;
                    }
                }

                return false;
            }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnPlay() {
            base.OnPlay();

            // Play once loading is over.
            if (playAfterLoading) {

                playAfterLoading = false;
                Play();
            }
        }

        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Registration.
            ParticleSystemManager.RegisterPlayer(this);
        }

        /// <summary>
        /// Called from the <see cref="ParticleSystemManager"/> to update this player.
        /// </summary>
        internal void ParticleUpdate() {
            // State update.
            switch (state) {

                // Stop detection.
                case State.Playing:

                    bool _isAlive = false;

                    foreach (ParticleSystem _particle in particleSystems) {
                        if (_particle.IsAlive()) {
                            _isAlive = true;
                            break;
                        }
                    }

                    if (!_isAlive) {
                        Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
                    }

                    break;

                // Ignore.
                case State.Inactive:
                case State.Paused:
                case State.Delay:
                default:
                    return;
            }

            // Follow.
            UpdateFollow();
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Stop playing.
            Stop(ParticleSystemStopBehavior.StopEmittingAndClear);

            // Unregistration.
            ParticleSystemManager.UnregisterPlayer(this);
        }
        #endregion

        #region Behaviour
        private static int lastPlayID = 0;

        private DelayHandler delay = default;

        // -----------------------


        /// <inheritdoc cref="ParticleSystemManager.Play(ParticleSystemAsset, Vector3)"/>
        public ParticleHandler Play(Vector3 _position) {
            ParticleHandler _player = DoPlay();
            transform.position = _position;

            return _player;
        }

        /// <inheritdoc cref="ParticleSystemManager.Play(ParticleSystemAsset, Transform)"/>
        public ParticleHandler Play(Transform _transform) {
            ParticleHandler _player = DoPlay();
            FollowTransform(_transform);

            return _player;
        }

        private ParticleHandler DoPlay() {
            Stop();

            // Stop previous player.
            if (particleAsset.AvoidDuplicate && particleAsset.GetHandler(out ParticleHandler _currentHandler)) {

                this.LogWarning($"Interrupting previous particle - {particleAsset.name.Bold()}");
                _currentHandler.Stop();
            }

            // ID.
            playID = ++lastPlayID;

            #if UNITY_EDITOR
            // Hierachy utility.
            if (isFromPool) {
                gameObject.name = $"{gameObject.name.GetPrefix()}{particleAsset.name.RemovePrefix()} - [{playID}]";
            }
            #endif

            // Delay.
            float _delay = particleAsset.Delay;
            if (_delay  != 0f) {

                SetState(State.Delay);
                delay = Delayer.Call(particleAsset.Delay, OnPlay, true);

            } else {
                OnPlay();
            }

            // Play.
            ParticleHandler _handler = new ParticleHandler(this, playID);
            particleAsset.SetHandler(_handler);

            return _handler;

            // ----- Local Method ----- \\

            void OnPlay() {

                // Play.
                SetState(State.Playing);

                foreach (ParticleSystem _particle in particleSystems) {
                    _particle.Play(false);
                }
            }
        }

        // -------------------------------------------
        // Initialization
        // -------------------------------------------

        /// <summary>
        /// Initializes this player for a specific <see cref="ParticleSystemAsset"/>.
        /// <br/> Should only be called once, on creation.
        /// </summary>
        /// <param name="_particle"><see cref="ParticleSystemAsset"/> to initialize this player with.</param>
        public void Initialize(ParticleSystemAsset _particle) {
            particleAsset = _particle;
            particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        // -------------------------------------------
        // Core
        // -------------------------------------------

        /// <summary>
        /// Plays or resumes all this player pre-configured particles.
        /// </summary>
        /// <returns>True if this player could be successfully played, false otherwise.</returns>
        [Button(SuperColor.Green)]
        public bool Play() {

            #if UNITY_EDITOR
            if (!Application.isPlaying && (state == State.Playing)) {
                Stop(ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            #endif

            switch (state) {

                // Ignore.
                case State.Playing:
                    this.LogWarningMessage("Particles are already being played");
                    return false;

                // Resume.
                case State.Paused:
                    SetState(State.Playing);

                    foreach (ParticleSystem _particle in particleSystems) {
                        _particle.Play(false);
                    }
                    break;

                case State.Delay:
                    delay.Resume();
                    return true;

                // Play from start.
                case State.Inactive:
                    DoPlay();
                    return true;

                default:
                    break;
            }

            return false;
        }

        /// <summary>
        /// Pauses this particle player.
        /// </summary>
        /// <returns>True if this player could be successfully paused, false otherwise.</returns>
        [Button(SuperColor.Orange)]
        public bool Pause() {
            switch (state) {

                // Ignore.
                case State.Inactive:
                case State.Paused:
                    return false;

                case State.Delay:
                    delay.Pause();
                    return true;

                case State.Playing:
                default:
                    break;
            }

            // Pause.
            SetState(State.Paused);

            foreach (ParticleSystem _particle in particleSystems) {
                _particle.Pause(false);
            }

            return true;
        }

        /// <summary>
        /// Stops playing this particle player.
        /// </summary>
        /// <param name="_behaviour">Behaviour applied when stopping the particle(s).</param>
        /// <returns>True if this player could be successfully stopped, false otherwise.</returns>
        [Button(SuperColor.Crimson)]
        public bool Stop(ParticleSystemStopBehavior _behaviour = ParticleSystemStopBehavior.StopEmitting) {

            switch (state) {

                // Ignore.
                case State.Inactive:
                    return false;

                // Cancel delay.
                case State.Delay:
                    delay.Cancel();
                    break;

                case State.Playing:
                case State.Paused:
                default:
                    break;
            }

            foreach (ParticleSystem _particle in particleSystems) {
                _particle.Stop(false, _behaviour);
            }

            switch (_behaviour) {

                // Stop.
                case ParticleSystemStopBehavior.StopEmittingAndClear:

                    StopFollowTransform();
                    SetState(State.Inactive);

                    // Send back to pool.
                    if (isFromPool) {
                        particleAsset.ReleaseInstance(this);
                    }

                    break;

                case ParticleSystemStopBehavior.StopEmitting:
                default:
                    break;
            }

            return true;
        }
        #endregion

        #region Poolable
        private bool isFromPool = false;

        // -----------------------

        public override void OnCreated() {
            base.OnCreated();

            // Pool management.
            isFromPool = true;
        }

        public override void OnRemovedFromPool() {
            base.OnRemovedFromPool();

            #if UNITY_EDITOR
            // Push on top of the hierarchy.
            transform.SetAsFirstSibling();
            #endif
        }

        public override void OnSentToPool() {
            base.OnSentToPool();

            #if UNITY_EDITOR
            // Push to the hierarchy bottom.
            transform.SetAsLastSibling();
            #endif
        }

        // -----------------------

        protected override void SetActive(bool _isActive) {
            base.SetActive(_isActive);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Sets the current state of this player.
        /// </summary>
        /// <param name="_state">New state of this player.</param>
        private void SetState(State _state) {
            state = _state;
        }

        // -------------------------------------------
        // Follow
        // -------------------------------------------

        /// <summary>
        /// Makes this player follow a specific <see cref="Transform"/> reference.
        /// </summary>
        /// <param name="_transform"><see cref="Transform"/> reference to follow.</param>
        public void FollowTransform(Transform _transform) {
            followTransform = true;
            referenceTransform = _transform;

            UpdateFollow();
        }

        /// <summary>
        /// Stops following any reference <see cref="Transform"/>.
        /// </summary>
        public void StopFollowTransform() {
            followTransform = false;
            referenceTransform = null;
        }

        /// <summary>
        /// Updates this player to follow its reference transform.
        /// </summary>
        private void UpdateFollow() {
            if (followTransform) {
                transform.SetPositionAndRotation(referenceTransform.position, referenceTransform.rotation);
            }
        }
        #endregion
    }
}
