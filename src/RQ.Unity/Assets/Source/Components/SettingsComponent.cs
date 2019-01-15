using RQ.Messaging;
using RQ.Model.Serialization;
using UnityEngine;

namespace RQ.Controller.ManageScene
{
    [AddComponentMenu("RQ/Components/Settings Component")]
    public class SettingsComponent : MessagingObject
    {
        public string SaveMessage;
        public string CancelMessage;
        public UISlider MusicVolumeSlider;
        public UISlider SoundEffectVolumeSlider;

        // Need to reset the prefs if Cancel is clicked.
        private GamePrefsData _originalGamePrefsData;

        public override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;
            _originalGamePrefsData = GameStateController.Instance.GetGamePrefs().Clone();
            MusicVolumeSlider.value = _originalGamePrefsData.MusicVolume;
            SoundEffectVolumeSlider.value = _originalGamePrefsData.SoundEffectVolume;
        }

        public void SetMusicVolume()
        {
            if (UIProgressBar.current == null)
                return;
            GameStateController.Instance.GetGamePrefs().MusicVolume = UIProgressBar.current.value;
            MessageDispatcher2.Instance.DispatchMsg("UpdateVolume", 0f, this.UniqueId, null, null);
        }

        public void SetSoundEffectVolume()
        {
            if (UIProgressBar.current == null)
                return;
            GameStateController.Instance.GetGamePrefs().SoundEffectVolume = UIProgressBar.current.value;
            MessageDispatcher2.Instance.DispatchMsg("UpdateVolume", 0f, this.UniqueId, null, null);
        }

        public void Save()
        {
            GameStateController.Instance.SaveGamePrefsToFile();
            // New original in case Settings gets reentered
            //_originalGamePrefsData = GameStateController.Instance.GetGamePrefs().Clone();
            MessageDispatcher2.Instance.DispatchMsg(SaveMessage, 0f, this.UniqueId, "Game Controller", null);
        }

        public void Cancel()
        {
            GameStateController.Instance.GetGamePrefs().SetGamePrefs(_originalGamePrefsData);
            // Volume got reset, send the update to revert to the original value
            MessageDispatcher2.Instance.DispatchMsg("UpdateVolume", 0f, this.UniqueId, null, null);
            MessageDispatcher2.Instance.DispatchMsg(CancelMessage, 0f, this.UniqueId, "Game Controller", null);
        }
    }
}
