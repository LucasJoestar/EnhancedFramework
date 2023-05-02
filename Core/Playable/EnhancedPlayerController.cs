// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="EventTrigger"/> used to control an <see cref="EnhancedPlayer"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Player/Enhanced Player Controller")]
    public class EnhancedPlayerController : EventTrigger {
        #region Global Members
        [Section("Enhanced Player Controller"), PropertyOrder(0)]

        [Tooltip("Player for this trigger to interact with")]
        [SerializeField, Enhanced, Required] private EnhancedPlayer player = null;

        [Space(10f), PropertyOrder(1)]

        [Tooltip("If true, plays the associated player on trigger interaction and stops it when the interaction stops\n\n" +
                 "Otherwise, stops it on trigger interaction and plays it when the interaction stops")]
        [SerializeField] private bool playOnInteraction = true;
        #endregion

        #region Enhanced Behaviour
        #if UNITY_EDITOR
        protected override void OnValidate() {
            base.OnValidate();

            // Reference.
            if (!player && TryGetComponent(out EnhancedPlayer _player)) {
                player = _player;
            }
        }
        #endif
        #endregion

        #region Trigger
        protected override void PlayTriggerEvent() {
            PlayObject(playOnInteraction);
        }

        protected override void StopTriggerEvent() {
            PlayObject(!playOnInteraction);
        }

        // -----------------------

        private void PlayObject(bool _play) {

            if (_play) {
                player.Play();
            } else {
                player.Stop();
            }
        }
        #endregion
    }
}
