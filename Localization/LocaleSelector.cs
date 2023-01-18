// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //


using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.Localization;

namespace EnhancedFramework.Localization {
    /// <summary>
    /// Utility component used to select a specific <see cref="Locale"/>.
    /// </summary>
    public class LocaleSelector : EnhancedBehaviour {
        #region Global Members
        [Tooltip("Locale to select")]
        [SerializeField, Enhanced, Required] private Locale locale = null;
        #endregion

        #region Behaviour
        /// <summary>
        /// Selects the configured <see cref="Locale"/>.
        /// </summary>
        public void SelectLocale() {
            LocalizationManager.Instance.SelectLocale(locale);
        }
        #endregion
    }
}
