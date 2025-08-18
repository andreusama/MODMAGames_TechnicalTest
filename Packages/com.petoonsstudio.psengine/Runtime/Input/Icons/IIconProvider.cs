using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input
{
    public interface IIconProvider
    {
        TMP_SpriteAsset IconTextSprites { get; }

        void Initialize();

        Sprite GetSprite(string mapType, string controlPath);
    }
}