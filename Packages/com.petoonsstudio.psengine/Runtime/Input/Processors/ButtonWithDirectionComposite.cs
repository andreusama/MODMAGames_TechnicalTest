using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.Layouts;

namespace PetoonsStudio.PSEngine.Input
{
    // Use InputBindingComposite<TValue> as a base class for a composite that returns
    // values of type TValue.
    // NOTE: It is possible to define a composite that returns different kinds of values
    //       but doing so requires deriving directly from InputBindingComposite.
#if UNITY_EDITOR
    [InitializeOnLoad] // Automatically register in editor.
#endif
    // Determine how GetBindingDisplayString() formats the composite by applying
    // the  DisplayStringFormat attribute.
    [DisplayStringFormat("{firstPart}+{secondPart}")]
    public class ButtonWithDirectionComposite : InputBindingComposite<float>
    {
        public enum Directions
        {
            BackDown, Down, ForwardDown, Back, Stand, Forward, BackUp, Up, ForwardUp
        }

        // Each part binding is represented as a field of type int and annotated with
        // InputControlAttribute. Setting "layout" restricts the controls that
        // are made available for picking in the UI.
        //
        // On creation, the int value is set to an integer identifier for the binding
        // part. This identifier can read values from InputBindingCompositeContext.
        // See ReadValue() below.
        [InputControl(layout = "Action")]
        public int modifier;

        [InputControl(layout = "Button")]
        public int input;

        public Directions EnableDirection;
        [Range(0f, 1f)]
        public float DeadZoneValue = 0.4f;

        // This method computes the resulting input value of the composite based
        // on the input from its part bindings.
        public override float ReadValue(ref InputBindingCompositeContext context)
        {
            var modifierValue = context.ReadValue<Vector2, Vector2MagnitudeComparer>(modifier);
            var inputValue = context.ReadValue<float>(input);

            if (inputValue > 0)
            {
                bool condition = ConditionDone(modifierValue);

                if (!condition)
                {
                    inputValue = 0f;
                }
            }

            return inputValue;
        }

        /// <summary>
        /// Check direction condition
        /// </summary>
        /// <param name="directionValue"></param>
        /// <returns></returns>
        public bool ConditionDone(Vector2 directionValue)
        {
            switch (EnableDirection)
            {
                case Directions.BackDown:
                    return (directionValue.x < -DeadZoneValue && directionValue.y < -DeadZoneValue);
                case Directions.Down:
                    return (directionValue.x > -DeadZoneValue && directionValue.x < DeadZoneValue && directionValue.y < -DeadZoneValue);
                case Directions.ForwardDown:
                    return (directionValue.x > DeadZoneValue && directionValue.y < -DeadZoneValue);
                case Directions.Back:
                    return (directionValue.x < -DeadZoneValue && directionValue.y > -DeadZoneValue && directionValue.y < DeadZoneValue);
                case Directions.Stand:
                    return (directionValue.x > DeadZoneValue && directionValue.x < DeadZoneValue && directionValue.y > -DeadZoneValue && directionValue.y < DeadZoneValue);
                case Directions.Forward:
                    return (directionValue.x > DeadZoneValue && directionValue.y > -DeadZoneValue && directionValue.y < DeadZoneValue);
                case Directions.BackUp:
                    return (directionValue.x < -DeadZoneValue && directionValue.y > DeadZoneValue);
                case Directions.Up:
                    return (directionValue.x > -DeadZoneValue && directionValue.x < DeadZoneValue && directionValue.y > DeadZoneValue);
                case Directions.ForwardUp:
                    return (directionValue.x > DeadZoneValue && directionValue.y > DeadZoneValue);
                default:
                    return false;
            }
        }

        // This method computes the current actuation of the binding as a whole.
        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            return ReadValue(ref context);
        }

        static ButtonWithDirectionComposite()
        {
            // Can give custom name or use default (type name with "Composite" clipped off).
            // Same composite can be registered multiple times with different names to introduce
            // aliases.
            //
            // NOTE: Registering from the static constructor using InitializeOnLoad and
            //       RuntimeInitializeOnLoadMethod is only one way. You can register the
            //       composite from wherever it works best for you. Note, however, that
            //       the registration has to take place before the composite is first used
            //       in a binding. Also, for the composite to show in the editor, it has
            //       to be registered from code that runs in edit mode.
            InputSystem.RegisterBindingComposite<ButtonWithDirectionComposite>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init() { } // Trigger static constructor.
    }
}
