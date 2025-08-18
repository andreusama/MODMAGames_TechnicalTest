using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Framework
{
    /// <summary>
    /// @Author: Alejandro Cortes Cabrejas
    /// Manages match data and state
    /// </summary>
    public interface IGameMode
    {
        public void ExitCurrentMode();

        public void StartCurrentMode(bool fromEditor = false);

        public void LoadCurrentMode<T>(SerializedGameMode<T> data) where T : IGameMode;

        public void LoadInitialScene();
        public void LoadLastScene();

        public void CompleteCurrentMode();

        public Task Save();
        public Task SavePreview();

        public GameModeProgression Progression { get; }

        public int CurrentSaveSlot { get; set; }
        public bool IsNewGame { get; set; }

        public static GameModeData Data { get; }
    }

    public abstract class GameModeProgression
    {
        public CollectionSaveManager BehaviourSaveCollection { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public float CurrentPlayTime { get; protected set; }
        public float SessionStartTime { get; protected set; }

        public float SessionTime => Mathf.Abs(Time.time - SessionStartTime);
        public float CurrentTotalTime => SessionTime + CurrentPlayTime;

        public GameModeProgression()
        {
            BehaviourSaveCollection = new CollectionSaveManager();
            IsCompleted = false;
            CurrentPlayTime = 0f;
            SessionStartTime = 0f;
        }

        public virtual void TrySaveBehaviour<T>(T data) where T : BehaviourPersistentData
        {
            BehaviourSaveCollection.NewRegister(data);
        }

        public virtual bool TryLoadBehaviour<T>(string guid, out T data) where T : BehaviourPersistentData
        {
            return BehaviourSaveCollection.TryGetRegister<T>(guid, out data);
        }
    }
}