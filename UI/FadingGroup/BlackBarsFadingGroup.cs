// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedFramework.Core;
using EnhancedFramework.Core.GameStates;

namespace EnhancedFramework.UI {
    /// <summary>
    /// <see cref="EnhancedBehaviour"/> UI class used to manage the black bars displayed at the top and bottom of the screen. 
    /// </summary>
    public class BlackBarsFadingGroup : FadingObjectSingleton<BlackBarsFadingGroup>, IGameStateOverrideCallback {

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            GameStateManager.Instance.RegisterOverrideCallback(this);
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            GameStateManager.Instance.UnregisterOverrideCallback(this);
        }
        #endregion

        #region Game State Callback
        void IGameStateOverrideCallback.OnGameStateOverride(in GameStateOverride _state) {
            if (_state is DefaultGameStateOverride _override) {
                SetVisibility(_override.ShowBlackBars);
            }
        }
        #endregion
    }
}
