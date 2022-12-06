// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Base <see cref="List{T}"/>-wrapper class to dervive multiple enhanced collections from.
    /// </summary>
    /// <typeparam name="T">This collection content type.</typeparam>
    [Serializable]
    public class EnhancedCollection<T> : IEnumerable<T>, IList<T> {
        #region Global Members
        /// <summary>
        /// The <see cref="List{T}"/> wrapped in this collection.
        /// </summary>
        [SerializeField] protected List<T> collection = new List<T>();

        // -----------------------

        /// <summary>
        /// The total amount of element in this collection.
        /// </summary>
        public virtual int Count {
            get { return collection.Count; }
        }

        /// <summary>
        /// The total capacity of this collcetion.
        /// </summary>
        public virtual int Capacity {
            get { return collection.Capacity; }
        }

        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }
        #endregion

        #region Constructor
        /// <inheritdoc cref="Set{T}"/>
        public EnhancedCollection() {
            collection = new List<T>();
        }

        /// <param name="_collection">The other collection to initialize this collection with.</param>
        /// <inheritdoc cref="Set{T}"/>
        public EnhancedCollection(IEnumerable<T> _collection) {
            collection = new List<T>(_collection);
        }

        /// <param name="_capacity">Initial capcity of this collection.</param>
        /// <inheritdoc cref="Set{T}"/>
        public EnhancedCollection(int _capacity) {
            collection = new List<T>(_capacity);
        }
        #endregion

        #region Operators
        public virtual T this[int _index] {
            get {
                return collection[_index];
            }
            set {
                collection[_index] = value;
            }
        }
        #endregion

        #region IEnumerable
        public IEnumerator<T> GetEnumerator() {
            for (int i = Count; i-- > 0;) {
                yield return collection[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Collection
        /// <summary>
        /// Adds a new element in this collection.
        /// </summary>
        /// <param name="_element">The element to add.</param>
        public virtual void Add(T _element) {
            collection.Add(_element);
        }

        /// <summary>
        /// Adds an other collection content in this collection.
        /// </summary>
        /// <param name="_collection">The other collection to add the content in this collection.</param>
        public virtual void AddRange(IEnumerable<T> _collection) {
            collection.AddRange(_collection);
        }

        /// <summary>
        /// Insert a new element at a specific index from this collection.
        /// </summary>
        /// <param name="_index">The index where to insert the new element.</param>
        /// <param name="_element">The element element to insert.</param>
        public virtual void Insert(int _index, T _element) {
            collection.Insert(_index, _element);
        }

        /// <summary>
        /// Copies the content of a specific array into this collection.
        /// </summary>
        /// <param name="_array">The array to copy the content in this collection.</param>
        /// <param name="_arrayIndex">The index of this collection where to copy the array content.</param>
        public virtual void CopyTo(T[] _array, int _arrayIndex) {
            collection.CopyTo(_array, _arrayIndex);
        }

        /// <param name="_element">Element of the array to move.</param>
        /// <returns>True if the element was part of this collection and could be moved, false otherwise.</returns>
        /// <inheritdoc cref="Shift(int, int)"/>
        public virtual bool Shift(T _element, int _shiftIndex) {
            int _index = IndexOf(_element);

            if (_index > -1) {
                Shift(_index, _shiftIndex);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shift an element of this collection to a new index, shifting all elements in the ways.
        /// </summary>
        /// <param name="_index">The index of the element to shift.</param>
        /// <param name="_shiftIndex">The destination index of the element to shift.</param>
        public virtual void Shift(int _index, int _shiftIndex) {
            if ((_index == _shiftIndex) || (_shiftIndex < 0) || (_shiftIndex >= Count)) {
                return;
            }

            T _element = collection[_index];

            if (_index < _shiftIndex) {
                for (int _i = _index; _i < _shiftIndex; _i++) {
                    collection[_i] = collection[_i + 1];
                }
            } else {
                for (int _i = _index; _i-- > _shiftIndex;) {
                    collection[_i + 1] = collection[_i];
                }
            }

            collection[_shiftIndex] = _element;
        }

        /// <inheritdoc cref="Fill(T, int, int)"/>
        public virtual void Fill(T _value) {
            for (int _i = 0; _i < Count; _i++) {
                collection[_i] = _value;
            }
        }

        /// <summary>
        /// Fills this collection with a specific element value.
        /// </summary>
        /// <param name="_value">The element value to fill this collection with.</param>
        /// <param name="_index">The index at which to start filling this collection.</param>
        /// <param name="_count">The total amount of elements from this collection to fill.</param>
        public virtual void Fill(T _value, int _index, int _count) {
            int _length = Math.Min(_index + _count, Count);

            for (int _i = _index; _i < _length; _i++) {
                collection[_i] = _value;
            }
        }

        /// <summary>
        /// Removes a specific element from this collection.
        /// </summary>
        /// <param name="_element">The element to remove.</param>
        /// <returns>True if the element was part of this collection and could be removed, false otherwise.</returns>
        public virtual bool Remove(T _element) {
            return collection.Remove(_element);
        }

        /// <summary>
        /// Removes the element at a specific index from this collection.
        /// </summary>
        /// <param name="_index">The index of the element to remove.</param>
        public virtual void RemoveAt(int _index) {
            collection.RemoveAt(_index);
        }

        /// <summary>
        /// Removes the first element from this collection.
        /// </summary>
        /// <returns>True if this collection is not empty and the first element could be removed, false otherwise.</returns>
        public virtual bool RemoveFirst() {
            if (Count != 0) {
                RemoveAt(0);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the last element from this collection.
        /// </summary>
        /// <returns>True if this collection is not empty and the last element could be removed, false otherwise.</returns>
        public virtual bool RemoveLast() {
            if (Count != 0) {
                RemoveAt(Count - 1);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all null entries from this collection.
        /// </summary>
        public virtual void RemoveNull() {
            for (int i = Count; i-- > 0;) {
                if (collection[i] == null) {
                    RemoveAt(i);
                }
            }
        }
        #endregion

        #region Container
        /// <summary>
        /// Get the index of a specific element from this collection.
        /// </summary>
        /// <param name="_element">The element to get the index from this collection.</param>
        /// <returns>-1 if the element could not be found in this collection, or its index otherwise.</returns>
        public virtual int IndexOf(T _element) {
            return collection.IndexOf(_element);
        }

        /// <summary>
        /// Find the first matching element from this collection.
        /// </summary>
        /// <param name="_match">Used to know if an element match.</param>
        /// <param name="_match">The first matching element found (null if none).</param>
        /// <returns>True if the element is contained in the collection, false otherwise.</returns>
        public virtual bool Find(Predicate<T> _match, out T _element) {
            if (FindIndex(_match, out int _index)) {
                _element = collection[_index];
                return true;
            }

            _element = default;
            return false;
        }

        /// <summary>
        /// Find the index of the first matching element from this collection.
        /// </summary>
        /// <param name="_match">Used to know if an element match.</param>
        /// <returns>-1 if no matching element could be found, or the element index if found.</returns>
        public virtual int FindIndex(Predicate<T> _match) {
            return collection.FindIndex(_match);
        }

        /// <summary>
        /// Find the index of the first matching element from this collection.
        /// </summary>
        /// <param name="_match">Used to know if an element match.</param>
        /// <param name="_index">-1 if no matching element could be found, or the element index if found.</param>
        /// <returns>True if the element is contained in the collection, false otherwise.</returns>
        public virtual bool FindIndex(Predicate<T> _match, out int _index) {
            _index = FindIndex(_match);
            return _index != -1;
        }

        /// <summary>
        /// Get if a specific element is contained in this collection.
        /// </summary>
        /// <param name="_element">The element to check.</param>
        /// <returns>True if the element is contained in the collection, false otherwise.</returns>
        public virtual bool Contains(T _element) {
            return collection.Contains(_element);
        }

        /// <summary>
        /// Get if any element in this collection match a specific condition.
        /// </summary>
        /// <param name="_match">Used to know if an element match.</param>
        /// <returns>True if any element in this collection matches the condition, false otherwise.</returns>
        public virtual bool Exists(Predicate<T> _match) {
            return collection.Exists(_match);
        }

        /// <summary>
        /// Get the first element int this collection.</typeparam>
        /// </summary>
        /// <returns>The first element from this collection.</returns>
        public virtual T First() {
            return collection[0];
        }

        /// <summary>
        /// Get the last element from this collection.
        /// </summary>
        /// <returns>The last element from this collection.</returns>
        public virtual T Last() {
            return collection[Count - 1];
        }

        /// <summary>
        /// A safe version of <see cref="First"/>.
        /// <br/> Get the first element from this collection, or ots default value if empty.
        /// </summary>
        /// <param name="_element">The first element from this collection, or its default value if empty.</param>
        /// <returns>True if the collection is not empty and an element could be found, false otherwise.</returns>
        public bool SafeFirst(out T _element) {
            return SafeGetter(First, out _element);
        }

        /// <summary>
        /// A safe version of <see cref="Last"/>.
        /// <br/> Get the last element from this collection, or its default value if empty.
        /// </summary>
        /// <param name="_element">The last element from this collection, or its default value if empty.</param>
        /// <returns>True if the collection is not empty and an element could be found, false otherwise.</returns>
        public bool SafeLast(out T _element) {
            return SafeGetter(Last, out _element);
        }

        // -----------------------

        private bool SafeGetter(Func<T> _getter, out T _element) {
            if (Count == 0) {
                _element = default;
                return false;
            }

            _element = _getter();
            return true;
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get a random element from this collection.
        /// </summary>
        /// <returns>A random element from this collection.</returns>
        public virtual T Random() {
            int _count = Count;
            if (_count == 0) {
                return default;
            }

            return collection[UnityEngine.Random.Range(0, _count)];
        }

        /// <inheritdoc cref="Sort(int, int, IComparer{T})"/>
        public virtual void Sort() {
            collection.Sort();
        }

        /// <param name="_comparison">Comparison used to sort each element.</param>
        /// <inheritdoc cref="Sort(int, int, IComparer{T})"/>
        public void Sort(Comparison<T> _comparison) {
            collection.Sort(_comparison);
        }

        /// <inheritdoc cref="Sort(int, int, IComparer{T})"/>
        public void Sort(IComparer<T> _comparer) {
            collection.Sort(_comparer);
        }

        /// <summary>
        /// Sorts the elements in this collection.
        /// </summary>
        /// <param name="_index">The index where to start sorting elements.</param>
        /// <param name="_count">The total count of elements to sort.</param>
        /// <param name="_comparer">Comparer used to sort each element.</param>
        public void Sort(int _index, int _count, IComparer<T> _comparer) {
            collection.Sort(_index, _count, _comparer);
        }

        /// <summary>
        /// Clears the content of this collection.
        /// </summary>
        public virtual void Clear() {
            collection.Clear();
        }

        // -----------------------

        protected bool Compare<U>(U _first, U _second) {
            return EqualityComparer<U>.Default.Equals(_first, _second);
        }
        #endregion
    }
}
