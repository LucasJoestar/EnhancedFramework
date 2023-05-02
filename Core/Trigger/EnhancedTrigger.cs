// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base interface for a trigger object.
    /// </summary>
    public interface ITrigger {
        #region Content
        /// <summary>
        /// Called whenever an actor enters this trigger.
        /// </summary>
        /// <param name="_actor"><see cref="ITriggerActor"/> entering this trigger.</param>
        void OnEnterTrigger(ITriggerActor _actor);

        /// <summary>
        /// Called whenever an actor exits this trigger.
        /// </summary>
        /// <param name="_actor"><see cref="ITriggerActor"/> exiting this trigger.</param>
        void OnExitTrigger(ITriggerActor _actor);
        #endregion
    }

    /// <summary>
    /// Base interface to inherit any actor interacting with a trigger from.
    /// </summary>
    public interface ITriggerActor {
        #region Content
        /// <summary>
        /// <see cref="EnhancedBehaviour"/> of this actor.
        /// </summary>
        EnhancedBehaviour Behaviour { get; }

        /// <summary>
        /// Forces this actor to exit a specific trigger.
        /// </summary>
        /// <param name="_trigger">Trigger to exit.</param>
        void ExitTrigger(ITrigger _trigger);
        #endregion
    }

    /// <summary>
    /// Base <see cref="EnhancedBehaviour"/> class to inherit your own triggers from.
    /// </summary>
    [SelectionBase]
    public abstract class EnhancedTrigger : EnhancedBehaviour, ITrigger { }
}
