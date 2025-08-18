using PetoonsStudio.PSEngine.Utils;
using UnityEngine;

#if UNITY_SWITCH
using UnityEngine.Switch;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
#if UNITY_SWITCH
    public struct PSCPUBoostModeEvent
    {
        public enum Type { NORMAL, FAST }

        public Type EventName;

        public PSCPUBoostModeEvent(Type newName)
        {
            EventName = newName;
        }
    }
#endif

    public class SwitchBoostMode : MonoBehaviour
#if UNITY_SWITCH
        , PSEventListener<PSCPUBoostModeEvent>
#endif
    {
#if UNITY_SWITCH
        private enum CPUBoostModeState
        {
            Disactive,
            Active,
            TemporalyActive
        }

        private Performance.CpuBoostMode m_CurrentCPUMode = Performance.CpuBoostMode.Normal;
        private CPUBoostModeState m_CurrentCPUModeState = CPUBoostModeState.Disactive;

        private float m_MaxCPUBoostModeTime = 0f;
        private float m_CurrentCPUBoostModeTime = 0f;

        void Update()
        {
            if (m_CurrentCPUModeState != CPUBoostModeState.TemporalyActive)
                return;

            UpdateBoostMaxTime();
            if (IsCPUBoostModeMaxTime())
            {
                DeactivateCPUBoostMode();
            }
        }

        private void OnEnable()
        {
            PSEventManager.AddListener(this);
        }

        private void OnDisable()
        {
            PSEventManager.RemoveListener(this);
        }

        public void Initialize(float maxModeTime)
        {
            m_MaxCPUBoostModeTime = maxModeTime;
        }

        public void OnPSEvent(PSCPUBoostModeEvent eventType)
        {
            switch (eventType.EventName)
            {
                case PSCPUBoostModeEvent.Type.NORMAL:
                    SetCpuBoostNormalMode();
                    break;
                case PSCPUBoostModeEvent.Type.FAST:
                    SetCpuBoostFastMode();
                    break;
                default:
                    SetCpuBoostNormalMode();
                    break;
            }
        }

        private void DeactivateCPUBoostMode()
        {
            SetCpuBoostNormalMode();
            m_CurrentCPUBoostModeTime = 0f;
        }

        private bool IsCPUBoostModeMaxTime()
        {
            return m_CurrentCPUBoostModeTime >= m_MaxCPUBoostModeTime;
        }

        private void UpdateBoostMaxTime()
        {
            m_CurrentCPUBoostModeTime += UnityEngine.Time.deltaTime;
        }

        private void SetCpuBoostNormalMode()
        {
            if (m_CurrentCPUMode == Performance.CpuBoostMode.Normal)
                return;

            m_CurrentCPUMode = Performance.CpuBoostMode.Normal;
            m_CurrentCPUModeState = CPUBoostModeState.Disactive;
            Performance.SetCpuBoostMode(m_CurrentCPUMode);
        }

        private void SetCpuBoostFastMode()
        {
            if (m_CurrentCPUMode == Performance.CpuBoostMode.FastLoad)
                return;

            m_CurrentCPUMode = Performance.CpuBoostMode.FastLoad;
            Performance.SetCpuBoostMode(m_CurrentCPUMode);

            if (m_MaxCPUBoostModeTime > 0f)
            {
                m_CurrentCPUModeState = CPUBoostModeState.TemporalyActive;
            }
            else 
            {
                m_CurrentCPUModeState = CPUBoostModeState.Active;
            }
        }
#endif
    }
}


