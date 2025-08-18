using UnityEngine;

namespace PetoonsStudio.PSEngine.Interaction
{
    [RequireComponent(typeof(Collider2D))]
    abstract public class Interactable2D : Interactable<Collider2D>
    {
    }
}
