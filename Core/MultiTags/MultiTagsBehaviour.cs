// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="Component"/> wrapper for a <see cref="TagGroup"/>.
    /// </summary>
    [ScriptGizmos(false, true)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Tag/Multi Tags")]
    public class MultiTagsBehaviour : EnhancedBehaviour, IEnumerable<Tag> {
        #region Global Members
        [Section("Mutli-Tags")]

        public TagGroup Tags = new TagGroup();

        // -----------------------

        /// <summary>
        /// The total amount of tags in this object.
        /// </summary>
        public int Count {
            get { return Tags.Count; }
        }
        #endregion

        #region Operator
        public static implicit operator TagGroup(MultiTagsBehaviour _behaviour) {
            return _behaviour.Tags;
        }

        public Tag this[int _index] {
            get { return Tags[_index]; }
        }
        #endregion

        #region IEnumerable
        public IEnumerator<Tag> GetEnumerator() {
            for (int i = 0; i < Count; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion
    }
}
