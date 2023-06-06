// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base interface to derive any animation event class from.
    /// </summary>
    public interface IEnhancedAnimationEvent {
        #region Content
        /// <summary>
        /// Invokes this event from a specific <see cref="EnhancedBehaviour"/>.
        /// </summary>
        /// <param name="_behaviour">The <see cref="EnhancedBehaviour"/> invoking this event.</param>
        void Invoke(EnhancedBehaviour _behaviour);
        #endregion
    }

    /// <summary>
    /// <see cref="ScriptableObject"/> base serializable class for all animation events.
    /// <para/>
    /// Always inherit from <see cref="EnhancedAnimationEvent{T}"/> instead of this base class.
    /// </summary>
    public abstract class EnhancedAnimationEvent : ScriptableObject, IEnhancedAnimationEvent {
        public const int AnimationEventMenuOrder = 151;

        #region Constructor
        /// <summary>
        /// Prevents inheriting from this class in other assemblies.
        /// </summary>
        private protected EnhancedAnimationEvent() { }
        #endregion

        #region Event
        /// <inheritdoc cref="IEnhancedAnimationEvent.Invoke(EnhancedBehaviour)"/>
        public abstract void Invoke(EnhancedBehaviour _behaviour);
        #endregion
    }

    /// <summary>
	/// Base class to inherit all your own animation events from.
    /// <para/>
    /// Invoked from a source <see cref="EnhancedBehaviour"/> target, used to apply its behaviour to.
	/// </summary>
    public abstract class EnhancedAnimationEvent<T> : EnhancedAnimationEvent where T : EnhancedBehaviour {
        #region Event
        public override sealed void Invoke(EnhancedBehaviour _behaviour) {

            // This method is called on each and every EnhancedBehaviour component on the source behaviour instance.
            // To prevent from invoking this event multiple times, cast it into its target type for match.
            if (_behaviour is T _eventBehaviour) {
                OnInvoke(_eventBehaviour);
            }
        }

        /// <summary>
        /// Override this to implement a specific behaviour.
        /// </summary>
        /// <param name="_behaviour">Source target <see cref="EnhancedBehaviour"/> of this event.</param>
        protected abstract void OnInvoke(T _behaviour);
        #endregion
    }
}
