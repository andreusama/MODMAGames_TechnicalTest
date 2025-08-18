using MoreMountains.Tools;

namespace PetoonsStudio.PSEngine.Framework
{
    [System.Serializable]
    public class SerializableSoundManagerSettings
    {
        public bool OverrideMixerSettings;
        public float MasterVolume;
        public bool MasterOn;
        public float MutedMasterVolume;
        public float MusicVolume;
        public bool MusicOn;
        public float MutedMusicVolume;
        public float SfxVolume;
        public bool SfxOn;
        public float MutedSfxVolume;
        public float UIVolume;
        public bool UIOn;
        public float MutedUIVolume;
        public float VoicesVolume;
        public bool VoicesOn;
        public float MutedVoicesVolume;

        public SerializableSoundManagerSettings()
        {

        }

        public SerializableSoundManagerSettings(MMSoundManagerSettings settings)
        {
            OverrideMixerSettings = settings.OverrideMixerSettings;
            MasterVolume = settings.MasterVolume;
            MasterOn = settings.MasterOn;
            MutedMasterVolume = settings.MutedMasterVolume;
            MusicVolume = settings.MusicVolume;
            MusicOn = settings.MusicOn;
            MutedMusicVolume = settings.MutedMusicVolume;
            SfxVolume = settings.SfxVolume;
            SfxOn = settings.SfxOn;
            MutedSfxVolume = settings.MutedSfxVolume;
            UIVolume = settings.UIVolume;
            UIOn = settings.UIOn;
            MutedUIVolume = settings.MutedUIVolume;
            VoicesVolume = settings.VoicesVolume;
            VoicesOn = settings.VoicesOn;
            MutedVoicesVolume = settings.MutedVoicesVolume;
        }

        public virtual void Deserialize(MMSoundManagerSettings settings)
        {
            settings.OverrideMixerSettings = OverrideMixerSettings;
            settings.MasterVolume = MasterVolume;
            settings.MasterOn = MasterOn;
            settings.MutedMasterVolume = MutedMasterVolume;
            settings.MusicVolume = MusicVolume;
            settings.MusicOn = MusicOn;
            settings.MutedMusicVolume = MutedMusicVolume;
            settings.SfxVolume = SfxVolume;
            settings.SfxOn = SfxOn;
            settings.MutedSfxVolume = MutedSfxVolume;
            settings.UIVolume = UIVolume;
            settings.UIOn = UIOn;
            settings.MutedUIVolume = MutedUIVolume;
            settings.VoicesVolume = VoicesVolume;
            settings.VoicesOn = VoicesOn;
            settings.MutedVoicesVolume = MutedVoicesVolume;
        }
    }
}