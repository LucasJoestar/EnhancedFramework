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
    public class PlayMakerTriggerExitEventSender : LevelTrigger {
        #region Global Members
        [Section("On Exit Trigger - Event Sender")]

        [SerializeField, Enhanced, Required] private PlayMakerFSM fsm = null;
        #endregion

        #region Enhanced Behaviour
        private const string EventName = "ENHANCED TRIGGER EXIT";

        // -----------------------

        protected override void OnExitTrigger(Component _component) {
            base.OnExitTrigger(_component);

            fsm.SendEvent(EventName);
        }
        #endregion
    }
}
