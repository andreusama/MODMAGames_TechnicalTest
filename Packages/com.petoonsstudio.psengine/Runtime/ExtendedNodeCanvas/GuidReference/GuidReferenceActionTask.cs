using CrossSceneReference;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using PetoonsStudio.PSEngine.QuestSystem;
using UnityEngine;

namespace PetoonsStudio.PSEngine.NodeCanvas
{
    [Category("GuidReference")]
    public abstract class GuidReferenceActionTask<T> : ActionTask<QuestLogic> where T: Component
    {

        [BlackboardOnly]
        public BBParameter<GuidReference> GuidReference;

        public GameObject GameObject => GuidReference.value.gameObject;

        protected T m_Target;

        protected override string OnInit()
        {
            m_Target = GameObject.GetComponent<T>();
            return base.OnInit();
        }
    }
}
