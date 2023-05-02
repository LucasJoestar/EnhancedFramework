// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="EnhancedBehaviour"/> used to send the global enter trigger event
    /// to a <see cref="PlayMakerFSM"/> instance, when something enters this trigger.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "PlayMaker/Trigger Enter Event Sender")]
    public class PlayMakerTriggerEnterEventSender : EnhancedTrigger {
        #region Global Members
        [Section("On Enter Trigger - Event Sender")]

        [SerializeField, Enhanced, Required] private PlayMakerFSM fsm = null;
        #endregion

        #region Enhanced Behaviour
        private const string EventName = "ENHANCED TRIGGER ENTER";

        // -----------------------

        protected override void OnEnterTrigger(ITriggerActor _actor, EnhancedBehaviour _behaviour) {
            base.OnEnterTrigger(_actor, _behaviour);
            fsm.SendEvent(EventName);
        }
        #endregion
    }
}
