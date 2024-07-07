// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Object = UnityEngine.Object;

namespace EnhancedFramework.Timeline {
    /// <summary>
    /// <see cref="PlayableDirector"/>-related binding data storage component.
    /// </summary>
    [ScriptGizmos(false, true)]
    public sealed class EnhancedPlayableBindingData : EnhancedBehaviour {
        #region Global Members
        [Section("Playable Binding Data")]

        [Tooltip("All serialized bindings for the associated Playable")]
        [SerializeField, Enhanced, ReadOnly] private PairCollection<Object, Object> bindings = new PairCollection<Object, Object>();
        #endregion

        #region Utility
        /// <summary>
        /// Serializes the binding object to be associated with a specific playable clip.
        /// </summary>
        /// <param name="_clip">Clip to serialize a binding object for.</param>
        /// <param name="_binding">Binding object of the associated clip.</param>
        public void RegisterData(Object _clip, Object _binding) {
            bindings.Set(_clip, _binding);

            #if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            #endif
        }

        /// <summary>
        /// Get the binding object associated with a specific clip.
        /// </summary>
        /// <param name="_clip">Clip to get the associated binding.</param>
        /// <param name="_binding">Binding of this clip.</param>
        /// <returns>True if a binding could be successfully retrieved for this clip, false otherwise.</returns>
        public bool GetBinding(Object _clip, out Object _binding) {
            return bindings.TryGetValue(_clip, out _binding);
        }

        /// <summary>
        /// Clears this object data.
        /// </summary>
        [Button(ActivationMode.Editor, SuperColor.HarvestGold, IsDrawnOnTop = false)]
        public void Clear() {
            bindings.Clear();
        }
        #endregion
    }
}
