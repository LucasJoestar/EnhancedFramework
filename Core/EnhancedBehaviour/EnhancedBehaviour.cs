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
    /// register/unregister itself from updates, avoiding using Unity update callbacks for a better result at a lower cost.
    /// <br/> You can register to a variety of callbacks using the <see cref="UpdateRegistration"/> enum flag.
    /// <para/>
    /// Can manage the object initialization from the OnInit method when using the <see cref="UpdateRegistration.Init"/> flag,
    /// which is more efficient and controlled to prevent framedrops when multiple objects initialization occurs.
    /// <para/>
    /// Comes also with basic activation/deactivation callbacks that ensure being called
    /// when its state really changes.
    /// <br/> You can use the OnPaused callback to implement specific behaviours when the object gets paused/unpaused,
    /// which happens when its local time scale factor reach 0.
    /// </summary>
    public class EnhancedBehaviour : MonoBehaviour, IBaseUpdate, IInitUpdate {
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

        /// <summary>
        /// Indicates whether this object is initialized or not.
        /// </summary>
        bool IInitUpdate.IsInitialized { get; set; } = false;
        #endregion

        #region Global Members
        [SerializeField, Enhanced, ReadOnly] protected float chronos = 1f;

        /// <summary>
        /// This object local time scale factor.
        /// <para/>
        /// Affects the values of both <see cref="DeltaTime"/> and <see cref="SmoothDeltaTime"/> properties.
        /// </summary>
        public float Chronos {
            get {
                return chronos;
            } internal set {
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
        /// Uses an overridable property for optimization purpose.
        /// </summary>
        public virtual Transform Transform {
            get {
                return transform;
            }
        }

        /// <summary>
        /// The unique identifier of this object.
        /// </summary>
        public int ID {
            get { return GetInstanceID(); }
        }
        #endregion

        #region Enhanced Behaviour
        /// <summary>
        /// Base unity message when this behaviour is enabled.
        /// <br/> Should not be overridden, always prefer using <see cref="OnBehaviourEnabled"/> instead.
        /// </summary>
        protected virtual void OnEnable() {
            OnBehaviourEnabled();
        }

        /// <summary>
        /// Base unity message when this behaviour is disabled.
        /// <br/> Should not be overridden, always prefer using <see cref="OnBehaviourDisabled"/> instead.
        /// </summary>
        protected virtual void OnDisable() {
            if (!GameManager.IsQuittingApplication) {
                OnBehaviourDisabled();
            }
        }

        void IInitUpdate.Init() {
            OnInit();
        }

        // -----------------------

        /// <summary>
        /// Called when this behaviour is being enabled.
        /// </summary>
        protected virtual void OnBehaviourEnabled() {
            if (UpdateRegistration != 0) {
                UpdateManager.Instance.Register(this, UpdateRegistration);
            }
        }

        /// <summary>
        /// Called on object initialization.
        /// <br/> Requires to set the flag <see cref="UpdateRegistration.Init"/> on this object <see cref="UpdateRegistration"/> property.
        /// <para/>
        /// Initialization is only called once after
        /// <br/> <see cref="OnBehaviourEnabled"/>, but before any local update.
        /// </summary>
        protected virtual void OnInit() { }

        /// <summary>
        /// Called when this object is being paused or unpaused.
        /// <para/>
        /// Pause happens when this object local <see cref="chronos"/> value is set to 0.
        /// </summary>
        protected virtual void OnPaused(bool _isPaused) { }

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
        /// <br/> This is an alternative to direct method calls from animation events,
        /// allowing more control and reference management.
        /// <para/>
        /// Note that this method should only be called from an animation event.
        /// </summary>
        /// <param name="_event">Event to call.</param>
        public void AnimationEvent(EnhancedAnimationEvent _event) {
            _event.Invoke(this);
        }
        #endregion

        #region Comparison
        /// <summary>
        /// Compare two objects using their instance id.
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
