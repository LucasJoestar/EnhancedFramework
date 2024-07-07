// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using System.Collections.Generic;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Enhanced collection preventing any duplicate element.
    /// </summary>
    /// <typeparam name="T">This list content type.</typeparam>
    [Serializable]
    public class Set<T> : EnhancedCollection<T> {
        #region Constructor
        /// <inheritdoc cref="EnhancedCollection{T}()"/>
        public Set() : base() { }

        /// <inheritdoc cref="EnhancedCollection{T}(IEnumerable{T})"/>
        public Set(IEnumerable<T> _collection) : base(_collection) { }

        /// <inheritdoc cref="EnhancedCollection{T}(int)"/>
        public Set(int _capacity) : base(_capacity) { }
        #endregion

        #region Collection
        public override void Add(T _element) {
            if (!Contains(_element)) {
                base.Add(_element);
            }
        }

        public override void AddRange(IEnumerable<T> _collection) {
            foreach (T _element in _collection) {
                Add(_element);
            }
        }

        public override void Insert(int _index, T _element) {
            if (!Contains(_element)) {
                base.Insert(_index, _element);
            }
        }
        #endregion
    }
}
