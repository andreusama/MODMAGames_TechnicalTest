using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PetoonsStudio.PSEngine.Framework
{
    /// <summary>
    /// @Author: Alejandro Cortes Cabrejas
    /// Handles generic game data and state
    /// </summary>
    public class GameManager : PersistentSingleton<GameManager>
    {
        public IGameMode CurrentGameMode { get; set; }
        public AssetReferenceT<SceneGroup> MenuScene;

        public virtual T GetCurrentGameMode<T>() where T : IGameMode
        {
            if (CurrentGameMode == null)
                return default;

            if (IsCurrentGameMode(typeof(T)))
                return (T)CurrentGameMode;
            else
                return default;
        }

        public virtual bool IsCurrentGameMode(Type type)
        {
            if (CurrentGameMode == null)
                return false;

            return type == CurrentGameMode.GetType();
        }

        public virtual void NewGameMode<T>() where T : IGameMode
        {
            CurrentGameMode = Activator.CreateInstance<T>();
            CurrentGameMode.StartCurrentMode();
        }

        public virtual void LoadGameMode<T>(SerializedGameMode<T> data) where T : IGameMode
        {
            CurrentGameMode = Activator.CreateInstance<T>();
            CurrentGameMode.LoadCurrentMode(data);
        }

        public virtual void ExitCurrentGameMode()
        {
            CurrentGameMode.ExitCurrentMode();
            CurrentGameMode = null;

            GC.Collect();
        }

        public virtual void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}