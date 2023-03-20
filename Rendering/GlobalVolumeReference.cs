// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace EnhancedFramework.Rendering {
    /// <summary>
    /// <see cref="Component"/> to attach to every <see cref="Volume"/> of the game,
    /// <br/> used to reference the currently active one with the highest priority.
    /// </summary>
    [RequireComponent(typeof(Volume))]
    public class GlobalVolumeReference : EnhancedBehaviour {
        #region Global Members
        /// <summary>
        /// The active <see cref="Volume"/> with the highest priority of the game.
        /// </summary>
        public static Volume GlobalVolume {
            get { return volumeBuffer.Value; }
        }

        private static readonly BufferV<Volume> volumeBuffer = new BufferV<Volume>();

        // -----------------------

        [Section("Global Volume Reference")]

        [SerializeField, Enhanced, ReadOnly] private int id = 0;
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // If a volume is attached to the object, register it.
            if (gameObject.TryGetComponent(out Volume _volume) && _volume.isGlobal) {
                id = EnhancedUtility.GenerateGUID();
                volumeBuffer.Push(id, new Pair<Volume, int>(_volume, (int)(_volume.priority * 100f)));
            }
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // If an id has been registered, pop its volume value.
            if (id != 0) {
                volumeBuffer.Pop(id);
            }
        }
        #endregion
    }
}
