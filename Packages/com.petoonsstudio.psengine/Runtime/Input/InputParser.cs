using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input
{
    public static class InputParser
    {
        public class ParsedAction
        {
            public InputAction Action;
            public int BindingIndex;
            public string ToReplace;

            public ParsedAction(InputAction action, int bidingIndex)
            {
                Action = action;
                BindingIndex = 0;
            }

            public ParsedAction(InputAction action, string toReplace)
            {
                Action = action;
                ToReplace = toReplace;
                BindingIndex = 0;
            }

            public ParsedAction(InputAction action, string toReplace, int bidingIndex)
            {
                Action = action;
                ToReplace = toReplace;
                BindingIndex = bidingIndex;
            }
        }

        public static InputAction ParseSingle(InputActionAsset actionAsset, string indicator)
        {
            string inputActionMap, inputAction;
            {
                string[] composed = indicator.Split('/');
                if (composed.Length < 2)
                {
                    Debug.LogWarning("InputAction provided is badly formatted");
                    return null;
                }

                inputActionMap = composed[0];
                inputAction = composed[1];
            }

            InputActionMap map = actionAsset.FindActionMap(inputActionMap);
            if (map == null) return null;

            InputAction action = map.FindAction(inputAction);
            return action;
        }

        public static List<ParsedAction> ParseText(InputActionAsset actionAsset, string text, string indicator)
        {
            List<ParsedAction> parsedActions = new List<ParsedAction>();

            while (text.Contains(indicator))
            {
                int startIndex = text.IndexOf(indicator);
                int endIndex = text.IndexOf(indicator, startIndex + 1);

                if (endIndex == -1) return parsedActions;

                string indicatorInstance = text.Substring(startIndex + 1, endIndex - startIndex - 1);

                int bindingIndex = 0;
                if (indicator.Contains("#"))
                {
                    var preCompose = indicator.Split("#");
                    indicatorInstance = preCompose[0];
                    var bindingIndexString = preCompose[1];
                    if (int.TryParse(bindingIndexString, out bindingIndex)) { }
                }

                InputAction action = ParseSingle(actionAsset, indicatorInstance);
                if (action != null)
                {
                    string toReplace = text.Substring(startIndex, endIndex - startIndex + 1);
                    ParsedAction parsedAction = new ParsedAction(action, toReplace, bindingIndex);
                    parsedActions.Add(parsedAction);
                    text = text.Replace(toReplace, "");
                }
                else
                {
                    text = text.Remove(startIndex, indicator.Length);
                }
            }

            return parsedActions;
        }


        private static string EmptyText(int length) => new string(' ', length);
    }
}
