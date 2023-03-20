// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;
using System;
using System.Diagnostics;
using UnityEngine;

using Object = UnityEngine.Object;

namespace EnhancedFramework.Core {
    /// <summary>
    /// <see cref="SaveData"/> data holder for a specific type collection.
    /// </summary>
    [Serializable]
    public class SaveDataType<T> {
        #region Global Members
        [JsonProperty] private PairCollection<string, T> collection = new PairCollection<string, T>();
        [JsonIgnore] private int index = 0;
        #endregion

        #region Utility
        /// <summary>
        /// Serializes a new data.
        /// </summary>
        /// <param name="_name">Name of the data to serialize.</param>
        /// <param name="_data">Data to serialize.</param>
        public void Serialize(string _name, T _data) {
            collection.Add(_name, _data);
        }

        /// <summary>
        /// Deserializes the next data.
        /// </summary>
        /// <returns>Next deserialized data.</returns>
        public T Deserialize() {
            if (index == collection.Count) {
                this.LogErrorMessage("Collection limit reached");
                return default;
            }

            return collection[index++].Second;
        }

        /// <summary>
        /// Clears this data content.
        /// </summary>
        internal void Clear() {
            collection.Clear();
            index = 0;
        }
        #endregion
    }

    /// <summary>
    /// <see cref="SaveManager"/> data holder used to serialize and deserialize values.
    /// </summary>
    [Serializable]
    public class SaveData {
        #region Data
        internal EnhancedObjectID id = EnhancedObjectID.None;

        // Value.
        public SaveDataType<object>   Object_data     = new SaveDataType<object>();
        public SaveDataType<string>   String_data     = new SaveDataType<string>();
        public SaveDataType<float>    Float_data      = new SaveDataType<float>();
        public SaveDataType<bool>     Bool_data       = new SaveDataType<bool>();
        public SaveDataType<int>      Int_data        = new SaveDataType<int>();

        // Struct.
        public SaveDataType<Quaternion>   Quaternion_data = new SaveDataType<Quaternion>();
        public SaveDataType<Vector2>      Vector2_data    = new SaveDataType<Vector2>();
        public SaveDataType<Vector3>      Vector3_data    = new SaveDataType<Vector3>();
        public SaveDataType<Vector4>      Vector4_data    = new SaveDataType<Vector4>();
        public SaveDataType<Color>        Color_data      = new SaveDataType<Color>();

        // Class.
        public SaveDataType<Object> UnityObject_data = new SaveDataType<Object>();
        #endregion

        #region Utility
        /// <summary>
        /// Setup this data for a specific <see cref="EnhancedObjectID"/>.
        /// </summary>
        /// <param name="_id"><see cref="EnhancedObjectID"/> to setup.</param>
        internal void Setup(EnhancedObjectID _id) {
            id.Copy(_id);
            Clear();
        }

        /// <summary>
        /// Clear this data content.
        /// </summary>
        internal void Clear() {
            Object_data.Clear();
            String_data.Clear();
            Float_data.Clear();
            Bool_data.Clear();
            Int_data.Clear();

            Quaternion_data.Clear();
            Vector2_data.Clear();
            Vector3_data.Clear();
            Vector4_data.Clear();
            Color_data.Clear();

            UnityObject_data.Clear();
        }
        #endregion
    }

    /// <summary>
    /// Application runtime serialization class.
    /// <br/> Used to serialize and deserialize data for any object in the game.
    /// </summary>
    [ScriptGizmos(false, true)]
    [DefaultExecutionOrder(-99)]
    [AddComponentMenu(FrameworkUtility.MenuPath + "Manager/Save Manager"), DisallowMultipleComponent]
    public class SaveManager : EnhancedSingleton<SaveManager> {
        #region Global Members
        private string saveJson = string.Empty;
        #endregion

        #region Registration
        private EnhancedCollection<ISaveable> savableObjects = new EnhancedCollection<ISaveable>();

        // -----------------------

        /// <summary>
        /// Registers a new <see cref="ISaveable"/> for save.
        /// </summary>
        /// <param name="_savable"><see cref="ISaveable"/> instance to register.</param>
        public void Register(ISaveable _savable) {
            #if DEVELOPMENT
            // Development security.
            if (savableObjects.Contains(_savable)) {
                this.LogErrorMessage($"Savable already registered - {_savable.GetType().Name.Bold()}", _savable.LogObject);
                return;
            }
            #endif

            savableObjects.Add(_savable);
            Deserialize(_savable);
        }

        /// <summary>
        /// Unregisters a new <see cref="ISaveable"/> from save.
        /// </summary>
        /// <param name="_savable"><see cref="ISaveable"/> instance to unregister.</param>
        public void Unregister(ISaveable _savable) {
            Serialize(_savable);
            savableObjects.Remove(_savable);
        }
        #endregion

        #region Serialization
        private static SaveData data = new SaveData();

        // List json per:
        //
        //  • Scene
        //  • Global Objects (Prefabs / Scriptable Objects)
        //  • Dynamic Instances

        // -----------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_savable"></param>
        public void Serialize(ISaveable _savable) {
            data.Setup(_savable.ID);

            _savable.Serialize(data);
            string _json = JsonConvert.SerializeObject(data);

            // Write json.
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_savable"></param>
        public void Deserialize(ISaveable _savable) {
            string _json = string.Empty;

            // Read json.

            data = JsonConvert.DeserializeObject<SaveData>(_json);
            _savable.Deserialize(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_savable"></param>
        public void RemoveData(ISaveable _savable) {
            // Get and remove data of id.
        }
        #endregion
    }
}
