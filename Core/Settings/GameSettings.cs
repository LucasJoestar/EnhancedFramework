// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using UnityEngine;

namespace EnhancedFramework.Core.Settings {
    /// <summary>
    /// Global game settings, referencing each and every <see cref="BaseSettings{T}"/> shared across the game.
    /// </summary>
    [CreateAssetMenu(fileName = "GGS_GameSettings", menuName = MenuPath + "GameSettings", order = MenuOrder - 25)]
    public class GameSettings : BaseSettings<GameSettings> {
        #region Global Members
        [Section("Game Settings")]

        [SerializeField] protected BaseSettings[] settings = new BaseSettings[] { };

        [Space(10f)]

        [SerializeField, Enhanced, Required] protected TagDatabase tagDatabase = null;
        [SerializeField, Enhanced, Required] protected BuildSceneDatabase buildSceneDatabase = null;
        #endregion

        #region Initialization
        internal override void Init() {
            base.Init();

            // Settings and databases assignement.
            foreach (BaseSettings _settings in settings) {
                _settings.Init();
            }

            MultiTags.Database = tagDatabase;
            BuildSceneDatabase.Database = buildSceneDatabase;
        }
        #endregion
    }
}
