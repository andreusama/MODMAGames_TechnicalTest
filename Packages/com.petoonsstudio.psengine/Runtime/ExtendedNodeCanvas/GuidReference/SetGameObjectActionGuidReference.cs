using ParadoxNotion.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.NodeCanvas
{
    [Name("Set Gameobject State")]
    [Description("Set the gameobject active state.")]
    public class SetGameObjectActionGuidReference : GuidReferenceActionTask<Transform>
    {
        public enum SetActiveMode
        {
            Deactivate = 0,
            Activate = 1,
            Toggle = 2
        }

        public SetActiveMode setTo = SetActiveMode.Toggle;

        protected override string info
        {
            get { return string.Format("{0} {1}", setTo, agentInfo); }
        }

        protected override void OnExecute()
        {

            bool value;

            if (setTo == SetActiveMode.Toggle)
            {

                value = !GameObject.activeSelf;

            }
            else
            {

                value = (int)setTo == 1;
            }

            GameObject.SetActive(value);
            EndAction();
        }
    }
}
