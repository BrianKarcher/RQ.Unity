using RQ.Common.Components;
using RQ.Messaging;
using RQ.Model.UI;
using UnityEngine;

namespace RQ2.Physics.Components
{
    [AddComponentMenu("RQ/Components/Hud Text")]
    public class HudTextComponent : ComponentPersistence<HudTextComponent>
    {
        public HUDText HudTextPrefab;
        private HUDText _hudText;

        public override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;
            if (HudTextPrefab == null)
                return;

            var setupData = new HudTextSetupData()
            {
                HudText = HudTextPrefab == null ? null : HudTextPrefab.gameObject,
                Target = this.transform
            };

            MessageDispatcher2.Instance.DispatchMsg("AddHudText", 0f, this.UniqueId, "UI Manager", setupData);
        }

        public override void StartListening()
        {
            base.StartListening();
            if (!Application.isPlaying)
                return;
            MessageDispatcher2.Instance.StartListening("SetHudText", this.UniqueId, (data) =>
            {
                var parentName = transform.parent.name;
                var hudText = (HUDText)data.ExtraInfo;
                _hudText = hudText;
            });

            _componentRepository.StartListening("AddHudText", this.UniqueId, (data) =>
            {
                var parentName = transform.parent.name;
                var hudTextEntryData = (HudTextEntryData)data.ExtraInfo;
                if (_hudText == null)
                {
                    Debug.LogError("HUD Text is null in " + _componentRepository.name);
                    return;
                }
                _hudText.Add(hudTextEntryData.Data, hudTextEntryData.Color, hudTextEntryData.Duration);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            if (!Application.isPlaying)
                return;
            MessageDispatcher2.Instance.StopListening("SetHudText", this.UniqueId, -1);
            _componentRepository.StopListening("AddHudText", this.UniqueId);
        }

        public override void Destroy()
        {
            base.Destroy();
            MessageDispatcher2.Instance.DispatchMsg("RemoveHudText", 0f, this.UniqueId, "UI Manager", _hudText);
        }

    }
}
