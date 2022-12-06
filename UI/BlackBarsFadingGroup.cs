// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if DOTWEEN_ENABLED
using EnhancedFramework.Core;
using EnhancedFramework.Core.GameStates;

namespace EnhancedFramework.UI {
    /// <summary>
    /// <see cref="EnhancedBehaviour"/> UI class used to manage the black bars displayed at the top and bottom of the screen. 
    /// </summary>
    public class BlackBarsFadingGroup : FadingGroupSingleton<BlackBarsFadingGroup, TweeningFadingGroup>, IGameStateOverrideCallback {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            GameStateManager.Instance.RegisterOverrideCallback(this);
        }

        private void OnDestroy() {
            if (!GameManager.IsQuittingApplication) {
                GameStateManager.Instance.UnregisterOverrideCallback(this);
            }
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
#endif
