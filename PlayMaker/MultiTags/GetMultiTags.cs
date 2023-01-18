// ===== Enhanced Framework - https://github.com/LucasJoestar/EnhancedFramework ===== //
//
// Notes:
//
// ================================================================================== //

using EnhancedEditor;
using EnhancedFramework.Core;
using HutongGames.PlayMaker;
using UnityEngine;

using Tooltip = HutongGames.PlayMaker.TooltipAttribute;

namespace EnhancedFramework.PlayMaker {
    /// <summary>
    /// <see cref="FsmStateAction"/> used to get all tags in a <see cref="UnityEngine.GameObject"/>, using the Mutli-Tags system.
    /// </summary>
    [Tooltip("Get all tags in a Game Object and store them in a Multi Tags Behaviour Variable.")]
    [ActionCategory(ActionCategory.GameObject)]
    public class GetMultiTags : FsmStateAction {
        #region Global Members
        // -------------------------------------------
        // Variable - Store
        // -------------------------------------------

        [Tooltip("The Game Object to get tags.")]
        [RequiredField]
        public FsmOwnerDefault GameObject = null;

        [Tooltip("Store the Tags in a Multi Tags Behaviour.")]
        [RequiredField, ObjectType(typeof(MultiTagsBehaviour))]
        public FsmObject StoreBehaviour = null;
        #endregion

        #region Behaviour
        public override void Reset() {
            base.Reset();

            GameObject = null;
            StoreBehaviour = null;            
        }

        public override void OnEnter() {
            base.OnEnter();

            GetTags();
            Finish();
        }

        // -----------------------

        private void GetTags() {
            if (StoreBehaviour.Value is MultiTagsBehaviour _behaviour) {
                GameObject _gameObject = Fsm.GetOwnerDefaultTarget(GameObject);

                _behaviour.Tags = _gameObject.IsValid()
                                ? _gameObject.GetTags()
                                : new TagGroup();
            }
        }
        #endregion
    }
}
