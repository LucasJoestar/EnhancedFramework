// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using System.Collections;
using System.Collections.Generic;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Buffer used to push and pop values according to a priority system.
    /// </summary>
    /// <typeparam name="T">Buffer value type.</typeparam>
    public class Buffer<T> : IEnumerable<Pair<int, T>> {
        #region Global Members
        /// <summary>
        /// Default buffer value.
        /// </summary>
        public readonly T DefaultValue = default;

        /// <summary>
        /// Content of the buffer (elements are not ordered).
        /// </summary>
        public List<Pair<int, T>> Content = new List<Pair<int, T>>();

        /// <summary>
        /// Current active value.
        /// </summary>
        public T Value { get; private set; } = default;

        // -----------------------

        /// <inheritdoc cref="Buffer{T}(int)"/>
        public Buffer(T _defaultValue = default) {
            DefaultValue = _defaultValue;
            Value = _defaultValue;
        }

        /// <summary>
        /// <inheritdoc cref="Buffer{T}"/>
        /// </summary>
        /// <param name="_capacity">Initial capacity of the buffer.</param>
        public Buffer(int _capacity, T _defaultValue = default) : this(_defaultValue) {
            Content = new List<Pair<int, T>>(_capacity);
        }
        #endregion

        #region IEnumerable
        public IEnumerator<Pair<int, T>> GetEnumerator() {
            for (int i = Content.Count; i-- > 0;) {
                yield return Content[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Stack
        /// <summary>
        /// Set the active value of the buffer.
        /// </summary>
        /// <param name="_value">New active value.</param>
        /// <returns>Active buffer value.</returns>
        public T Set(T _value) {
            Value = _value;
            return _value;
        }

        /// <summary>
        /// Push a new value into the buffer.
        /// </summary>
        /// <param name="_value">New buffer value.</param>
        /// <param name="_id">Id associated with this value. Also acts as a priority
        /// <br/> (The active value will always be the one with the highest priority).</param>
        /// <returns><inheritdoc cref="Set(T)" path="/returns"/></returns>
        public T Push(T _value, int _id) {
            int _index = GetIndex(_id);

            if (_index > -1) {
                Content[_index] = new Pair<int, T>(_id, _value);
            } else {
                Content.Add(new Pair<int, T>(_id, _value));
            }

            return UpdateValue();
        }

        /// <summary>
        /// Pops and removes a value from the buffer.
        /// </summary>
        /// <param name="_id">Id of the value to remove from buffer.</param>
        /// <returns><inheritdoc cref="Set(T)" path="/returns"/></returns>
        public T Pop(int _id) {
            int _index = GetIndex(_id);

            if (_index > -1) {
                Content.RemoveAt(_index);
                UpdateValue();
            }

            return Value;
        }

        /// <summary>
        /// Resets the buffer and set its default value.
        /// </summary>
        /// <returns><inheritdoc cref="Set(T)" path="/returns"/></returns>
        public T Reset() {
            Content.Clear();
            return Set(DefaultValue);
        }

        // -----------------------

        /// <summary>
        /// Same as <see cref="Push(T, int)"/>.
        /// <para/> Mainly used for collection initialization.
        /// </summary>
        /// <param name="_pair">Pair value to insert into the buffer.</param>
        public void Add(Pair<int, T> _pair) {
            Push(_pair.Second, _pair.First);
        }
        #endregion

        #region Utility
        private int GetIndex(int _id) {
            return Content.FindIndex((v) => v.First == _id);
        }

        private T UpdateValue() {
            Pair<int, T> _newValue = new Pair<int, T>(int.MinValue, DefaultValue);

            if (Content.Count != 0) {
                // Search for the value with the highest priority.
                foreach (var _pair in Content) {
                    if (_pair.First > _newValue.First) {
                        _newValue = _pair;
                    }
                }
            }

            Value = _newValue.Second;
            return Value;
        }
        #endregion
    }
}
