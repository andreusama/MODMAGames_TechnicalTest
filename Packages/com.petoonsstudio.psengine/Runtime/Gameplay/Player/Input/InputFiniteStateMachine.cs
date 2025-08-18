using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class InputFiniteStateMachine : FiniteStateMachine<PlayerInput>
    {
    }

    public class InputState : FSMState<PlayerInput>
    {
        public bool RequiresInputUIModule { get; protected set; }
        public virtual List<string> Maps { get; protected set; }

        public InputState(PlayerInput context, FiniteStateMachine<PlayerInput> stateMachine) : base(context, stateMachine)
        {
            Maps = new();
#if PETOONS_DEBUG
            Maps.Add("Debug");
#endif
        }

        public void AddNewMap(string map)
        {
            Maps.Add(map);
            Update(map,true);
        }

        public void RemoveMap(string map)
        {
            Maps.Remove(map);
            Update(map,false);
        }

        private void Update(string map,bool enable)
        {
            var inputMap = m_Context.actions.FindActionMap(map);
            if (inputMap == null) return;
            if (enable)
                inputMap.Enable();
            else
                inputMap.Disable();
        }

        public override void Enter()
        {
            base.Enter();

            m_Context.DisableAllActions();

            foreach (string key in Maps)
            {
                var map = m_Context.actions.FindActionMap(key);
                if (map == null) continue;

                map.Enable();
            }
        }
    }

    public class GameplayInputState : InputState
    {
        public GameplayInputState(PlayerInput context, FiniteStateMachine<PlayerInput> stateMachine) : base(context, stateMachine)
        {
            RequiresInputUIModule = false;
            Maps.Add("Player");
        }
    }

    public class DialogueInputState : InputState
    {
        public DialogueInputState(PlayerInput context, FiniteStateMachine<PlayerInput> stateMachine) : base(context, stateMachine)
        {
            RequiresInputUIModule = true;
            Maps.Add("Dialogue");
        }
    }

    public class CutsceneInputState : InputState
    {
        public CutsceneInputState(PlayerInput context, FiniteStateMachine<PlayerInput> stateMachine) : base(context, stateMachine)
        {
            RequiresInputUIModule = false;
            Maps.Add("Cutscene");
        }
    }

    public class UIInputState : InputState
    {
        public UIInputState(PlayerInput context, FiniteStateMachine<PlayerInput> stateMachine) : base(context, stateMachine)
        {
            RequiresInputUIModule = true;
            Maps.Add("UI");
        }
    }
}
