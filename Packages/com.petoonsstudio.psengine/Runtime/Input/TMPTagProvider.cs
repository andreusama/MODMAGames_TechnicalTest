using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using static PetoonsStudio.PSEngine.Input.InputParser;

namespace PetoonsStudio.PSEngine.Input
{
    public class TMPTagProvider
    {
        /// <summary>
        /// Struct to hold information about bindings.
        /// By default Unity doesn't hold the partComposite bindings as child of the composite binding.
        /// This struct holds composite bindings in a parent/child structure.
        /// </summary>
        public struct BindingStruct
        {
            public InputBinding Binding;
            public bool IsComposite;
            public List<InputBinding> CompositeParts;

            public BindingStruct(InputBinding binding)
            {
                Binding = binding;
                IsComposite = false;
                CompositeParts = new List<InputBinding>();
            }

            public BindingStruct(InputBinding binding, bool isComposite)
            {
                Binding = binding;
                IsComposite = isComposite;
                CompositeParts = new List<InputBinding>();
            }
        }

        public static string Separator = "/";

        /// <summary>
        /// Replace parsed actions with an sprite if exist or text by default
        /// </summary>
        /// <param name="parsedActions"></param>
        /// <param name="text"></param>
        /// <param name="spriteAsset"></param>
        /// <returns></returns>
        public static string ReplaceActions(List<ParsedAction> parsedActions, string text, TMP_SpriteAsset spriteAsset)
        {
            foreach (var action in parsedActions)
            {
                BindingStruct parsedBinding = GetBindingFromParsedAction(action);

                if (parsedBinding.IsComposite)
                {
                    var compositeBindingControlPath = InputManager.GetCompositeBindingControlPath(parsedBinding);
                    if(spriteAsset.GetSpriteIndexFromName(compositeBindingControlPath) != -1)
                    {
                        text = ReplaceActionWithSpriteTag(action, compositeBindingControlPath, text);
                        continue;
                    }

                    text = ExistPartCompositesSprites(parsedBinding, spriteAsset) ?
                        ReplaceActionWithCompositeSpriteTags(action, parsedBinding, text) :
                        ReplaceActionWithControlsPath(action, parsedBinding, text);
                }
                else
                {
                    var bindingPath = InputManager.GetBindingControlPath(parsedBinding.Binding);
                    if (spriteAsset.GetSpriteIndexFromName(bindingPath) != -1)
                    {
                        text = ReplaceActionWithSpriteTag(action, bindingPath, text);
                    }
                    else
                    {
                        text = ReplaceActionWithControlsPath(action, parsedBinding, text);
                    }
                }
            }
            return text;
        }

        /// <summary>
        /// Replace an action with the Sprite image corresponding to the Binding path
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingPath"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ReplaceActionWithSpriteTag(ParsedAction action, string bindingPath, string text)
        {
            var tagText = GetSpriteImageNonCompositeTag(bindingPath);
            return text.Replace(action.ToReplace, tagText);
        }

        /// <summary>
        /// Replace an action with a Sprite for each Composite binding of the action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingStruct"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ReplaceActionWithCompositeSpriteTags(ParsedAction action, BindingStruct bindingStruct, string text)
        {
            var tags = GetSpriteImageCompositeTags(bindingStruct);
            var tag = ParseCompositeTags(tags);
            return text.Replace(action.ToReplace, tag);
        }
        
        /// <summary>
        /// Replace an action with the exact name of the binding control path.
        /// Example: <gamepad>/buttonSouth -> BUTTONSOUTH
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingStruct"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ReplaceActionWithControlsPath(ParsedAction action, BindingStruct bindingStruct, string text)
        {
            var bindingControlPath = GetControlPath(bindingStruct).ToUpper();

            return text.Replace(action.ToReplace, bindingControlPath);
        }

        /// <summary>
        /// Return if exist an sprite for each partComposite binding of a binding
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="spriteAsset"></param>
        /// <returns></returns>
        public static bool ExistPartCompositesSprites(BindingStruct binding, TMP_SpriteAsset spriteAsset)
        {
            bool existIndividualSprites = true;
            foreach (var compositePart in binding.CompositeParts)
            {
                var bindingPath = InputManager.GetBindingControlPath(compositePart);
                if (spriteAsset.GetSpriteIndexFromName(bindingPath) == -1)
                    existIndividualSprites = false;
            }
            return existIndividualSprites;
        }

        /// <summary>
        /// Return string with control tag
        /// </summary>
        /// <param name="device"></param>
        /// <param name="controlPath"></param>
        /// <returns></returns>
        public static string GetSpriteImageNonCompositeTag(string controlPath)
        {
            return $"<sprite name=\"{controlPath}\">";
        }

        /// <summary>
        /// Return array of strings with tags of controls (keyname if keyboard, sprite name if controller)
        /// </summary>
        /// <param name="device"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        public static string[] GetSpriteImageCompositeTags(BindingStruct cs)
        {
            string[] tags = new string[cs.CompositeParts.Count];
            int counter = 0;
            foreach (var control in cs.CompositeParts)
            {
                string path = control.effectivePath.Substring(control.effectivePath.IndexOf('/') + 1);
                tags[counter] = $"<sprite name=\"{path}\">";
                
                counter++;
            }
            return tags;
        }

        /// <summary>
        /// Returns Control Path from binding wheter is composite or not
        /// </summary>
        /// <param name="bindingStruct"></param>
        /// <returns></returns>
        public static string GetControlPath(BindingStruct bindingStruct)
        {
            if (bindingStruct.IsComposite)
            {
                string[] tags = InputManager.GetCompositeBindingControlPaths(bindingStruct);
                return ParseCompositeTags(tags);
            }
            else
            {
                return InputManager.GetBindingControlPath(bindingStruct.Binding);
            }
        }

        /// <summary>
        /// Parse array of tags to show them, base only puts an space between tags
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static string ParseCompositeTags(string[] tags)
        {
            string tag = "";
            for (int i = 0; i < tags.Length; i++)
            {
                tag += tags[i];
                if (i < tags.Length - 1)
                    tag += Separator;
            }
            return tag;
        }

        /// <summary>
        /// Return the list of the BindingStructs filtered by scheme of an action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="scheme"></param>
        /// <returns></returns>
        public static List<BindingStruct> GetSchemeBindings(InputAction action, string scheme)
        {
            List<BindingStruct> bindings = new List<BindingStruct>();
            for (int i = 0; i < action.bindings.Count; i++)
            {
                if (!action.bindings[i].isComposite && !action.bindings[i].isPartOfComposite && InputManager.Instance.IsBindingMatchingCurrentScheme(action.bindings[i]))
                {
                    bindings.Add(new BindingStruct(action.bindings[i]));
                }
                else if (action.bindings[i].isComposite)
                {
                    if ((i + 1 > action.bindings.Count - 1) || !InputManager.Instance.IsBindingMatchingCurrentScheme(action.bindings[i + 1]))
                    {
                        continue;
                    }

                    BindingStruct compositeBinding = new BindingStruct(action.bindings[i], true);
                    
                    var nextBinding = i + 1;
                    while ((nextBinding > action.bindings.Count - 1) || action.bindings[nextBinding].isPartOfComposite)
                    {
                        compositeBinding.CompositeParts.Add(action.bindings[nextBinding]);
                        nextBinding++;
                    }

                    bindings.Add(compositeBinding);
                    i = nextBinding;
                }
            }
            return bindings;
        }

        /// <summary>
        /// Return the selected by index binding of a Parsed Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static BindingStruct GetBindingFromParsedAction(ParsedAction action)
        {
            var schemeBindings = GetSchemeBindings(action.Action, InputManager.Instance.GetSchemeName());
            int bindingIndex = 0;

            if (action.BindingIndex <= schemeBindings.Count - 1)
            {
                bindingIndex = action.BindingIndex;
            }
            return schemeBindings[bindingIndex];
        }
    }
}