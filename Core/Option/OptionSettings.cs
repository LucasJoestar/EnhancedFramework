// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.Settings;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

using Range = EnhancedEditor.RangeAttribute;

namespace EnhancedFramework.Core.Option {
    /// <summary>
    /// <see cref="OptionSettings"/>-related option base class.
    /// <br/> Inherit from this class to create your own options.
    /// </summary>
    [Serializable]
    public abstract class BaseGameOption {
        #region Global Members
        [SerializeField, Enhanced, ReadOnly] internal int guid = EnhancedUtility.GenerateGUID();
        [SerializeField] internal string name = "New Option";

        // -----------------------

        /// <summary>
        /// Identifier name of this option.
        /// </summary>
        public string Name {
            get { return name; }
        }

        /// <summary>
        /// Unique GUID of this option.
        /// </summary>
        public int GUID {
            get { return guid; }
        }

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <inheritdoc cref="BaseGameOption"/>
        public BaseGameOption() { }

        /// <param name="_name"><inheritdoc cref="Name" path="/summary"/></param>
        /// <param name="_guid"><inheritdoc cref="GUID" path="/summary"/></param>
        /// <inheritdoc cref="BaseGameOption"/>
        public BaseGameOption(string _name, int _guid) {
            name = _name;
            guid = _guid;
        }
        #endregion

        #region Option
        /// <summary>
        /// Applies this option value(s).
        /// </summary>
        public abstract void Apply();

        /// <summary>
        /// Refreshes this option value(s) from the game current state.
        /// </summary>
        public abstract void Refresh();
        #endregion
    }

    // -------------------------------------------
    // Options
    // -------------------------------------------

    /// <summary>
    /// Default <see cref="BaseGameOption"/> empty class.
    /// </summary>
    [Serializable, DisplayName("<Default>")]
    public class DefaultGameOption : BaseGameOption {
        #region Behaviour
        public override void Apply() { }

        public override void Refresh() { }
        #endregion
    }

    /// <summary>
    /// <see cref="BaseGameOption"/> used to save a specific <see cref="AudioMixer"/> volume.
    /// </summary>
    [Serializable, DisplayName("Audio/Volume")]
    public class AudioVolumeOption : BaseGameOption {
        #region Global Members
        [Space(10f)]

        [Tooltip("Audio Mixer to control a volume from")]
        [SerializeField, Enhanced, Required] private AudioMixer audioMixer = null;

        [Tooltip("Name of the volume to control")]
        [SerializeField] private string audioGroupName = "Master Volume";

        [Space(10f)]

        [Tooltip("Value of this volume")]
        [SerializeField, Enhanced, Range(0f, 1f)] private float volume = 1f;

        [Tooltip("Range of this volume")]
        [SerializeField, Enhanced, MinMax(-100f, 100f)] private Vector2 range = new Vector2(-80f, 20f);

        // -----------------------

        /// <summary>
        /// Value of this volume option (between 0 and 1).
        /// </summary>
        public float Volume {
            get { return volume; }
            set {
                volume = Mathf.Clamp(value, 0f, 1f);
                Apply();
            }
        }

        /// <summary>
        /// Range of this volume value.
        /// </summary>
        public Vector2 Range {
            get { return range; }
        }
        #endregion

        #region Behaviour
        public override void Apply() {

            // Set mixer float value.
            float _value = Mathf.Lerp(range.x, range.y, volume);
            audioMixer.SetFloat(audioGroupName, _value);
        }

        public override void Refresh() {

            // Read mixer float value.
            if (audioMixer.GetFloat(audioGroupName, out float _volume)) {

                float _difference = range.y - range.x;
                _volume = (_volume - range.x) / _difference;

                volume = _volume;
            }
        }
        #endregion
    }

    /// <summary>
    /// <see cref="BaseGameOption"/> used to set the game <see cref="FullScreenMode"/>.
    /// </summary>
    [Serializable, DisplayName("General/Full Screen Mode")]
    public class FullScreenModeOption : BaseGameOption {
        #region Global Members
        [Space(10f)]

        [Tooltip("Game full screen mode")]
        [SerializeField] private FullScreenMode mode = FullScreenMode.FullScreenWindow;

        // -----------------------

        /// <summary>
        /// Game full screen mode.
        /// </summary>
        public FullScreenMode Mode {
            get { return mode; }
            set {
                mode = value;
                Apply();
            }
        }
        #endregion

        #region Behaviour
        public override void Apply() {
            Screen.fullScreenMode = mode;
        }

        public override void Refresh() {
            mode = Screen.fullScreenMode;
        }
        #endregion
    }

    // -------------------------------------------
    // Settings
    // -------------------------------------------

    /// <summary>
    /// Game options wrapper.
    /// </summary>
    [Serializable]
    public class GameOptionsWrapper {
        #region Global Members
        [SerializeReference] public BaseGameOption[] Options = new BaseGameOption[] { };

        // -------------------------------------------
        // Constructor(s)
        // -------------------------------------------

        /// <summary>
        /// Prevents from creating any new instance of this class.
        /// </summary>
        internal protected GameOptionsWrapper() { }
        #endregion
    }

    /// <summary>
    /// Game option related settings, saved on disk.
    /// </summary>
    public class OptionSettings : BaseSettings<OptionSettings> {
        #region Global Members
        [Section("Option Settings")]

        [Tooltip("Path where to write these settings on disk")]
        [SerializeField] private OptionPath path = OptionPath.PersistentPath;

        [Tooltip("Name of the option file to write on disk (including extension)")]
        [SerializeField] private string fileName = "GameOption.ini";

        [Space(10f)]

        [Tooltip("All game Scriptable Object option wrappers")]
        [SerializeField] internal ScriptableGameOption[] scriptableOptions = new ScriptableGameOption[0];

        // -----------------------

        [NonSerialized] private GameOptionsWrapper option = new GameOptionsWrapper();

        // -----------------------

        /// <summary>
        /// Total amount of <see cref="BaseGameOption"/> in these settings.
        /// </summary>
        public int Count {
            get { return option.Options.Length; }
        }

        /// <summary>
        /// These option settings file path.
        /// </summary>
        public string FilePath {
            get { return Path.Combine(path.Get(true), fileName); }
        }
        #endregion

        #region Initialization
        protected internal override void Init() {
            base.Init();

            // Loading and initialization.
            Load();

            foreach (ScriptableGameOption _option in scriptableOptions) {
                _option.Initialize(this);
            }

            Save();
        }
        #endregion

        #region Option
        /// <summary>
        /// Get a specific <see cref="BaseGameOption"/> from its registered guid and name.
        /// </summary>
        /// <param name="_guid">The guid to get the associated option.</param>
        /// <param name="_name">The name of the option to get.</param>
        /// <param name="_creator">Called to create this option if not found.</param>
        /// <returns>The option associated with the given guid and name.</returns>
        public T GetOption<T>(int _guid, string _name, Func<T> _creator) where T : BaseGameOption {

            int _index = Array.FindIndex(option.Options, o => (o.GUID == _guid) && o.Name.Equals(_name, StringComparison.OrdinalIgnoreCase));

            if (_index != -1) {

                BaseGameOption _indexOption = option.Options[_index];

                if (_indexOption is T _temp) {
                    return _temp;
                }

                this.LogWarning($"Option with name \'{_indexOption.Name}\' and guid \'{_indexOption.GUID}\' does not match - Removing it");
                ArrayUtility.RemoveAt(ref option.Options, _index);
            }

            T _option = _creator();

            _option.name = _name;
            _option.guid = _guid;

            ArrayUtility.Add(ref option.Options, _option);
            return _option;
        }

        /// <summary>
        /// Get the <see cref="BaseGameOption"/> at the given index.
        /// <br/> Use <see cref="Count"/> to get the total amount of <see cref="BaseGameOption"/> in these settings.
        /// </summary>
        public BaseGameOption GetOption(int _index) {
            return option.Options[_index];
        }

        /// <summary>
        /// Applies all game option values.
        /// </summary>
        [Button(SuperColor.Green, IsDrawnOnTop = false)]
        public void Apply() {

            foreach (BaseGameOption _option in option.Options) {
                _option.Apply();
            }
        }

        /// <summary>
        /// Refreshes all game option value from the current game state.
        /// </summary>
        [Button(SuperColor.HarvestGold, IsDrawnOnTop = false)]
        public void Refresh() {

            foreach (BaseGameOption _option in option.Options) {
                _option.Refresh();
            }
        }
        #endregion

        #region File
        /// <summary>
        /// Loads these option data from disk.
        /// </summary>
        public void Load() {
            string _filePath = FilePath;

            // Read data from file.
            if (File.Exists(_filePath)) {
                string _json = File.ReadAllText(_filePath);

                try {

                    GameOptionsWrapper _wrapper = new GameOptionsWrapper();
                    JsonUtility.FromJsonOverwrite(_json, _wrapper);

                    option = _wrapper;

                } catch (Exception e) {
                    this.LogException(e);
                }
            }

            Apply();
        }

        /// <summary>
        /// Saves these option data on disk.
        /// </summary>
        public void Save() {

            string _filePath = FilePath;
            string _json = JsonUtility.ToJson(option);

            File.WriteAllText(_filePath, _json);
        }

        /// <summary>
        /// Clears and deletes these option file from disk.
        /// </summary>
        [Button(SuperColor.Crimson, IsDrawnOnTop = false)]
        public void Clear() {

            string _filePath = FilePath;

            if (File.Exists(_filePath)) {
                File.Delete(_filePath);
            }
        }

        /// <summary>
        /// Opens the game option directory.
        /// </summary>
        [Button(SuperColor.Green, IsDrawnOnTop = false)]
        public void Open() {

            string _filePath = Path.GetDirectoryName(FilePath);

            if (Directory.Exists(_filePath)) {
                Application.OpenURL(_filePath);
            }
        }
        #endregion
    }
}
