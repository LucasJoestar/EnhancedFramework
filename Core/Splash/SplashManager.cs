// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.GameStates;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Game splash manager class.
    /// <para/>
    /// Should be loaded after and in a different Splash specific scene than Core.
    /// </summary>
    public class SplashManager : EnhancedSingleton<SplashManager>, ILoadingProcessor {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Play;

        #region Loading Processor
        public override bool IsLoadingProcessor => true;

        // True while playing any animation.
        public bool IsProcessing {
            get { return isPlaying; }
        }
        #endregion

        #region Global Members
        [Section("Splash Manager")]

        [SerializeField, Enhanced, Required] private SceneBundle nextScene = null;

        [Space(10f)]

        [SerializeField] private BlockArray<PolymorphValue<SplashAnimation>> animations = new BlockArray<PolymorphValue<SplashAnimation>>();

        // -----------------------

        private SceneBundle splashBundle = null;
        private bool isPlaying = false;
        #endregion

        #region Enhanced Behaviour
        protected override void OnBehaviourEnabled() {
            base.OnBehaviourEnabled();

            // Get this object scene bundle once fully loaded.
            EnhancedSceneManager.OnPostLoadBundle += OnBundleLoaded;
        }

        protected override void OnPlay() {
            base.OnPlay();

            StartCoroutine(PlaySplash());
        }

        protected override void OnBehaviourDisabled() {
            base.OnBehaviourDisabled();

            // Unregister from callback if not yet done.
            if (splashBundle.IsNull()) {
                EnhancedSceneManager.OnPostLoadBundle -= OnBundleLoaded;
            }
        }

        // -------------------------------------------
        // Loading Callback
        // -------------------------------------------

        private void OnBundleLoaded(SceneBundle _bundle) {
            // Get this splash scene.
            if (_bundle.ContainScene(gameObject.scene, out _)) {
                splashBundle = _bundle;

                EnhancedSceneManager.OnPostLoadBundle -= OnBundleLoaded;
            }
        }
        #endregion

        #region Behaviour
        private const float TimeBeforeSplash = .1f;
        private GameState gameState = null;

        // -----------------------

        private IEnumerator PlaySplash() {
            // Wait some time before starting the splash to let the time for
            // all objects in the scene to be properly initialized.
            //
            // (The Update system make sure to not perform initializations and play for more than a fixed amount of time)
            yield return new WaitForSecondsRealtime(TimeBeforeSplash);

            gameState = GameState.CreateState<SplashGameState>();
            isPlaying = true;

            EnhancedSceneManager.Instance.LoadSceneBundle(nextScene, LoadSceneMode.Additive);

            // Play each animation.
            for (int i = 0; i < animations.Count; i++) {
                SplashAnimation _animation = animations[i];
                yield return _animation.Play();
            }

            isPlaying = false;
            gameState.RemoveState();

            // Unload this splash scene and its content, as it will never be used again.
            this.Assert(splashBundle.IsValid());
            EnhancedSceneManager.Instance.UnloadSceneBundle(splashBundle, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        }
        #endregion
    }
}
