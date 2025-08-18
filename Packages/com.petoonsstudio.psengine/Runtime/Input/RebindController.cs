using Newtonsoft.Json;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input
{
    public struct MMRebindEvent
    {
        public InputAction Action;
        public string PreviousBinding;
        public string NewBinding;
        public int PlayerID;

        public MMRebindEvent(InputAction action, string previousBinding, string newBinding, int playerID = 0)
        {
            Action = action;
            PreviousBinding = previousBinding;
            NewBinding = newBinding;
            PlayerID = playerID;
        }
    }

    public struct MMBindingsResetEvent
    {
        public int PlayerID;

        public MMBindingsResetEvent(int playerID = -1)
        {
            PlayerID = playerID;
        }
    }

    [Serializable]
    public struct RebindChange
    {
        public string Map;
        public string Action;
        public string OriginalBindingPath;
        public string OverridedBindingPath;

        public RebindChange(string map, string action, string originalBindingPath, string overridedBindingPath)
        {
            Map = map;
            Action = action;
            OriginalBindingPath = originalBindingPath;
            OverridedBindingPath = overridedBindingPath;
        }
        public RebindChange(InputAction action, int bindingIndex, string newPath)
        {
            Action = action.name;
            Map = action.actionMap.name;
            OverridedBindingPath = newPath;
            OriginalBindingPath = action.bindings[bindingIndex].path;
        }
    }

    public class RebindController : MonoBehaviour
    {
        [SerializeField, Tooltip("Time to wait OnMatchWaitForAnother when remapping.")]
        private float m_DelayForRebindAcceptInput = 0.1f;
        
        public Action<InputActionRebindingExtensions.RebindingOperation> OnRebindComplete;
        public Action<InputActionRebindingExtensions.RebindingOperation> OnRebindCancel;
        public Action<InputActionRebindingExtensions.RebindingOperation, string> OnRebindFail;
        public Action<InputActionRebindingExtensions.RebindingOperation, string, string, int, int> OnRebindApply;

        public Dictionary<int, List<RebindChange>> RebindChanges = new SerializedDictionary<int, List<RebindChange>>();

        /// <summary>
        /// Use to remap an action, this function uses some generic or commonly used modifiers for a RebindingOperation.
        /// If you want to change them or apply new ones use the CustomRemapAction function instead.
        /// To change specific binding provide a value for the Index.
        /// </summary>
        /// <param name="action">Action to remap.</param>
        /// <param name="bindingIndex"> -1 = Override will be applied to all bindings / >= 0 = Index of all the current bindings that belongs to the current platform that will be modified</param>
        public void RemapAction(InputAction action, int playerIndex, int bindingIndex = 0)
        {
            SetupInputRebind(action, playerIndex);

            var rebindOperation = action.PerformInteractiveRebinding()
                       .WithControlsExcluding("Mouse")
                       .WithControlsExcluding("Left Stick")
                       .WithCancelingThrough("<Keyboard>/escape")
                       .WithCancelingThrough("<Gamepad>/start")
                       .WithControlsHavingToMatchPath(InputManager.GetControlGenericName())
                       .OnMatchWaitForAnother(m_DelayForRebindAcceptInput)
                       .Start();

            rebindOperation.OnCancel(op =>
            {
                OnBindingCancel(op);
            });

            rebindOperation.OnApplyBinding((op, path) =>
            {
                OnBindingApply(op, path, bindingIndex, playerIndex);
            });

            rebindOperation.OnComplete(op =>
            {
                OnBindingComplete(op);
            });
        }

        /// <summary>
        /// This operation does not add any modifiers beyond the minimum necessary for customisation or proper operation.
        /// To change specific binding provide a value for the Index.
        /// </summary>
        /// <param name="action">Action to remap.</param>
        /// <param name="bindingIndex"> -1 = Override will be applied to all bindings / >= 0 = Index of all the current bindings that belongs to the current platform that will be modified</param>
        public InputActionRebindingExtensions.RebindingOperation CustomRemapAction(InputAction action, int playerIndex = 0, int bindingIndex = 0)
        {
            SetupInputRebind(action, playerIndex);

            return action.PerformInteractiveRebinding()
                       .OnMatchWaitForAnother(m_DelayForRebindAcceptInput)
                       .OnApplyBinding((op, path) => OnBindingApply(op, path, bindingIndex, playerIndex))
                       .OnCancel((op) => OnBindingCancel(op))
                       .OnComplete((op) => OnBindingComplete(op));
        }

        /// <summary>
        /// Swap repeated binding paths of the same scheme between maps.
        /// </summary>
        /// <param name="rebindedAction"></param>
        /// <param name="newPath"></param>
        /// <param name="oldPath"></param>
        public virtual void SwapRepeatedBindInMap(InputAction rebindedAction, string newPath, string oldPath, int binidingIndexChanged, int playerID)
        {
            UpdateInputAssetWithRebindChanges(rebindedAction.actionMap.asset, playerID);

            var actionMap = rebindedAction.actionMap;
            if (actionMap == null)
                return;

            foreach (var otherAction in actionMap.actions)
            { 
                //For each different action check bindings
                for (int i = 0; i < otherAction.bindings.Count; i++)
                {
                    var currentBinding = otherAction.bindings[i];

                    //Continue if it's not matching current scheme
                    if (!InputManager.Instance.IsBindingMatchingCurrentScheme(currentBinding))
                        continue;

                    var otherBindingFixedPath = InputManager.Instance.FixPath(currentBinding.effectivePath);

                    //If paths are the same, then it's a swap and we need to change the binding
                    if (otherBindingFixedPath == newPath && (otherAction != rebindedAction || binidingIndexChanged != i))
                    {
                        //Swapping other action binding
                        SaveRebind(otherAction, oldPath, i, playerID);
                        PSEventManager.TriggerEvent(new MMRebindEvent(otherAction, otherBindingFixedPath, oldPath, playerID));
                    }
                }
            }
        }

        /// <summary>
        /// Swap repeated binding paths in the actions array.
        /// </summary>
        /// <param name="rebindedAction"></param>
        /// <param name="actions"></param>
        /// <param name="newPath"></param>
        /// <param name="oldPath"></param>
        public virtual void SwapRepeatedBindInActions(InputAction rebindedAction, InputActionReference[] actions, string newPath, string oldPath, int binidingIndexChanged, int playerID)
        {
            UpdateInputAssetWithRebindChanges(rebindedAction.actionMap.asset, playerID);

            foreach (var otherAction in actions)
            {
                //For each different action check bindings
                for (int i = 0; i < otherAction.action.bindings.Count; i++)
                {
                    var currentBinding = otherAction.action.bindings[i];

                    //Continue if it's not matching current scheme
                    if (!InputManager.Instance.IsBindingMatchingCurrentScheme(currentBinding))
                        continue;

                    var otherBindingFixedPath = InputManager.Instance.FixPath(currentBinding.effectivePath);

                    //If paths are the same, then it's a swap and we need to change the binding
                    if (otherBindingFixedPath == newPath && (otherAction.action != rebindedAction || binidingIndexChanged != i))
                    {
                        //Swapping other action binding
                        SaveRebind(otherAction.action, oldPath, i, playerID);
                        PSEventManager.TriggerEvent(new MMRebindEvent(otherAction, otherBindingFixedPath, oldPath, playerID));
                    }
                }
            }
        }

        /// <summary>
        /// Reset on specific 
        /// </summary>
        /// <param name="playerID"></param>
        public void ResetBindingOverrides(int playerID = 0)
        {
            RebindChanges[playerID].Clear();
            PSEventManager.TriggerEvent(new MMBindingsResetEvent(playerID));
        }

        /// <summary>
        /// Clean all binding overrides for all players.
        /// </summary>
        /// <param name="playerID"></param>
        public void ResetAllBindingOverrides()
        {
            RebindChanges.Clear();
            PSEventManager.TriggerEvent(new MMBindingsResetEvent(-1));
        }

        /// <summary>
        /// Returns the Rebinded changes serialized as JSON
        /// </summary>
        /// <returns>The RebindChanges as .json</returns>
        public string SaveRebindChanges()
        {
            return JsonConvert.SerializeObject(RebindChanges);
        }

        /// <summary>
        /// Load the RebindChanges from a .json string
        /// </summary>
        /// <param name="rebindChanges"></param>
        public void LoadRebindChanges(string rebindChanges)
        {
            RebindChanges = JsonConvert.DeserializeObject<Dictionary<int, List<RebindChange>>>(rebindChanges);
            UpdateInputAssetWithRebindChanges(InputManager.Instance.InputAsset);
        }

        /// <summary>
        /// Update and input action asset with playerIndex rebind changes.
        /// </summary>
        /// <param name="inputAsset">Input action asset to update</param>
        /// <param name="playerID">Default 0 value that refers to the index of RebindChanges that will be applied</param>
        public void UpdateInputAssetWithRebindChanges(InputActionAsset inputAsset, int playerID = 0)
        {
            if (RebindChanges.Count <= 0)
                return;

            if (!RebindChanges.ContainsKey(playerID))
                return;

            var changes = RebindChanges[playerID];

            foreach (var rebindChange in changes)
            {
                ApplyRebindChange(inputAsset, rebindChange);
            }
        }
        
        protected virtual void SetupInputRebind(InputAction action, int playerIndex)
        {
            UpdateInputAssetWithRebindChanges(action.actionMap.asset, playerIndex);

            action.Disable();
        }

        protected virtual void OnBindingCancel(InputActionRebindingExtensions.RebindingOperation operation)
        {
            OnRebindCancel?.Invoke(operation);
            operation.Dispose();
        }

        protected virtual void OnBindingApply(InputActionRebindingExtensions.RebindingOperation operation, string path, int bindinxIndex, int playerIndex)
        {
            var fixedPath = InputManager.Instance.FixPath(path);
            string oldPath = "";

            InputAction action = operation.action;
            InputActionMap actionMap = action.actionMap;
            InputActionAsset asset = actionMap.asset;
            int bindingIndexChanged = -1;

            //Check to avoid bindings that are not allowed
            if (BindingExclusionsServiceProvider.Instance.IsBindingExclude(fixedPath))
            {
                // REBINDING FAILED, excluded
                OnRebindFail?.Invoke(operation, path);
                operation.Dispose();
                return;
            }

            for (int i = 0, j = 0; i < action.bindings.Count; i++)
            {
                if (IsBindingValidForRebind(action.bindings[i]))
                {
                    if (j != bindinxIndex)
                    {
                        ++j;
                        continue;
                    }

                    oldPath = action.bindings[i].effectivePath;
                    bindingIndexChanged = i;
                    SaveRebind(action, fixedPath, i, playerIndex);
                    break;
                }
            }

            PSEventManager.TriggerEvent(new MMRebindEvent(action, oldPath, fixedPath));
            
            OnRebindApply?.Invoke(operation, path, oldPath, bindingIndexChanged, playerIndex);

            operation.Dispose();
        }

        /// <summary>
        /// Check a binding if is valid for rebinding, currently uses the binding mask to decide if it's valid or not to rebind.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        protected virtual bool IsBindingValidForRebind(InputBinding binding)
        {
            return InputManager.Instance.IsBindingMatchingCurrentScheme(binding);
        }

        protected void ApplyBindingOverride(InputAction action, int bindingIndex, string newPath)
        {
            action.ApplyBindingOverride(bindingIndex, newPath);
        }

        protected virtual void OnBindingComplete(InputActionRebindingExtensions.RebindingOperation operation)
        {
            OnRebindComplete?.Invoke(operation);
            operation.action.Enable();
            operation.Dispose();
        }

        private void SaveRebind(InputAction action, string newBindingPath, int bindingIndex, int playerIndex = 0)
        {
            if (RebindChanges.ContainsKey(playerIndex))
            {
                //Search for the player override
                var overrides = RebindChanges[playerIndex];
                
                var bindingOverriding = action.bindings[bindingIndex].path;
                for (int i = 0; i < overrides.Count; i++)
                {
                    //Look for override already created for player
                    if (overrides[i].OriginalBindingPath == bindingOverriding)
                    {
                        var a = overrides[i];
                        a.OverridedBindingPath = newBindingPath;
                        overrides[i] = a;
                        return;
                    }
                }
            }
            else
            {
                RebindChanges.Add(playerIndex, new List<RebindChange>());
            }

            RebindChange newRebindChange = new RebindChange(action, bindingIndex, newBindingPath);
            RebindChanges[playerIndex].Add(newRebindChange);
        }
        
        private void ApplyRebindChange(InputActionAsset inputAsset, RebindChange rebindChange)
        {
            var map = inputAsset.FindActionMap(rebindChange.Map);

            if (map == null)
                return;

            var action = map.FindAction(rebindChange.Action);

            if (action == null)
                return;

            action.ApplyBindingOverride(rebindChange.OverridedBindingPath, InputManager.Instance.GetSchemeName(), rebindChange.OriginalBindingPath);
        }

#if UNITY_EDITOR
        [ContextMenu("List Rebind changes")]
        public void ListRebindInformation()
        {
            foreach (var pair in RebindChanges)
            {
                Debug.Log($"Player:{pair.Key}");
                var changes = pair.Value;
                foreach (var change in changes)
                {
                    Debug.Log($"Change {change.Map}/{change.Action}:{change.OriginalBindingPath} -> {change.OverridedBindingPath}");
                }
            }
        }

        [ContextMenu("Reset all rebinds")]
        public void ClearRebindInformation()
        {
            RebindChanges.Clear();
            PSEventManager.TriggerEvent(new MMBindingsResetEvent());
        }

        protected void OnValidate()
        {
            if(TryGetComponent(out InputManager input))
            {
                if(input.RebindController == null || input.RebindController != this)
                    input.RebindController = this;
            }
        }
#endif
    }
}
