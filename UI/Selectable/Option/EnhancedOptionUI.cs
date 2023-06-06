// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core.Option;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace EnhancedFramework.UI {
    /// <summary>
    /// Enhanced <see cref="Selectable"/> behaviour, attached to a specific <see cref="ScriptableGameOption"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    public abstract class EnhancedOptionUI : EnhancedSelectable {
        #region Global Members
        [Space(10f), HorizontalLine(SuperColor.Grey, 1f), Space(10f)]

        [Tooltip("The option associated with this UI element")]
        [SerializeField, Enhanced, Required] protected ScriptableGameOption option = null;

        [Tooltip("Called whenever this option value is changed from the game interface")]
        [SerializeField] private UnityEvent onApply = new UnityEvent();

        [Space(5f)]

        [Tooltip("If true, automatically saves this option values whenever it changes")]
        [SerializeField] private bool saveOnChange = false;

        /// <summary>
        /// The option associated with this UI element.
        /// </summary>
        public BaseGameOption Option {
            get { return option.Option; }
        }
        #endregion

        #region Option
        /// <summary>
        /// Refreshes this option value in the game interface.
        /// </summary>
        public abstract void RefreshOption();
        #endregion

        #region Utility
        /// <summary>
        /// Applies this option values.
        /// </summary>
        public virtual void Apply() {
            Option.Apply();
            onApply.Invoke();

            if (saveOnChange) {
                Save();
            }

            RefreshOption();
        }

        /// <summary>
        /// Saves this option values.
        /// </summary>
        public virtual void Save() {
            OptionSettings.I.Save();
        }
        #endregion
    }
}
