// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Settings {
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "GAM_GameSettings", menuName = MenuPath + "GameSettings", order = MainMenuOrder)]
    public class GameSettings : ScriptableObject {
        #region Global Members
        // Menu constants.
        private const int MainMenuOrder     = 150;

        public const string MenuPath    = "Enhanced Framework/Datas/Settings/";
        public const int MenuOrder      = MainMenuOrder + 10;

        [Section("Game Settings")]

        [SerializeField, Enhanced, Required] private CollisionSettings collisionSettings = null;
        [SerializeField, Enhanced, Required] private PhysicsSettings physicsSettings     = null;

        [SerializeField, Enhanced, Required] private TagDatabase tagDatabase = null;
        [SerializeField, Enhanced, Required] private BuildSceneDatabase buildSceneDatabase = null;
        #endregion

        #region Initialization
        public void Initialize() {
            // Settings static assignement.
            //CollisionSettings.I = CollisionSettings;
            //PhysicsSettings.I = PhysicsSettings;

            BuildSceneDatabase.Database = buildSceneDatabase;
            MultiTags.Database = tagDatabase;
        }
        #endregion
    }
}
