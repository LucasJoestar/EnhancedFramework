// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("EnhancedFramework.Core")]
namespace EnhancedFramework.Core.Settings {
    /// <summary>
    /// Base class for all game settings.
    /// <para/> Should not be used directly, prefer inheriting from <see cref="BaseSettings{T}"/> instead.
    /// </summary>
    public abstract class BaseSettings : EnhancedScriptableObject {
        #region Content
        /// <summary>
        /// Initialize and set this settings instance as the general one to be used for the game.
        /// </summary>
        internal protected abstract void Init();
        #endregion
    }

    /// <summary>
    /// Base class to inherit all game settings from.
    /// </summary>
    /// <typeparam name="T">This class type.</typeparam>
    public abstract class BaseSettings<T> : BaseSettings where T : BaseSettings {
        #region Content
        public const string MenuPrefix  = "GS_";
        public const string MenuPath    = FrameworkUtility.MenuPath + "Game Settings/";
        public const int MenuOrder      = FrameworkUtility.MenuOrder;

        /// <summary>
        /// Global shared instance across the entiere game.
        /// </summary>
        public static T I { get; protected set; } = null;

        // -----------------------

        internal protected override void Init() {
            I = this as T;
        }
        #endregion
    }

    /// <summary>
    /// Base class to inherit all game database from.
    /// </summary>
    /// <typeparam name="T">This class type.</typeparam>
    public abstract class BaseDatabase<T> : BaseSettings where T : BaseSettings {
        #region Content
        public const string MenuPrefix  = "DT_";
        public const string MenuPath    = FrameworkUtility.MenuPath + "Game Database/";
        public const int MenuOrder      = FrameworkUtility.MenuOrder;

        /// <summary>
        /// Global shared instance across the entiere game.
        /// </summary>
        public static T Database { get; protected set; } = null;

        // -----------------------

        internal protected override void Init() {
            Database = this as T;
        }
        #endregion
    }
}
