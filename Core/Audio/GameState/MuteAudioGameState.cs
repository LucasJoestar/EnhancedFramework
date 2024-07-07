// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;

namespace EnhancedFramework.Core.GameStates {
    /// <summary>
    /// <see cref="GameState"/> applied to mute an associated <see cref="MuteAudioGroup"/> while on stack.
    /// </summary>
    [Serializable, DisplayName(FrameworkUtility.MenuPath + "Audio/Mute Audio")]
    public sealed class MuteAudioGameState : GameState, IBoundGameState<MuteAudioGroup> {
        #region Global Members
        /// <summary>
        /// Doesn't require to be active, but just to exist.
        /// </summary>
        public const int PriorityConst = -10;

        public override int Priority {
            get { return PriorityConst; }
        }

        // Prevent from discarding this state.
        public override bool IsPersistent {
            get { return true; }
        }

        public override bool MultipleInstance {
            get { return true; }
        }

        // -----------------------

        /// <summary>
        /// The <see cref="MuteAudioGroup"/> associated with this state.
        /// </summary>
        public MuteAudioGroup BoundElement = null;

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="MuteAudioGameState"/>
        public MuteAudioGameState() : base(true) { }
        #endregion

        #region Bound
        public void Bound(MuteAudioGroup _object) {
            BoundElement = _object;
        }
        #endregion

        #region Behaviour
        protected override void OnInit() {
            base.OnInit();

            BoundElement.OnInit(this);
        }

        protected override void OnTerminate() {
            base.OnTerminate();

            BoundElement.OnTerminate(this);
        }
        #endregion
    }
}
