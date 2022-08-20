// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using System.Collections;
using System.Collections.Generic;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Array wrapper which is automatically expanded by a fixed amount
    /// each time it is needed, without shrinking back when removing element.
    /// <para/>
    /// Can be cleared to remove empty spaces.
    /// </summary>
    /// <typeparam name="T">Array element type.</typeparam>
    [Serializable]
    public class Stamp<T> : IEnumerable<T> {
        #region Buffer
        public const int DefaultExpandSize = 3;

        // -----------------------

        private readonly int expandSize = DefaultExpandSize;

        /// <summary>
        /// Total count of the stamp.
        /// </summary>
        public int Count = 0;

        /// <summary>
        /// Array of the stamp. Its size should not be set externally.
        /// </summary>
        private T[] Array = new T[] { };
        #endregion

        #region Constructor
        /// <inheritdoc cref="Stamp{T}(int, int)"/>
        public Stamp(int _expandSize = DefaultExpandSize) {
            expandSize = _expandSize;
        }

        /// <summary>
        /// Creates a new stamp (one way expanding array).
        /// </summary>
        /// <param name="_size">Initial size of the buffer.</param>
        /// <param name="_expandSize">Amount by wich the array is expanded
        /// each time it needs more space.</param>
        public Stamp(int _size, int _expandSize) : this(_expandSize) {
            Array = new T[_size];
            Count = _size;
        }
        #endregion

        #region Operators
        public T this[int _index] {
            get => Array[_index];
            set => Array[_index] = value;
        }
        #endregion

        #region IEnumerable
        public IEnumerator<T> GetEnumerator() {
            for (int i = Count; i-- > 0;) {
                yield return Array[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Enumeration
        /// <summary>
        /// Get the first element from this stamp.</typeparam>
        /// <returns>First element from this stamp.</returns>
        public T First() {
            return Array[0];
        }

        /// <summary>
        /// A safe version of <see cref="First{T}(T[])"/>.
        /// <br/> Get the first element from this stamp or the default value if empty.
        /// </summary>
        /// <param name="_element">First element from this stamp or the default value if empty.</param>
        /// <returns>False if this stamp is empty, true otherwise.</returns>
        public bool SafeFirst(out T _element) {
            if (Count == 0) {
                _element = default;
                return false;
            }

            _element = Array.First();
            return true;
        }

        /// <summary>
        /// Get the last element from this stamp.
        /// </summary>
        /// <returns>Last element from this stamp.</returns>
        public T Last() {
            return Array[Count - 1];
        }

        /// <summary>
        /// A safe version of <see cref="Last{T}(T[])"/>.
        /// <br/> Get the last element from this stamp or the default value if empty.
        /// </summary>
        /// <param name="_element">Last element from this stamp or the default value if empty.</param>
        /// <returns>False if this stamp is empty, true otherwise.</returns>
        public bool SafeLast(out T _element) {
            if (Count == 0) {
                _element = default;
                return false;
            }

            _element = Array.Last();
            return true;
        }
        #endregion

        #region Management
        /// <summary>
        /// Adds a new element in the stamp.
        /// </summary>
        /// <param name="_element">New array element.</param>
        public void Add(T _element) {
            if (Array.Length == Count) {
                Expand();
            }

            Array[Count] = _element;
            Count++;
        }

        /// <summary>
        /// Tries to restore last removed element from the stamp.
        /// Automatically add it to the buffer is succesfully restored.
        /// </summary>
        /// <param name="_element">Restored element.</param>
        /// <returns>True if successfully restored element, false otherwise.</returns>
        public bool Restore(out T _element) {
            if ((Array.Length == Count) || ReferenceEquals(Array[Count], null)) {
                _element = default;
                return false;
            }

            _element = Array[Count];
            Count++;

            return true;
        }

        /// <summary>
        /// Removes an element from the stamp.
        /// </summary>
        /// <param name="_element">Element to remove.</param>
        public void Remove(T _element) {
            int _index = System.Array.IndexOf(Array, _element);
            RemoveAt(_index);
        }

        /// <summary>
        /// Removes the element at specified index from the stamp.
        /// </summary>
        /// <param name="_element">Index of the element to remove.</param>
        public void RemoveAt(int _index) {
            int _last = Count - 1;
            if (_index != _last) {
                T _removed = Array[_index];

                Array[_index] = Array[_last];
                Array[_last] = _removed;
            }

            Count--;
        }

        /// <summary>
        /// Removes the first element of the stamp.
        /// </summary>
        public void RemoveFirst() {
            RemoveAt(0);
        }

        /// <summary>
        /// Removes the last element of the stamp.
        /// </summary>
        public void RemoveLast() {
            RemoveAt(Count - 1);
        }

        // -----------------------

        /// <summary>
        /// Clear this stamp by setting its count to zero.
        /// </summary>
        public void Clear() {
            Count = 0;
        }

        /// <summary>
        /// Completely resets this stamp by removing all entries and setting its count to 0.
        /// </summary>
        public void Reset() {
            Array = new T[] { };
            Count = 0;
        }

        /// <summary>
        /// Resize this collection content by removing empty entries.
        /// Does not delete existing elements.
        /// </summary>
        public void Resize() {
            System.Array.Resize(ref Array, Count);
        }

        // -----------------------

        private void Expand() {
            System.Array.Resize(ref Array, Count + expandSize);
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get if a given element is contained within the stamp.
        /// </summary>
        /// <param name="_element">Element to check if the stamp contain it.</param>
        /// <returns>True if the stamp contains the element, false otherwise.</returns>
        public bool Contains(T _element) {
            for (int i = 0; i < Count; i++) {
                if (Array[i].Equals(_element))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Modifies this stamp to match a given template content and size.
        /// </summary>
        /// <param name="_template">Template to copy.</param>
        public void Copy(Stamp<T> _template) {
            // Resize stamp and copy each element.
            Count = _template.Count;
            if (Array.Length < Count) {
                Resize();
            }

            for (int i = 0; i < Count; i++) {
                Array[i] = _template[i];
            }
        }
        #endregion
    }
}
