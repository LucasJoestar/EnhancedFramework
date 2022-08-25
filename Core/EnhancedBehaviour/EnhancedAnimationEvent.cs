// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base serializable class for all animation events.
    /// <br/> Should not be directly inherited by new classes,
    /// always prefer using <see cref="EnhancedAnimationEvent{T}"/> instead.
    /// </summary>
    public abstract class EnhancedAnimationEvent : ScriptableObject {
        public const int AnimationEventMenuOrder = 151;

        #region Event Behaviour
        /// <summary>
        /// Invokes this event.
        /// </summary>
        internal abstract void Invoke(EnhancedBehaviour _behaviour);
        #endregion
    }

    /// <summary>
	/// Base class to inherit all your own animation events from.
    /// <para/> Uses a <see cref="EnhancedBehaviour"/> target to apply this behaviour to.
	/// </summary>
    public abstract class EnhancedAnimationEvent<T> : EnhancedAnimationEvent where T : EnhancedBehaviour {
        #region Event Behaviour
        internal override sealed void Invoke(EnhancedBehaviour _behaviour) {
            // This method is called for each and every EnhancedBehaviour on the source object.
            // To prevent from performing this event behaviour multiple times, cast it into its target type first.
            if (_behaviour is T _generic) {
                OnInvoke(_generic);
            }
        }

        /// <summary>
        /// Override this to implement this event specific behaviour.
        /// </summary>
        /// <param name="_behaviour">Target <see cref="EnhancedBehaviour"/> of this event.</param>
        protected abstract void OnInvoke(T _behaviour);
        #endregion
    }
}
