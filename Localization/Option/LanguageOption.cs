// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.Option;
using System;
using UnityEngine;
using UnityEngine.Localization;

using DisplayName = EnhancedEditor.DisplayNameAttribute;

namespace EnhancedFramework.Localization {

    /// <summary>
    /// <see cref="BaseGameOption"/> used to change the current language of the game.
    /// </summary>
    [Serializable, DisplayName("General/Language")]
    public class LanguageOption : BaseGameOption {
        #region Global Members
        [Space(10f)]

        [Tooltip("Current language of the game")]
        [SerializeField, Enhanced, Required, DisplayName("Default")] private Locale language = null;

        [Tooltip("All available language of the game")]
        [SerializeField] private Locale[] allLanguage = new Locale[0];

        // -----------------------

        /// <summary>
        /// Game rendering resolution
        /// </summary>
        public Locale Language {
            get { return language; }
            set {
                if (value == null) {
                    return;
                }

                language = value;
                Apply();
            }
        }

        /// <summary>
        /// Total count of available language in the game.
        /// </summary>
        public int LanguageCount {
            get { return allLanguage.Length; }
        }

        /// <summary>
        /// The index of the active selected language of the game.
        /// </summary>
        public int SelectedLanguageIndex {
            get { return Array.IndexOf(allLanguage, language); }
        }
        #endregion

        #region Behaviour
        public override void Apply() {
            LocalizationManager.Instance.SelectLocale(language);
        }

        public override void Refresh() {
            language =  LocalizationManager.Instance.SelectedLocale;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get a game language at a specific index.
        /// <para/>
        /// Use <see cref="LanguageCount"/> to get the total count of available language in the game.
        /// </summary>
        /// <param name="_index">Index of the language to get.</param>
        /// <returns>The language at the given index.</returns>
        public Locale GetLanguage(int _index) {
            return allLanguage[_index];
        }
        #endregion
    }
}
