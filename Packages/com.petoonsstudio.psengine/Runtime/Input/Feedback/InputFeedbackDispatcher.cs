using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    [RequireComponent(typeof(InputFeedbackHUB))]
    public class InputFeedbackDispatcher : PersistentSingleton<InputFeedbackDispatcher>
    {
        public bool ExecuteCommands = false;

        protected SerializedDictionary<int, InputFeedbackCommand> CommandsQueue = new();

        protected const int MILISECONDS_PER_TICK = 50;
        protected float m_CurrentDelay = 0f;

        protected virtual void OnEnable()
        {
            ExecuteCommands = true;
            ExecuteCommandsAsync();
        }
        protected virtual void OnDisable()
        {
            ExecuteCommands = false;
        }

        public virtual void RequestCommand(InputFeedbackCommand task, DeviceFeedbackData data)
        {
            if (!CommandsQueue.ContainsKey(data.DeviceId))
                CommandsQueue.Add(data.DeviceId, task);
            else
                CommandsQueue[data.DeviceId] = task;
        }
        protected virtual async void ExecuteCommandsAsync()
        {
            while (ExecuteCommands)
            {
                var temp = new Dictionary<int, InputFeedbackCommand>(CommandsQueue);
                CommandsQueue.Clear();
                foreach (var commandRequest in temp)
                {
                    await RunCommand(commandRequest.Value);
                }

                await Task.Yield();
            }
        }
        protected virtual async Task RunCommand(InputFeedbackCommand task)
        {
            DeviceFeedbackData DeviceData = task.Data;

            if (DeviceData.CurrentTask != null)
            {
                switch (DeviceData.CurrentTask)
                {
                    case InputFeedbackAsyncCommand asyncCommand:
                        await asyncCommand.Stop();
                        break;
                }
            }

            task.Start();
        }

        #region VIBRATION
        public virtual void PlayAudioVibration(AudioVibrationParams vibrationParams)
        {
            for (int i = 0; i < InputFeedbackHUB.GAMEPADS_MAX_SLOTS && i < PlayerInput.all.Count; i++)
            {
                PlayUserAudioVibration(i, vibrationParams);
            }
        }
        public virtual void PlayUserAudioVibration(int userIndex, AudioVibrationParams vibrationParams)
        {
            foreach (var device in GetDevices(userIndex))
            {
                if (!InputFeedbackHUB.Instance.AudioVibrationSupported(device)) continue;

                var data = InputFeedbackHUB.Instance.GetData<DeviceFeedbackData>(device.deviceId);

                var task = new PlayVibrationCommand(data, userIndex, vibrationParams);
                RequestCommand(task, data);
            }
        }

        public virtual void SetDeviceVibration(int deviceId, VibrationParams vibrationData)
        {
            if (!InputFeedbackHUB.Instance.VibrationSupported(InputFeedbackHUB.Instance.GetDevice(deviceId))) return;

            var data = InputFeedbackHUB.Instance.GetData<DeviceFeedbackData>(deviceId);
            var task = new SetVibrationCommand(data, vibrationData);
            RequestCommand(task, data);
        }

        public virtual void SetDevicesVibration(VibrationParams vibrationData)
        {
            foreach (var device in InputSystem.devices)
            {
                SetDeviceVibration(device.deviceId, vibrationData);
            }
        }
        public virtual void SetUserVibration(int userIndex, VibrationParams vibrationData)
        {
            foreach (var device in GetDevices(userIndex))
            {
                SetDeviceVibration(device.deviceId, vibrationData);
            }
        }
        #endregion

        #region COLOR
        public virtual void SetDeviceColor(int deviceId, Color color)
        {
            if (!InputFeedbackHUB.Instance.ColorSupported(InputFeedbackHUB.Instance.GetDevice(deviceId))) return;

            var data = InputFeedbackHUB.Instance.GetData<DeviceFeedbackData>(deviceId);
            var task = new SetColorCommand(data, color);
            RequestCommand(task, data);
        }
        public virtual void SetDevicesColor(Color color)
        {
            foreach (var device in InputSystem.devices)
            {
                SetDeviceColor(device.deviceId, color);
            }
        }
        public virtual void SetUserColor(int userIndex, Color color)
        {
            foreach (var device in GetDevices(userIndex))
            {
                SetDeviceColor(device.deviceId, color);
            }
        }

        public virtual void BlendDeviceColor(int deviceId, Color newColor, float blendDuration = 1f)
        {
            if (!InputFeedbackHUB.Instance.ColorSupported(InputFeedbackHUB.Instance.GetDevice(deviceId))) return;
            
            var data = InputFeedbackHUB.Instance.GetData<DeviceFeedbackData>(deviceId);
            var task = new BlendColorCommand(data, newColor, blendDuration);
            RequestCommand(task, data);
        }
        public virtual void BlendDevicesColor(Color newColor, float blendDuration = 1f)
        {
            foreach (var device in InputSystem.devices)
            {
                BlendDeviceColor(device.deviceId, newColor, blendDuration);
            }
        }
        public virtual void BlendUserColor(int userIndex, Color color, float blendDuration = 1f)
        {
            foreach (var device in GetDevices(userIndex))
            {
                BlendDeviceColor(device.deviceId, color, blendDuration);
            }
        }

        public virtual void BreathDeviceColor(int deviceId, List<Color> colors, float blendDuration = 1f, float breathDelay = 0f, float duration = -1)
        {
            if (!InputFeedbackHUB.Instance.ColorSupported(InputFeedbackHUB.Instance.GetDevice(deviceId))) return;
            
            var data = InputFeedbackHUB.Instance.GetData<DeviceFeedbackData>(deviceId);
            var task = new BreathColorsCommand(data, colors, blendDuration, breathDelay, duration);
            RequestCommand(task, data);
        }
        public virtual void BreathDevicesColor(List<Color> colors, float blendDuration = 1f, float breathDelay = 0f, float duration = -1)
        {
            foreach (var device in InputSystem.devices)
            {
                BreathDeviceColor(device.deviceId, colors, blendDuration, breathDelay, duration);
            }
        }
        public virtual void BreathUserColors(int userIndex, List<Color> colors, float blendDuration = 1f, float breathDelay = 0f, float duration = -1)
        {
            if (userIndex >= PlayerInput.all.Count || userIndex < 0)
            {
                Debug.LogError($"Requested breath colors for userIndex {userIndex} when the total number of players is {PlayerInput.all.Count}");
                return;
            }

            PlayerInput playerInput = PlayerInput.all[userIndex];

            foreach (var device in playerInput.devices)
            {
                BreathDeviceColor(device.deviceId, colors, blendDuration, breathDelay, duration);
            }
        }
        #endregion

        protected virtual ReadOnlyArray<InputDevice> GetDevices(int userIndex)
        {
            if (userIndex >= PlayerInput.all.Count || userIndex < 0)
            {
                Debug.LogError($"Requested userIndex {userIndex} when the total number of players is {PlayerInput.all.Count}");
                return new ReadOnlyArray<InputDevice>();
            }

            PlayerInput playerInput = PlayerInput.all[userIndex];

            return playerInput.devices;
        }
    }
}
