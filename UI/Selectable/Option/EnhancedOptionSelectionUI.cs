// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

#if LOCALIZATION_ENABLED
#define LOCALIZATION
#endif

using EnhancedEditor;
using EnhancedFramework.Core;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#if LOCALIZATION
using UnityEngine.Localization;
#endif

namespace EnhancedFramework.UI {
    /// <summary>
    /// Determines which selection movement should be interpreted.
    /// </summary>
    public enum SelectionMovement {
        None = 0,
        Horizontal  = 1,
        Vertical    = 2,
    }

    /// <summary>
    /// Enhanced <see cref="EnhancedOptionUI"/> for an option with multiple available choices.
    /// </summary>
    [ScriptGizmos(false, true)]
    #pragma warning disable
    public class EnhancedOptionSelectionUI : EnhancedOptionUI {
        #region Parameters
        /// <summary>
        /// <see cref="EnhancedOptionSelectionUI"/>-related UI parameters.
        /// </summary>
        [Flags]
        public enum Parameters {
            None = 0,
            Text                    = 1 << 1,
            SingleActiveObject      = 1 << 2,
            MutlipleActiveObjects   = 1 << 3,

            #if LOCALIZATION
            Localization            = 1 << 9,
            #endif
        }
        #endregion

        #region Global Members
        [Space(10f)]

        [Tooltip("If true, allows looping when reaching selection limit")]
        [SerializeField] private bool loopChoice = true;

        [Tooltip("Determines how selection navigation is interpreted")]
        [SerializeField] private SelectionMovement direction = SelectionMovement.Horizontal;

        [Tooltip("Parameters of this option")]
        [SerializeField] private Parameters parameters = Parameters.None;

        [Space(10f)]

        [Tooltip("Total count of available options")]
        [SerializeField, Enhanced, DrawMember("OptionCount"), ReadOnly] private int optionCount = 0;

        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("Audio played when selecting a new option")]
        [SerializeField] private AudioAsset onSelectAudio = null;

        [Tooltip("This option text")]
        [SerializeField, Enhanced, ShowIf("UseText")] private TextMeshProUGUI text = null;

        [Tooltip("Determines if GameObjects are activated or deactivated when their index is inferior or equal to the selected value")]
        [SerializeField, Enhanced, ShowIf("UseGameObjects")] private bool activateGameObject = true;

        [Space(10f)]

        [Tooltip("This option string values. Let empty to display the default option string value")]
        [SerializeField] private string[] stringContent = new string[0];

        #if LOCALIZATION
        [Tooltip("This option localized values")]
        [SerializeField] private LocalizedString[] localizedContent = new LocalizedString[0];
        #endif

        [Tooltip("This option associated feedback game objects")]
        [SerializeField] private GameObject[] gameObjects = new GameObject[0];

        // -----------------------

        /// <summary>
        /// Index of this option selected value.
        /// </summary>
        public int SelectedOptionIndex {
            get { return Option.SelectedValueIndex; }
        }

        /// <summary>
        /// Total count of available options.
        /// </summary>
        public int OptionCount {
            get {
                if (option == null) {
                    return 0;
                }

                return option.defaultOption.AvailableOptionCount;
            }
        }

        /// <summary>
        /// Indicates if this option uses a text.
        /// </summary>
        public bool UseText {
            get { return parameters.HasFlag(Parameters.Text); }
        }

        /// <summary>
        /// Indicates if this option uses multiple <see cref="GameObject"/>.
        /// </summary>
        public bool UseGameObjects {
            get { return parameters.HasFlag(Parameters.SingleActiveObject) || parameters.HasFlag(Parameters.MutlipleActiveObjects); }
        }

        /// <summary>
        /// Indicates if this option uses string values.
        /// </summary>
        public bool UseString {
            get {
                #if LOCALIZATION
                return parameters.HasFlag(Parameters.Text) && !parameters.HasFlag(Parameters.Localization);
                #else
                return parameters.HasFlag(Parameters.Text);
                #endif
            }
        }

        #if LOCALIZATION
        /// <summary>
        /// Indicates if this option uses localization.
        /// </summary>
        public bool UseLocalization {
            get { return parameters.HasFlag(Parameters.Text) && parameters.HasFlag(Parameters.Localization); }
        }
        #endif
        #endregion

        #region Selectable
        public override void OnMove(AxisEventData _eventData) {

            switch (direction) {

                // Horizontal.
                case SelectionMovement.Horizontal:

                    switch (_eventData.moveDir) {

                        case MoveDirection.Left:
                            SelectPreviousOption();
                            return;

                        case MoveDirection.Right:
                            SelectNextOption();
                            return;

                        case MoveDirection.Up:
                        case MoveDirection.Down:
                        case MoveDirection.None:
                        default:
                            break;
                    }

                    break;

                // Vertical.
                case SelectionMovement.Vertical:

                    switch (_eventData.moveDir) {

                        case MoveDirection.Up:
                            SelectNextOption();
                            return;

                        case MoveDirection.Down:
                            SelectPreviousOption();
                            return;

                        case MoveDirection.Left:
                        case MoveDirection.Right:
                        case MoveDirection.None:
                        default:
                            break;
                    }

                    break;

                case SelectionMovement.None:
                default:
                    break;
            }

            base.OnMove(_eventData);
        }
        #endregion

        #region Option
        /// <summary>
        /// Selects the next option from the list.
        /// </summary>
        public virtual void SelectNextOption() {
            SelectOption(SelectedOptionIndex + 1);
        }

        /// <summary>
        /// Selects the previous option from the list.
        /// </summary>
        public virtual void SelectPreviousOption() {
            SelectOption(SelectedOptionIndex - 1);
        }

        /// <summary>
        /// Selects a specific option from the list.
        /// </summary>
        /// <param name="_index">Index of the option to select.</param>
        public virtual void SelectOption(int _index) {

            int _optionCount = Option.AvailableOptionCount;
            if (_optionCount == 0) {
                return;
            }

            // Clamp option value.
            if (loopChoice) {

                while (_index >= _optionCount) {
                    _index -= _optionCount;
                }

                while (_index < 0) {
                    _index += _optionCount;
                }

            } else {
                _index = Mathf.Clamp(_index, 0, _optionCount - 1);
            }

            if (SelectedOptionIndex == _index) {
                return;
            }

            Option.SelectedValueIndex = _index;
            Apply();

            // Audio.
            if (onSelectAudio.IsValid()) {
                onSelectAudio.PlayAudio();
            }
        }
        #endregion

        #region Display
        public override void RefreshOption() {

            RefreshText();
            RefreshGameObjects();
        }

        // -------------------------------------------
        // Interface
        // -------------------------------------------

        private void RefreshText() {

            if (!UseText) {
                return;
            }

            #if LOCALIZATION
            // Localization.
            if (UseLocalization) {

                text.text = localizedContent[SelectedOptionIndex].GetLocalizedString();
                return;
            }
            #endif

            // Simple string.
            string _value;

            if (stringContent.Length == 0) {
                _value = Option.SelectedValueString;
            } else {
                _value = stringContent[SelectedOptionIndex];
            }

            text.text = _value;
        }

        private void RefreshGameObjects() {

            if (!UseGameObjects) {
                return;
            }

            // Activate game objects.
            for (int i = 0; i < gameObjects.Length; i++) {

                bool _active = parameters.HasFlag(Parameters.SingleActiveObject)
                             ? (i == SelectedOptionIndex)
                             : (parameters.HasFlag(Parameters.MutlipleActiveObjects) ? (i <= SelectedOptionIndex) : false);

                gameObjects[i].SetActive(_active == activateGameObject);
            }
        }
        #endregion
    }
}
