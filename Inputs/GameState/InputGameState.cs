// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if INPUT_SYSTEM_PACKAGE
using EnhancedFramework.Core.GameStates;
using System;

namespace EnhancedFramework.Input {
    /// <summary>
    /// Base <see cref="GameState{T}"/> class used to automatically enable/disable
    /// a specific <see cref="InputMapAsset"/> whenever it became the current active state.
    /// </summary>
    /// <typeparam name="T"><inheritdoc cref="GameState{T}" path="/typeparam[@name='T']"/></typeparam>
    [Serializable]
    public abstract class InputGameState<T> : GameState<T> where T : GameStateOverride {
        #region Global Members
        /// <summary>
        /// The <see cref="InputMapAsset"/> to manage activation in this state.
        /// </summary>
        public readonly InputMapAsset Map = null;

        // -----------------------

        /// <summary>
        /// Creates a new instance of this state, with a specific <see cref="InputMapAsset"/> associated.
        /// </summary>
        /// <param name="_map"><inheritdoc cref="Map" path="/summary"/></param>
        public InputGameState(InputMapAsset _map) : base(true) {
            Map = _map;
        }
        #endregion

        #region Behaviour
        protected override void OnEnabled() {
            base.OnEnabled();

            Map.Enable();
        }

        protected override void OnDisabled() {
            base.OnDisabled();

            Map.Disable();
        }
        #endregion
    }
}
#endif
