// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using UnityEngine;

namespace EnhancedFramework.Core {
    /// <summary>
    /// Implement this interface to serialize / deserialize any values for an object during runtime.
    /// <para/> Use the <see cref="SaveManager"/> class to manipulate an object data.
    /// </summary>
    public interface ISaveable {
        #region Content
        /// <summary>
        /// Unique id used to serialize and deserialize this object.
        /// </summary>
        public EnhancedObjectID ID { get; }

        /// <summary>
        /// Object used for logging messages.
        /// <br/> Use the <see cref="SaveManager"/> instance if none.
        /// </summary>
        public Object LogObject { get; }

        /// <summary>
        /// Serializes this object data.
        /// <br/> Use this to write and save values.
        /// </summary>
        /// <param name="_data">Use this to serialize data (don't use deserialization).</param>
        public void Serialize(SaveData _data);

        /// <summary>
        /// Deserializes this object data.
        /// <br/> Use this to override values.
        /// <para/>
        /// Deserialization should be performed in the same order than for serialization.
        /// </summary>
        /// <param name="_data">Use this to deserialize data (don't use serialization).</param>
        public void Deserialize(SaveData _data);
        #endregion
    }
}
