// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if LOCALIZATION_ENABLED
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace EnhancedFramework.Core {
	/// <summary>
	/// Contains multiple extension methods related to the <see cref="LocalizedString"/> class.
	/// </summary>
	public static class LocalizedStringExtensions {
		#region Content
		/// <summary>
		/// Get the localized text value of this <see cref="LocalizedString"/>.
		/// </summary>
		/// <param name="_string">The <see cref="LocalizedString"/> to get the associated string value.</param>
		/// <returns>The localized text of the string.</returns>
		public static string GetLocalizedValue(this LocalizedString _string) {
			if ((_string == null) || string.IsNullOrEmpty(_string.TableReference)) {
				return string.Empty;
			}

			return _string.GetLocalizedString();
		}

		/// <summary>
		/// Set the localized text value of this <see cref="LocalizedString"/>.
		/// </summary>
		/// <param name="_string">The <see cref="LocalizedString"/> to set the associated string value.</param>
		/// <param name="_text">The localized text of the string for the active locale.</param>
		/// <returns>True if the string value was successfully set, false otherwise.</returns>
		public static bool SetLocalizedValue(this LocalizedString _string, string _text) {
			if ((_string == null) || string.IsNullOrEmpty(_string.TableReference)) {
				return false;
			}

			StringTableEntry _entry = LocalizationSettings.StringDatabase.GetTableEntry(_string.TableReference, _string.TableEntryReference).Entry;
			_entry.Value = _text;

			return true;
		}
		#endregion
	}
}
#endif
