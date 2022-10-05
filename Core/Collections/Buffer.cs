// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Buffer used to push and pop values according to a priority system.
    /// </summary>
    /// <typeparam name="T">Buffer value type.</typeparam>
    /// <typeparam name="U">Buffer key type.</typeparam>
    /// <typeparam name="V">Buffer priority value type.</typeparam>
    public abstract class Buffer<T, U, V> : IEnumerable<KeyValuePair<U, V>> {
        #region Global Members
        /// <summary>
        /// Default buffer value.
        /// </summary>
        public T DefaultValue = default;

        /// <summary>
        /// Content of the buffer (elements are not ordered).
        /// </summary>
        public Dictionary<U, V> Content = new Dictionary<U, V>();

        /// <summary>
        /// Current active value.
        /// </summary>
        public T Value { get; private set; } = default;

        /// <summary>
        /// Called whenever this buffer active value is changed (new value as first, previous one as second).
        /// </summary>
        public Action<T, T> OnValueChanged = null;

        // -----------------------

        /// <inheritdoc cref="Buffer{T}(int)"/>
        public Buffer(T _defaultValue = default, Action<T, T> _onValueChanged = null) {
            DefaultValue = _defaultValue;
            Value = _defaultValue;

            OnValueChanged = _onValueChanged;
        }

        /// <summary>
        /// <inheritdoc cref="Buffer{T}"/>
        /// </summary>
        /// <param name="_capacity">Initial capacity of the buffer.</param>
        public Buffer(int _capacity, T _defaultValue = default, Action<T, T> _onValueChanged = null) : this(_defaultValue, _onValueChanged) {
            Content = new Dictionary<U, V>(_capacity);
        }
        #endregion

        #region IEnumerable
        public IEnumerator<KeyValuePair<U, V>> GetEnumerator() {
            foreach (var _pair in Content) {
                yield return _pair;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        #endregion

        #region Buffer
        /// <summary>
        /// Set the active value of the buffer.
        /// </summary>
        /// <param name="_value">New active value.</param>
        /// <returns>The active buffer value.</returns>
        public T Set(T _value) {
            OnValueChanged?.Invoke(_value, Value);
            Value = _value;

            return _value;
        }

        /// <summary>
        /// Push a new value into the buffer.
        /// </summary>
        /// <param name="_key">Key of the value.</param>
        /// <param name="_priorityValue">Priority value to be associated with the key.</param>
        /// <returns><inheritdoc cref="Set(T)" path="/returns"/></returns>
        public T Push(U _key, V _priorityValue) {
            if (Content.ContainsKey(_key)) {
                Content[_key] = _priorityValue;
            } else {
                Content.Add(_key, _priorityValue);
            }

            return UpdateValue();
        }

        /// <summary>
        /// Pops and removes a value from the buffer.
        /// </summary>
        /// <param name="_key">Key of the value to remove.</param>
        /// <returns><inheritdoc cref="Set(T)" path="/returns"/></returns>
        public T Pop(U _key) {
            Content.Remove(_key);
            return UpdateValue();
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
        /// Same as <see cref="Push(T, int, int)"/>.
        /// <para/> Mainly used for collection initialization.
        /// </summary>
        /// <param name="_value">The value to insert into the buffer.</param>
        public void Add(KeyValuePair<U, V> _pair) {
            Push(_pair.Key, _pair.Value);
        }
        #endregion

        #region Value
        private T UpdateValue() {
            Pair<U, V> _value = GetDefaultValue();

            if (Content.Count != 0) {
                // Search for the value with the highest priority.
                foreach (var _pair in Content) {
                    if (HasPriority(_pair, _value)) {
                        _value = new Pair<U, V>(_pair.Key, _pair.Value);
                    }
                }
            }

            T _newValue = GetValue(_value);

            if (!EqualityComparer<T>.Default.Equals(Value, _newValue)) {
                OnValueChanged?.Invoke(_newValue, Value);
                Value = _newValue;
            }

            return Value;
        }

        // -----------------------

        protected abstract Pair<U, V> GetDefaultValue();

        protected abstract bool HasPriority(Pair<U, V> _new, Pair<U, V> _current);

        protected abstract T GetValue(Pair<U, V> _pair);
        #endregion
    }

    /// <summary>
    /// <inheritdoc cref="Buffer{T, U, V}"/>
    /// <para/>
    /// Uses the value as the key.
    /// </summary>
    /// <typeparam name="T">Key / value type.</typeparam>
    public class BufferR<T> : Buffer<T, T, int> {
        #region Constructor
        /// <inheritdoc cref="Buffer{T, U, V}.Buffer(T, Action{T, T})"/>
        public BufferR(T _defaultValue = default, Action<T, T> _onValueChanged = null) : base(_defaultValue, _onValueChanged) { }

        /// <inheritdoc cref="Buffer{T, U, V}.Buffer(int, T, Action{T, T})"/>
        public BufferR(int _capacity, T _defaultValue = default, Action<T, T> _onValueChanged = null) : base(_capacity, _defaultValue, _onValueChanged) { }
        #endregion

        #region Value
        protected override Pair<T, int> GetDefaultValue() {
            return new Pair<T, int>(DefaultValue, int.MinValue);
        }

        protected override bool HasPriority(Pair<T, int> _new, Pair<T, int> _current) {
            return _new.Second > _current.Second;
        }

        protected override T GetValue(Pair<T, int> _pair) {
            return _pair.First;
        }
        #endregion
    }

    /// <summary>
    /// <inheritdoc cref="Buffer{T, U, V}"/>
    /// <para/>
    /// Uses the priority as the key.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public class BufferI<T> : Buffer<T, int, T> {
        #region Constructor
        /// <inheritdoc cref="Buffer{T, U, V}.Buffer(T, Action{T, T})"/>
        public BufferI(T _defaultValue = default, Action<T, T> _onValueChanged = null) : base(_defaultValue, _onValueChanged) { }

        /// <inheritdoc cref="Buffer{T, U, V}.Buffer(int, T, Action{T, T})"/>
        public BufferI(int _capacity, T _defaultValue = default, Action<T, T> _onValueChanged = null) : base(_capacity, _defaultValue, _onValueChanged) { }
        #endregion

        #region Value
        protected override Pair<int, T> GetDefaultValue() {
            return new Pair<int, T>(int.MinValue, DefaultValue);
        }

        protected override bool HasPriority(Pair<int, T> _new, Pair<int, T> _current) {
            return _new.First > _current.First;
        }

        protected override T GetValue(Pair<int, T> _pair) {
            return _pair.Second;
        }
        #endregion
    }

    /// <summary>
    /// <inheritdoc cref="Buffer{T, U, V}"/>
    /// <para/>
    /// Let the user use a different object for the value, the key and the priority.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    public class BufferV<T> : Buffer<T, int, Pair<T, int>> {
        #region Constructor
        /// <inheritdoc cref="Buffer{T, U, V}.Buffer(T, Action{T, T})"/>
        public BufferV(T _defaultValue = default, Action<T, T> _onValueChanged = null) : base(_defaultValue, _onValueChanged) { }

        /// <inheritdoc cref="Buffer{T, U, V}.Buffer(int, T, Action{T, T})"/>
        public BufferV(int _capacity, T _defaultValue = default, Action<T, T> _onValueChanged = null) : base(_capacity, _defaultValue, _onValueChanged) { }
        #endregion

        #region Value
        protected override Pair<int, Pair<T, int>> GetDefaultValue() {
            return new Pair<int, Pair<T, int>>(int.MinValue, new Pair<T, int>(DefaultValue, int.MinValue));
        }

        protected override bool HasPriority(Pair<int, Pair<T, int>> _new, Pair<int, Pair<T, int>> _current) {
            return _new.Second.Second > _current.Second.Second;
        }

        protected override T GetValue(Pair<int, Pair<T, int>> _pair) {
            return _pair.Second.First;
        }
        #endregion
    }
}
