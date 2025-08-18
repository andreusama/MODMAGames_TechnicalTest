using PetoonsStudio.PSEngine.Gameplay;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input
{
    public class InputMapsController : MonoBehaviour
    {
        public virtual void ChangeTo<T>(int index = -1, bool addToStack = false) where T : InputState
        {
            if (index != -1)
            {
                PlayerInput.all[index].GetComponent<PlayerInputController>().ChangeTo<T>(addToStack);
            }
            else
            {
                foreach (var playerInput in PlayerInput.all)
                {
                    playerInput.GetComponent<PlayerInputController>().ChangeTo<T>(addToStack);
                }
            }
        }

        public void ChangeTo(string assemblyQualifiedName, int index = -1, bool addToStack = false)
        {
            Type type = Type.GetType(assemblyQualifiedName);

            if (!type.IsSubclassOf(typeof(InputState)))
                return;

            if (index != -1)
            {
                PlayerInput.all[index].GetComponent<PlayerInputController>().ChangeTo(type, addToStack);
            }
            else
            {
                foreach (var playerInput in PlayerInput.all)
                {
                    playerInput.GetComponent<PlayerInputController>().ChangeTo(type, addToStack);
                }
            }
        }

        public virtual void AddMapToCurrentState(string mapName, int index = -1)
        {
            if (index != -1)
            {
                PlayerInput.all[index].GetComponent<PlayerInputController>().AddMapToState(mapName);
            }
            else
            {
                foreach (var playerInput in PlayerInput.all)
                {
                    playerInput.GetComponent<PlayerInputController>().AddMapToState(mapName);
                }
            }
        }

        public virtual void RemoveMapToCurrentState(string mapName, int index = -1)
        {
            if (index != -1)
            {
                PlayerInput.all[index].GetComponent<PlayerInputController>().RemoveMapToState(mapName);
            }
            else
            {
                foreach (var playerInput in PlayerInput.all)
                {
                    playerInput.GetComponent<PlayerInputController>().RemoveMapToState(mapName);
                }
            }
        }

        public virtual void RestorePreviousState(int index = -1)
        {
            if (index != -1)
            {
                PlayerInput.all[index].GetComponent<PlayerInputController>().RestorePreviousState();
            }
            else
            {
                foreach (var playerInput in PlayerInput.all)
                {
                    playerInput.GetComponent<PlayerInputController>().RestorePreviousState();
                }
            }
        }

        public virtual void GoGameplayState(int index = -1, bool addToStack = false)
        {
            ChangeTo<GameplayInputState>(index, addToStack);
        }
        public virtual void GoUIState(int index = -1, bool addToStack = false)
        {
            ChangeTo<UIInputState>(index, addToStack);
        }
        public virtual void GoCutsceneState(int index = -1, bool addToStack = false)
        {
            ChangeTo<CutsceneInputState>(index, addToStack);
        }
    }
}
