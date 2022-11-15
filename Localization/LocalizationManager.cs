// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if LOCALIZATION_ENABLED
using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EnhancedFramework.Localization {
    public interface ILocalizable {
        void GetLocalizationTables(Set<TableReference> _stringTables, Set<TableReference> _assetTables);
    }

    [Serializable]
    public class LocalizationResourceLoader : ResourceLoader<LocalizationResourceLoader> {
        #region Global Members
        public override bool IsProcessing {
            get { return currentOperations.Count != 0; }
        }

        // -----------------------

        private readonly Set<TableReference> stringTables   = new Set<TableReference>();
        private readonly Set<TableReference> assetTables    = new Set<TableReference>();

        private readonly List<AsyncOperationHandle> currentOperations = new List<AsyncOperationHandle>();
        #endregion

        #region Behaviour
        public override void Load() {
            // Load all localization tables, previously setup in the Fill method.
            if (stringTables.Count != 0) {
                ManageOperation(LocalizationManager.Instance.LoadStringTables(stringTables));
            }

            if (assetTables.Count != 0) {
                ManageOperation(LocalizationManager.Instance.LoadAssetTables(assetTables));
            }

            // ----- Local Methods ----- \\

            void ManageOperation(AsyncOperationHandle _handle) {
                if (_handle.IsDone) {
                    return;
                }

                currentOperations.Add(_handle);
                _handle.Completed += OnOperationComplete;
            }

            void OnOperationComplete(AsyncOperationHandle _handle) {
                currentOperations.Remove(_handle);
            }
        }

        public override void Unload() {
            // Release all loaded tables.
            if (stringTables.Count != 0) {
                LocalizationManager.Instance.ReleaseStringTables(stringTables);
            }

            if (assetTables.Count != 0) {
                LocalizationManager.Instance.ReleaseAssetTables(assetTables);
            }
        }

        // -----------------------

        /// <summary>
        /// Fills this loader localization tables with a specific <see cref="ILocalizable"/> instance.
        /// </summary>
        /// <param name="_localizable">The <see cref="ILocalizable"/> to preload tables.</param>
        public void FillTables(ILocalizable _localizable) {
            _localizable.GetLocalizationTables(stringTables, assetTables);
        }
        #endregion
    }

    /// <summary>
    /// Interface used to receives callbacks on multiple localization-related events.
    /// <para/>
    /// Must be registered on initialization and unregistered on deactivation using
    /// <see cref="LocalizationManager.Register(ILocalizer)"/> and <see cref="LocalizationManager.Unregister(ILocalizer)"/>.
    /// </summary>
    public interface ILocalizer {
        #region Content
        /// <summary>
        /// Called when the game active locale is changed.
        /// </summary>
        /// <param name="_locale">New active locale (use this to update localized strings and assets).</param>
        void OnLocaleChanged(Locale _locale);
        #endregion
    }

    /// <summary>
    /// Localization singleton class, managing all localization-related operations (like tables loading and selected locale).
    /// </summary>
    public class LocalizationManager : EnhancedSingleton<LocalizationManager>, ILoadingProcessor {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        // True while any localization table async operation is running.
        public bool IsProcessing {
            get { return operations.Count != 0; }
        }

        // -----------------------

        private readonly List<ILocalizer> localizables = new List<ILocalizer>();
        private readonly List<AsyncOperationHandle> operations = new List<AsyncOperationHandle>();
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            EnhancedSceneManager.Instance.RegisterProcessor(this);

            // Initialize the localization system.
            // Tables can't be preloaded while not initialized.
            AsyncOperationHandle<LocalizationSettings> _handle = LocalizationSettings.InitializationOperation;

            if (!_handle.IsDone) {
                operations.Add(_handle);
                _handle.Completed += OnComplete;
            }

            // ----- Local Method ----- \\

            void OnComplete(AsyncOperationHandle<LocalizationSettings> _operation) {
                _operation.Completed -= OnComplete;
                operations.Remove(_operation);

                switch (_operation.Status) {
                    case AsyncOperationStatus.Succeeded:
                        this.Log("Localization initialization successfully completed");
                        break;

                    case AsyncOperationStatus.Failed:
                        this.LogError("Localization initialization failed");
                        break;

                    case AsyncOperationStatus.None:
                    default:
                        this.LogWarning("Localization initialization status unknown");
                        break;
                }
            }
        }

        private void OnDestroy() {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
        }
        #endregion

        #region Localizable
        /// <summary>
        /// Registers a new localizer instance.
        /// </summary>
        /// <param name="_localizable">The localizer to register.</param>
        public void Register(ILocalizer _localizable) {
            localizables.Add(_localizable);
        }

        /// <summary>
        /// Unregisters a specific localizer.
        /// </summary>
        /// <param name="_localizable">The localizer to unregister.</param>
        public void Unregister(ILocalizer _localizable) {
            localizables.Remove(_localizable);
        }
        #endregion

        #region Locale
        /// <summary>
        /// Selects and activates a specific locale.
        /// </summary>
        /// <param name="_locale">The new locale to select.</param>
        public void SelectLocale(Locale _locale) {
            LocalizationSettings.SelectedLocale = _locale;
        }

        private void OnLocaleChanged(Locale _locale) {
            this.LogWarning("Changed Locale => " + _locale.name);

            // Update localizables.
            foreach (ILocalizer _localizable in localizables) {
                _localizable.OnLocaleChanged(_locale);
            }
        }
        #endregion

        #region Loading
        /// <summary>
        /// Loads a specific string table.
        /// </summary>
        /// <param name="_table">The string table to load.</param>
        public AsyncOperationHandle LoadStringTable(TableReference _table) {
            return ManageAsyncOperationHandle(LocalizationSettings.StringDatabase.PreloadTables(_table));
        }

        /// <summary>
        /// Loads a collection of string tables.
        /// </summary>
        /// <param name="_tables">All string tables to load.</param>
        public AsyncOperationHandle LoadStringTables(IList<TableReference> _tables) {
            return ManageAsyncOperationHandle(LocalizationSettings.StringDatabase.PreloadTables(_tables));
        }

        /// <summary>
        /// Loads a specific asset table.
        /// </summary>
        /// <param name="_table">The asset table to load.</param>
        public AsyncOperationHandle LoadAssetTable(TableReference _table) {
            return ManageAsyncOperationHandle(LocalizationSettings.AssetDatabase.PreloadTables(_table));
        }

        /// <summary>
        /// Loads a collection of asset tables.
        /// </summary>
        /// <param name="_tables">All asset tables to load.</param>
        public AsyncOperationHandle LoadAssetTables(IList<TableReference> _tables) {
            return ManageAsyncOperationHandle(LocalizationSettings.AssetDatabase.PreloadTables(_tables));
        }

        // -----------------------

        private AsyncOperationHandle ManageAsyncOperationHandle(AsyncOperationHandle _handle) {
            // Use a callback if the operation is not complete.
            if (_handle.IsDone) {
                OnAsyncOperationComplete(_handle);
                return _handle;
            }

            operations.Add(_handle);
            _handle.Completed += OnComplete;

            return _handle;

            // ----- Local Method ----- \\

            void OnComplete(AsyncOperationHandle _operation) {
                _operation.Completed -= OnComplete;

                operations.Remove(_handle);
                OnAsyncOperationComplete(_operation);
            }
        }

        private void OnAsyncOperationComplete(AsyncOperationHandle _handle) {
            switch (_handle.Status) {
                // Everything is fine.
                case AsyncOperationStatus.Succeeded:
                    this.Log("Localization Table loading operation successfully completed");
                    break;

                // Error.
                case AsyncOperationStatus.Failed:
                    this.LogError("Localization Table loading operation failed");
                    break;

                // Unknown.
                case AsyncOperationStatus.None:
                default:
                    this.LogWarning("Localization Table loading operation status unknown");
                    break;
            }
        }
        #endregion

        #region Release
        /// <summary>
        /// Releases a collection of string tables.
        /// </summary>
        /// <param name="_tables">All string tables to release.</param>
        public void ReleaseStringTables(IList<TableReference> _tables) {
            foreach (TableReference _table in _tables) {
                ReleaseStringTable(_table);
            }
        }

        /// <summary>
        /// Releases a collection of asset tables.
        /// </summary>
        /// <param name="_tables">All asset tables to release.</param>
        public void ReleaseAssetTables(IList<TableReference> _tables) {
            foreach (TableReference _table in _tables) {
                ReleaseAssetTable(_table);
            }
        }

        /// <summary>
        /// Releases a specific string table.
        /// </summary>
        /// <param name="_table">The string table to release.</param>
        public void ReleaseStringTable(TableReference _table) {
            LocalizationSettings.StringDatabase.ReleaseTable(_table);
        }

        /// <summary>
        /// Releases a specific asset table.
        /// </summary>
        /// <param name="_table">The asset table to release.</param>
        public void ReleaseAssetTable(TableReference _table) {
            LocalizationSettings.AssetDatabase.ReleaseTable(_table);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Utility method only used from the component inspector window.
        /// </summary>
        [Button(ActivationMode.Play, SuperColor.Green)]
        #pragma warning disable IDE0051
        private void GetOperationCount() {
            this.LogWarning($"Localization async operation count - {operations.Count}");
        }
        #endregion
    }
}
#endif
