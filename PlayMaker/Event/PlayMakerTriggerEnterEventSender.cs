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
    public class PlayMakerTriggerEnterEventSender : LevelTrigger {
        #region Global Members
        [Section("On Enter Trigger - Event Sender")]

        [SerializeField, Enhanced, Required] private PlayMakerFSM fsm = null;
        #endregion

        #region Enhanced Behaviour
        private const string EventName = "ENHANCED TRIGGER ENTER";

        // -----------------------

        protected override void OnEnterTrigger(Component _component) {
            base.OnEnterTrigger(_component);

            fsm.SendEvent(EventName);
        }
        #endregion
    }
}
