using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class AnimatorExtensions
    {
        /// <summary>
        /// Reset animation parameters
        /// </summary>
        /// <param name="animator"></param>
        public static void ResetParameters(this Animator animator)
        {
            AnimatorControllerParameter[] parameters = animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];
                switch (parameter.type)
                {
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(parameter.name, parameter.defaultInt);
                        break;
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(parameter.name, parameter.defaultFloat);
                        break;
                    case AnimatorControllerParameterType.Bool:
                        animator.SetBool(parameter.name, parameter.defaultBool);
                        break;
                }
            }
        }

        /// <summary>
        /// Determines if an animator contains a certain parameter, based on a type and a name
        /// </summary>
        /// <returns><c>true</c> if has parameter of type the specified self name type; otherwise, <c>false</c>.</returns>
        /// <param name="self">Self.</param>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
        {
            if (name == null || name == "" || self == null) { return false; }
            AnimatorControllerParameter[] parameters = self.parameters;
            foreach (AnimatorControllerParameter currParam in parameters)
            {
                if (currParam.type == type && currParam.name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public static void OverrideControllerWithAnimation(this Animator animator, RuntimeAnimatorController emptyController, AnimationClip animationClip, out RuntimeAnimatorController originalController)
        {
            AnimatorOverrideController overrider = new AnimatorOverrideController(emptyController);
            var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            foreach (var anim in overrider.animationClips)
            {
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(anim, animationClip));
            }
            overrider.ApplyOverrides(anims);

            originalController = animator.runtimeAnimatorController;
            animator.runtimeAnimatorController = overrider;

            animator.Rebind();
        }

        public static IEnumerator PlayAnimation(this Animator animator, AnimationClip animationClip)
        {
            AnimationPlayableUtilities.PlayClip(animator, animationClip, out PlayableGraph graph);
            graph.GetRootPlayable(0).SetDuration(animationClip.length);
            yield return new WaitUntil(() => graph.GetRootPlayable(0).IsDone());
            graph.Destroy();
        }
    }
}

