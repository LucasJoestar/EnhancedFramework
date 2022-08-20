// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EnhancedFramework.Chronos")]
namespace EnhancedFramework.Core {
    /// <summary>
    /// Base class to derive every <see cref="MonoBehaviour"/> of the project from.
    /// <para/>
    /// The <see cref="EnhancedBehaviour"/> uses component activation callbacks to automatically
    /// register/unregister itself from updates, avoiding using Unity update callbacks
    /// for a better result at lower cost.
    /// <br/> You can register to a variety of callbacks using the <see cref="UpdateRegistration"/> flag.
    /// <para/>
    /// Comes also with basic activation/deactivation callbacks that ensure being called
    /// when its state really changes.
    /// <br/> You can use the OnPaused callback to implement specific behaviours when the object gets paused/unpaused,
    /// which can happen in a variety of situations like a combo or a slow motion effect.
    /// </summary>
    public class EnhancedBehaviour : MonoBehaviour, IBaseUpdate {
        #region Update Registration
        /// <summary>
        /// Override this to specify this object update registration.
        /// <para/>
        /// Use "base.UpdateRegistration | <see cref="EnhancedFramework.UpdateRegistration"/>.value"
        /// to add a new registration or override its value
        /// </summary>
        public virtual UpdateRegistration UpdateRegistration => 0;

        /// <summary>
        /// Object used for console logging, using <see cref="UpdateManager"/> update system.
        /// </summary>
        Object IBaseUpdate.GetLogObject {
            get {
                return this;
            }
        }
        #endregion

        #region Global Members
        [Section("Enhanced Behaviour")]

        [SerializeField, Enhanced, ReadOnly] protected float chronos = 1f;

        /// <summary>
        /// This object-related time scale.
        /// </summary>
        public float Chronos {
            get => chronos;
            internal set {
                bool doPause = (chronos == 0f) != (value == 0f);
                chronos = value;

                if (doPause) {
                    OnPaused(value == 0f);
                }
            }
        }

        /// <summary>
        /// Object-related version of <see cref="Time.deltaTime"/>.
        /// </summary>
        public float DeltaTime {
            get {
                return Time.deltaTime * chronos;
            }
        }

        /// <summary>
        /// A smooth verison of this object-related version of <see cref="Time.deltaTime"/>.
        /// </summary>
        public float SmoothDeltaTime {
            get {
                return Time.smoothDeltaTime * chronos;
            }
        }

        /// <summary>
        /// <inheritdoc cref="Component.transform"/>
        /// Using an overridable property for optimization purpose.
        /// </summary>
        public virtual Transform Transform {
            get {
                return transform;
            }
        }
        #endregion

        #region Enhanced Behaviour
        protected virtual void Awake() {
            // Implement object initialization registration here.
        }

        protected virtual void OnEnable() {
            OnBehaviourEnabled();
        }

        protected virtual void OnDisable() {
            if (!GameManager.IsQuittingApplication) {
                OnBehaviourDisabled();
            }
        }
        #endregion

        #region State Callbacks
        /// <summary>
        /// Called when this object pause state is changed.
        /// </summary>
        protected virtual void OnPaused(bool _isPaused) { }

        /// <summary>
        /// Called when this behaviour is being enabled.
        /// </summary>
        protected virtual void OnBehaviourEnabled() {
            if (UpdateRegistration != 0) {
                UpdateManager.Instance.Register(this, UpdateRegistration);
            }
        }

        /// <summary>
        /// Called when this behaviour is being disabled.
        /// </summary>
        protected virtual void OnBehaviourDisabled() {
            if (UpdateRegistration != 0) {
                UpdateManager.Instance.Unregister(this, UpdateRegistration);
            }
        }
        #endregion

        #region Animation
        /// <summary>
        /// Calls an <see cref="EnhancedAnimationEvent"/> on this behaviour.
        /// <br/> This is an alternative to direct animation events, allowing more controls on calls
        /// and reference management.
        /// <para/>
        /// This method should only be called from an animation.
        /// </summary>
        /// <param name="_event">Event to call.</param>
        public void AnimationEvent(EnhancedAnimationEvent _event) {
            _event.Invoke(this);
        }
        #endregion

        #region Comparison
        /// <summary>
        /// Compare two objects by their instance id.
        /// </summary>
        /// <returns>True if they are the same, false otherwise.</returns>
        public bool Compare(EnhancedBehaviour _other) {
            return GetInstanceID() == _other.GetInstanceID();
        }
        #endregion

        #region Utility
        /// <inheritdoc cref="TransformExtensions.RelativeVector(Transform, Vector3)"/>
        public Vector3 GetRelativeVector(Vector3 _vector) {
            return Transform.RelativeVector(_vector);
        }

        /// <inheritdoc cref="TransformExtensions.WorldVector(Transform, Vector3)"/>
        public Vector3 GetWorldVector(Vector3 _vector) {
            return Transform.WorldVector(_vector);
        }

        // -----------------------

        /// <summary>
        /// <inheritdoc cref="GetRelativeVector(Vector3)"/>
        /// <para/>
        /// This method overload can be used when this transform rotation is already cached in a value.
        /// <br/> This only differs from <see cref="VectorExtensions.RotateInverse(Vector3, Quaternion)"/>
        /// in its signature, to maintain a coherent architecture.
        /// </summary>
        /// <param name="_rotation">Rotation of the object.</param>
        /// <inheritdoc cref="GetRelativeVector(Vector3)"/>
        public Vector3 GetRelativeVector(Vector3 _vector, Quaternion _rotation) {
            return _vector.RotateInverse(_rotation);
        }

        /// <summary>
        /// <inheritdoc cref="GetWorldVector(Vector3)"/>
        /// <para/>
        /// This method overload can be used when this transform rotation is already cached in a value.
        /// <br/> This only differs from <see cref="VectorExtensions.Rotate(Vector3, Quaternion)"/>
        /// in its signature, to maintain a coherent architecture.
        /// </summary>
        /// <param name="_rotation">Rotation of the object.</param>
        /// <inheritdoc cref="GetWorldVector(Vector3)"/>
        public Vector3 GetWorldVector(Vector3 _vector, Quaternion _rotation) {
            return _vector.Rotate(_rotation);
        }
        #endregion
    }
}
