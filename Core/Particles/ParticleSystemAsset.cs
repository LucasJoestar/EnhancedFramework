// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using UnityEngine;

namespace EnhancedFramework.Core {
	/// <summary>
	/// <see cref="EnhancedAssetFeedback"/> used to play various <see cref="ParticleSystem"/>.
	/// </summary>
	[CreateAssetMenu(fileName = "PSA_Particle", menuName = MenuPath + "Particle", order = MenuOrder)]
	public class ParticleSystemAsset : EnhancedAssetFeedback, IObjectPoolManager<EnhancedParticleSystemPlayer> {
		#region Global Members
		[Section("Particle System Asset")]

        [Tooltip("Particle systems wrapped in this asset")]
        [SerializeField] private BlockArray<ParticleSystem> particles = new BlockArray<ParticleSystem>();

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("If true, ensures that only one instance is playing at a time")]
        [SerializeField] private bool avoidDuplicate = false;

        [Tooltip("If true, keeps this particle playing while the game is paused")]
        [SerializeField] private bool ignorePause = false;

        [Space(5f)]

        [Tooltip("If true, clears this particle pool when performing a loading operation")]
        [SerializeField] private bool clearOnLoading = false;

        // -----------------------

        /// <summary>
        /// Determines if this particle should keeps playing while performing a scene loading operation.
        /// </summary>
        public bool ClearOnLoading {
            get { return clearOnLoading; }
        }


        /// <summary>
        /// Determines if this particle should keeps playing while performing a scene loading operation.
        /// </summary>
        public bool IgnorePause {
            get { return ignorePause; }
        }

        /// <summary>
        /// Determines if only one instance of this particle should be playing at a time.
        /// </summary>
        public bool AvoidDuplicate {
            get { return avoidDuplicate; }
        }
        #endregion

        #region Behaviour
        [NonSerialized] private ParticleHandler handler = default;

        // -----------------------

        /// <inheritdoc cref="PlayParticle(Transform)"/>
        public ParticleHandler PlayParticle(Vector3 _position) {
            return ParticleSystemManager.Instance.Play(this, _position);
        }

        /// <summary>
        /// Plays this particle system.
        /// </summary>
        /// <inheritdoc cref="ParticleSystemManager.Play(ParticleSystemAsset, Transform)"/>
        public ParticleHandler PlayParticle(Transform _transform) {
            return ParticleSystemManager.Instance.Play(this, _transform);
        }

        // -------------------------------------------
        // Handler
        // -------------------------------------------

        /// <summary>
        /// Sets this particle current handler.
        /// </summary>
        /// <param name="_player">This particle current handler.</param>
        internal void SetHandler(ParticleHandler _player) {
            handler = _player;
        }

        /// <summary>
        /// Get the last created <see cref="ParticleHandler"/> to play this particle.
        /// </summary>
        /// <param name="_player">Last created <see cref="ParticleHandler"/> for this particle.</param>
        /// <returns>True if this particle handler is valid, false otherwise.</returns>
        public bool GetHandler(out ParticleHandler _player) {
            _player = handler;
            return _player.IsValid;
        }

        // -------------------------------------------
        // Feedback
        // -------------------------------------------

        public override void Play(Transform _transform, FeedbackPlayOptions _options) {
            // Instant play (delay managed in the player).
            OnPlay(_transform, _options);
        }

        protected override void OnPlay(Transform _transform, FeedbackPlayOptions _options) {
            switch (_options) {

                // Play at position.
                case FeedbackPlayOptions.PlayAtPosition:
                    PlayParticle(_transform.position);
                    break;

                // Follow transform.
                case FeedbackPlayOptions.FollowTransform:
                    PlayParticle(_transform);
                    break;

                // Vanilla.
                case FeedbackPlayOptions.None:
                default:
                    this.LogErrorMessage("Particles cannot be played without a proper Transform reference");
                    break;
            }
        }

        protected override void OnStop() {
            handler.Stop();
        }
        #endregion

        #region Pool
        private ObjectPool<EnhancedParticleSystemPlayer> particlePool = new ObjectPool<EnhancedParticleSystemPlayer>(0);
        private bool isInitialize = false;

        // -----------------------

        /// <summary>
        /// Get a <see cref="EnhancedParticleSystemPlayer"/> instance from this pool.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Get"/>
        public EnhancedParticleSystemPlayer GetInstance() {

            if (!isInitialize) {

                particlePool.Initialize(this);
                isInitialize = true;
            }

            return particlePool.Get();
        }

        /// <summary>
        /// Releases a specific <see cref="EnhancedParticleSystemPlayer"/> instance and sent it back to this pool.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Release(T)"/>
        public bool ReleaseInstance(EnhancedParticleSystemPlayer _player) {
            return particlePool.Release(_player);
        }

        /// <summary>
        /// Clears this <see cref="EnhancedParticleSystemPlayer"/> pool content and destroys all its instances.
        /// </summary>
        /// <inheritdoc cref="ObjectPool{T}.Clear(int)"/>
        public void ClearPool(int _capacity = 1) {
            particlePool.Clear(_capacity);
        }

        // -------------------------------------------
        // Manager
        // -------------------------------------------

        /// <inheritdoc cref="IObjectPoolManager{EnhancedParticlePlayer}.CreateInstance"/>
        EnhancedParticleSystemPlayer IObjectPoolManager<EnhancedParticleSystemPlayer>.CreateInstance() {

            EnhancedParticleSystemPlayer _player = new GameObject("PSP_NewParticle").AddComponent<EnhancedParticleSystemPlayer>();
            Transform _transform = _player.transform;

            ParticleSystem _particle = Instantiate(GetParticle(), _transform);
            _particle.transform.ResetLocal();

            _transform.SetParent(ParticleSystemManager.Instance.GetPoolRoot(this));
            _transform.ResetLocal();

            _player.Initialize(this);
            return _player;
        }

        /// <inheritdoc cref="IObjectPoolManager{EnhancedParticlePlayer}.DestroyInstance(EnhancedParticlePlayer)"/>
        void IObjectPoolManager<EnhancedParticleSystemPlayer>.DestroyInstance(EnhancedParticleSystemPlayer _instance) {
            Destroy(_instance.gameObject);
        }
        #endregion

        #region Utility
        private int lastRandomIndex = -1;

        // -----------------------

        /// <summary>
        /// Get a random particles from this feedback.
        /// </summary>
        public ParticleSystem GetParticle() {
            return particles.Array.Random(ref lastRandomIndex);
        }
        #endregion
    }
}
