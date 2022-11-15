// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;
using RLoader = EnhancedEditor.Reference<EnhancedFramework.Core.ResourceLoader>;

[assembly: InternalsVisibleTo("EnhancedFramework.Editor")]
namespace EnhancedFramework.Core {
    /// <summary>
    /// Base <see cref="ResourceLoader{T}"/>-related resource interface.
    /// <br/> Always prefer using <see cref="IResourceBehaviour{T}"/> instead of this interface when possible.
    /// <para/>
    /// This only exist to serialize all behaviours using a single non-generic type.
    /// </summary>
    public interface IResourceBehaviour {
        #region Content
        /// <summary>
        /// This resource instance object to use for logging messages to the console.
        /// </summary>
        Object LogObject { get; }
        #endregion
    }

    /// <summary>
    /// Generic interface to inherit your own <see cref="ResourceLoader{T}"/>-related resources from.
    /// </summary>
    /// <typeparam name="T">This behaviour related <see cref="ResourceLoader{T}"/> type.</typeparam>
    public interface IResourceBehaviour<T> : IResourceBehaviour where T : ResourceLoader, new() {
        #region Content
        /// <summary>
        /// Fills a specific <see cref="ResourceLoader"/> with this behaviour.
        /// </summary>
        /// <param name="_resource">The <see cref="ResourceLoader"/> to fill with this behaviour.</param>
        void FillResource(T _resource);
        #endregion
    }

    /// <summary>
    /// Base loading resource class.
    /// <br/> Always inherit from <see cref="ResourceLoader{T}"/> instead of this class.
    /// <para/>
    /// This only exist to serialize all loaders using a single non-generic type.
    /// </summary>
    [Serializable]
    public abstract class ResourceLoader {
        #region Global Members
        /// <summary>
        /// The total count of <see cref="IResourceBehaviour"/> in this loader.
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Indicates whether this loader is currently loading or unloading any resource.
        /// </summary>
        public abstract bool IsProcessing { get; }

        // -----------------------

        /// <summary>
        /// Prevents inheriting from this class in other assemblies.
        /// </summary>
        private protected ResourceLoader() { }
        #endregion

        #region Setup
        /// <summary>
        /// Registers a new resource behaviour instance in this loader.
        /// </summary>
        /// <param name="_behaviour">The <see cref="IResourceBehaviour"/> to register.</param>
        /// <returns>True if the behaviour could be successfully registered, false otherwise.</returns>
        internal abstract bool RegisterBehaviour(IResourceBehaviour _behaviour);

        /// <summary>
        /// Fills this loader with all its resources before loading.
        /// </summary>
        internal protected abstract void Setup();
        #endregion

        #region Behaviour
        /// <summary>
        /// Starts loading all resources in this loader.
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Starts unloading all resources in this loader.
        /// </summary>
        public abstract void Unload();
        #endregion
    }

    /// <summary>
    /// Base class to inherit your own resource loaders from.
    /// <para/>
    /// Use this to dynamically load and unload resources from a scene.
    /// </summary>
    /// <typeparam name="T">This <see cref="ResourceLoader"/> class type.</typeparam>
    [Serializable]
    public abstract class ResourceLoader<T> : ResourceLoader where T : ResourceLoader, new() {
        #region Global Members
        /// <summary>
        /// All registered behaviours in this loader.
        /// </summary>
        public List<SerializedInterface<IResourceBehaviour<T>>> Behaviours = new List<SerializedInterface<IResourceBehaviour<T>>>();

        // -----------------------

        public override int Count {
            get { return Behaviours.Count; }
        }
        #endregion

        #region Setup
        internal override bool RegisterBehaviour(IResourceBehaviour _behaviour) {
            if (_behaviour is IResourceBehaviour<T> _tBehaviour) {
                Behaviours.Add(new SerializedInterface<IResourceBehaviour<T>>(_tBehaviour));
                return true;
            }

            return false;
        }

        internal protected override void Setup() {
            foreach (SerializedInterface<IResourceBehaviour<T>> _behaviour in Behaviours) {
                _behaviour.Interface.FillResource(this as T);
            }
        }
        #endregion

        #region Utility
        /// <summary>
        /// Get this loader <see cref="IResourceBehaviour{T}"/> at the given index.
        /// <para/>
        /// You can use <see cref="Count"/> to get the total amount of behaviours in this loader.
        /// </summary>
        /// <param name="_index">The index at which to get the desired behaviour.</param>
        /// <returns>The <see cref="IResourceBehaviour{T}"/> instance of this loader at the given index.</returns>
        public IResourceBehaviour<T> GetBehaviourAt(int _index) {
            return Behaviours[_index].Interface;
        }
        #endregion
    }

    /// <summary>
    /// Scene-depending manager class, used to automatically load and unload
    /// <br/> all resources associated with it following its lifetime.
    /// </summary>
    public class SceneResourceManager : EnhancedBehaviour, ILoadingProcessor, IUnloadingCallbackReceiver {
        public override UpdateRegistration UpdateRegistration => base.UpdateRegistration | UpdateRegistration.Init;

        #region Global Members
        [Section("Scene Resource Manager")]

        [SerializeField, Enhanced, ReadOnly] private List<RLoader> resources = new List<RLoader>();

        // -----------------------
        
        public bool IsProcessing {
            get { return resources.Exists(r => r.Value.IsProcessing); }
        }
        #endregion

        #region Enhanced Behaviour
        protected override void OnInit() {
            base.OnInit();

            // Registration.
            EnhancedSceneManager.Instance.RegisterProcessor(this);
            EnhancedSceneManager.Instance.RegisterUnloadingCallbackReceiver(this);
            
            // Get and load all resources during initialization.
            foreach (ResourceLoader _resource in resources) {
                _resource.Setup();
                _resource.Load();
            }
        }

        private void OnDestroy() {
            // Unregistration.
            EnhancedSceneManager.Instance.UnregisterProcessor(this);
            EnhancedSceneManager.Instance.UnregisterUnloadingCallbackReceiver(this);
        }

        // -----------------------

        void IUnloadingCallbackReceiver.OnPreUnloadBundle(SceneBundle _bundle) {
            // When this object scene is about to be unloaded, unload its associated resources.
            Scene _scene = gameObject.scene;
            if (!_bundle.Scenes.Find(s => s.Scene == _scene, out _)) {
                return;
            }

            for (int i = resources.Count; i-- > 0;) {
                resources[i].Value.Unload();
            }
        }
        #endregion

        #region Editor
        #if UNITY_EDITOR
        /// <summary>
        /// Called from an editor class to serialize all resources in the scene.
        /// </summary>
        internal void GetSceneResources() {
            // Each time this object scene is saved, register all resources in it.
            GameObject[] _objects = gameObject.scene.GetRootGameObjects();
            resources.Clear();

            foreach (GameObject _object in _objects) {
                IResourceBehaviour[] _behaviours = _object.GetComponentsInChildren<IResourceBehaviour>();

                foreach (IResourceBehaviour _behaviour in _behaviours) {
                    RegisterBehaviour(_behaviour);
                }
            }

            // ----- Local Methods ----- \\

            void RegisterBehaviour(IResourceBehaviour _behaviour) {
                foreach (ResourceLoader _resource in resources) {
                    if (_resource.RegisterBehaviour(_behaviour)) {
                        return;
                    }
                }

                // If no appropriate loader could be found,
                // find the behaviour interface and create a new instance of its loader.
                foreach (Type _type in _behaviour.GetType().GetInterfaces()) {

                    if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(IResourceBehaviour<>)) {

                        Type _resourceType = _type.GetGenericArguments()[0];
                        ResourceLoader _loader = Activator.CreateInstance(_resourceType) as ResourceLoader;

                        if (_loader.RegisterBehaviour(_behaviour)) {
                            resources.Add(_loader);
                            return;
                        }
                    }
                }
            }
        }
        #endif
        #endregion
    }
}
