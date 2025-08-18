using UnityEngine;
using Cinemachine;

namespace PetoonsStudio.PSEngine.Utils
{
    /// <summary>
    /// An add-on module for Cinemachine Virtual Camera that locks the camera's Y co-ordinate
    /// </summary>
    [ExecuteInEditMode]
    [SaveDuringPlay]
    [AddComponentMenu("")] // Hide in menu
    public class LockCameraAxis : CinemachineExtension
    {
        [Tooltip("Axis to be locked")]
        public Axis LockAxis = Axis.Z;

        [Tooltip("Lock by this value")]
        public float Value = 10;

        public enum Axis { X, Y, Z }

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (enabled && stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;

                switch (LockAxis)
                {
                    case Axis.X:
                        pos.x = Value;
                        break;
                    case Axis.Y:
                        pos.y = Value;
                        break;
                    case Axis.Z:
                        pos.z = Value;
                        break;
                }

                state.RawPosition = pos;
            }
        }
    }
}