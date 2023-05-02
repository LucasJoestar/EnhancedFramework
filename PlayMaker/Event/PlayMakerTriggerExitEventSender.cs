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
    /// <see cref="EnhancedBehaviour"/> used to send the global exit trigger event
    /// to a <see cref="PlayMakerFSM"/> instance, when something enters this trigger.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "PlayMaker/Trigger Exit Event Sender")]
    public class PlayMakerTriggerExitEventSender : EnhancedTrigger {
        #region Global Members
        [Section("On Exit Trigger - Event Sender")]

        [SerializeField, Enhanced, Required] private PlayMakerFSM fsm = null;
        #endregion

        #region Enhanced Behaviour
        private const string EventName = "ENHANCED TRIGGER EXIT";

        // -----------------------

        protected override void OnExitTrigger(ITriggerActor _actor, EnhancedBehaviour _behaviour) {
            base.OnExitTrigger(_actor, _behaviour);
            fsm.SendEvent(EventName);
        }
        #endregion
    }
}
