using RQ.Common.Components;
using RQ.Messaging;
using UnityEngine;

namespace UtilityManager
{
    [AddComponentMenu("RQ/Components/AI/Intelligence Component")]
    public class IntelligenceComponent : ComponentPersistence<IntelligenceComponent>
    {
        private IntelligenceBehavior _intelligenceBehavior;
        private long _globalEntityDiedId;
        /// <summary>
        /// Doing this in Start to ensure that all entities are registered
        /// </summary>
        public override void Start()
        {
            base.Start();
            if (!Application.isPlaying)
                return;
            _intelligenceBehavior = GetComponent<IntelligenceBehavior>();
            _intelligenceBehavior.Init(GetComponentRepository());
        }

        public override void StartListening()
        {
            base.StartListening();
            _globalEntityDiedId = MessageDispatcher2.Instance.StartListening("GlobalEntityDied", _componentRepository.UniqueId, (data) =>
            {
                Debug.LogError("GlobalEntityDied received");
                _intelligenceBehavior.RemoveTarget((string)data.ExtraInfo);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("GlobalEntityDied", _componentRepository.UniqueId, _globalEntityDiedId);
        }
    }
}
