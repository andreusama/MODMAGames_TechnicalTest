using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public class DirectionProcessor : InputProcessor<Vector2>
    {
        public enum Directions
        {
            BackDown, Down, ForwardDown, Back, Stand, Forward, BackUp, Up, ForwardUp
        }

        public Directions Direction;

#if UNITY_EDITOR
        static DirectionProcessor()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterProcessor<DirectionProcessor>();
        }

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            switch (Direction)
            {
                case Directions.BackDown:
                    if(value.x < 0f && value.y < 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.Down:
                    if (value.x == 0f && value.y < 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.ForwardDown:
                    if (value.x > 0f && value.y < 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.Back:
                    if (value.x < 0f && value.y == 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.Stand:
                    if (value.x == 0f && value.y == 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.Forward:
                    if (value.x > 0f && value.y == 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.BackUp:
                    if (value.x < 0f && value.y > 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.Up:
                    if (value.x == 0f && value.y > 0f)
                    {
                        return value;
                    }
                    break;
                case Directions.ForwardUp:
                    if (value.x > 0f && value.y > 0f)
                    {
                        return value;
                    }
                    break;
            }

            return Vector2.zero;
        }
    }
}

