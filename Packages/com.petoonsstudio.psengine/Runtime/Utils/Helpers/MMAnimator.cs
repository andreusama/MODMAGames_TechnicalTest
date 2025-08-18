using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// Various static methods used throughout the Infinite Runner Engine and the Corgi Engine.
    /// </summary>

    public static class MMAnimator
    {
        // Adds an animator parameter name to a parameter list if that parameter exists.
        public static void AddAnimatorParamaterIfExists(Animator animator, string parameterName, AnimatorControllerParameterType type, List<string> parameterList)
        {
            if (animator.HasParameterOfType(parameterName, type))
            {
                parameterList.Add(parameterName);
            }
        }

        /// <summary>
        /// Get octopus fall clip
        /// </summary>
        /// <returns></returns>
        public static float GetTimeClip(Animator animator, string clip)
        {
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;    //Get Animator controller

            for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
            {
                if (ac.animationClips[i].name == clip)        //If it has the same name as your clip
                {
                    return ac.animationClips[i].length;
                }
            }

            return 0f;
        }

        /// <summary>
        /// Get clip name
        /// </summary>
        /// <param name="animator"></param>
        /// <returns></returns>
        public static AnimationClip GetClip(Animator animator, int layerIndex = 0)
        {
            int w = animator.GetCurrentAnimatorClipInfo(layerIndex).Length;

            AnimationClip[] clipName = new AnimationClip[w];
            for (int i = 0; i < w; i += 1)
            {
                clipName[i] = animator.GetCurrentAnimatorClipInfo(layerIndex)[i].clip;
            }

            if (clipName.Length > 0)
            {
                return clipName[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Is the animator playing requested animator state
        /// </summary>
        public static bool IsAnimationPlaying(Animator animator, int layerIndex, string tag, float maxNormalizedTime = 1.0f)
        {
            var value = animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag(tag)
                && (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < maxNormalizedTime || maxNormalizedTime < 0);
            return value;
        }

        // <summary>
        /// Updates the animator bool.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetBool(parameterName, value);
            }
        }

        public static void UpdateAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Triggers an animator trigger.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTrigger(Animator animator, string parameterName, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator integer.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value, List<string> parameterList)
        {
            if (parameterList.Contains(parameterName))
            {
                animator.SetInteger(parameterName, value);
            }
        }


        // <summary>
        /// Updates the animator bool without checking the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBool(Animator animator, string parameterName, bool value)
        {
            animator.SetBool(parameterName, value);
        }

        /// <summary>
        /// Updates the animator trigger without checking the parameter's existence
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        public static void UpdateAnimatorTrigger(Animator animator, string parameterName)
        {
            animator.SetTrigger(parameterName);
        }

        /// <summary>
        /// Triggers an animator trigger without checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTrigger(Animator animator, string parameterName)
        {
            animator.SetTrigger(parameterName);
        }

        /// <summary>
        /// Resets an animator trigger without checking for the parameter's existence
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="parameterName"></param>
        public static void ResetAnimatorTrigger(Animator animator, string parameterName)
        {
            animator.ResetTrigger(parameterName);
        }

        /// <summary>
        /// Updates the animator float without checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloat(Animator animator, string parameterName, float value)
        {
            animator.SetFloat(parameterName, value);
        }

        /// <summary>
        /// Updates the animator integer without checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorInteger(Animator animator, string parameterName, int value)
        {
            animator.SetInteger(parameterName, value);
        }

        // <summary>
        /// Updates the animator bool after checking the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorBoolIfExists(Animator animator, string parameterName, bool value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
            {
                animator.SetBool(parameterName, value);
            }
        }

        // <summary>
        /// Updates the animator trigger after checking the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void UpdateAnimatorTriggerIfExists(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Resets an animator trigger after checking for the parameter's existence
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="parameterName"></param>
        public static void ResetAnimatorTriggerifExist(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.ResetTrigger(parameterName);
            }   
        }

        /// <summary>
        /// Triggers an animator trigger after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetAnimatorTriggerIfExists(Animator animator, string parameterName)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
            {
                animator.SetTrigger(parameterName);
            }
        }

        /// <summary>
        /// Updates the animator float after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloatIfExists(Animator animator, string parameterName, float value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value);
            }
        }

        /// <summary>
        /// Updates the animator float after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorFloatIfExists(Animator animator, string parameterName, float value, float dampTime, float deltaTime)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
            {
                animator.SetFloat(parameterName, value, dampTime, deltaTime);
            }
        }

        /// <summary>
        /// Updates the animator integer after checking for the parameter's existence.
        /// </summary>
        /// <param name="animator">Animator.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Value.</param>
        public static void UpdateAnimatorIntegerIfExists(Animator animator, string parameterName, int value)
        {
            if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
            {
                animator.SetInteger(parameterName, value);
            }
        }
    }
}
