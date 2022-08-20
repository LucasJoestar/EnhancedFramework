// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Serializable mother class for all <see cref="EnhancedAnimationEvent{T}"/>.
    /// <br/> Should not be inherited by new classes.
    /// </summary>
    public abstract class EnhancedAnimationEvent : ScriptableObject {
        #region Content
        public const int AnimationEventMenuOrder = 151;

        /// <summary>
        /// Invokes this event.
        /// </summary>
        public abstract void Invoke(EnhancedBehaviour _behaviour);
        #endregion
    }

    /// <summary>
	/// Base class for all <see cref="ScriptableObject"/> to use for animation event
	/// with an <see cref="EnhancedBehaviour"/>.
	/// </summary>
    public abstract class EnhancedAnimationEvent<T> : EnhancedAnimationEvent where T : EnhancedBehaviour {
        #region Content
        public override sealed void Invoke(EnhancedBehaviour _behaviour) {
            if (_behaviour is T _generic) {
                OnInvoke(_generic);
            }
        }

        /// <summary>
        /// Called when invoking this event.
        /// <br/> Use this to implement your behaviour.
        /// </summary>
        /// <param name="_behaviour">Target behaviour of the event.</param>
        public abstract void OnInvoke(T _behaviour);
        #endregion
    }
}
