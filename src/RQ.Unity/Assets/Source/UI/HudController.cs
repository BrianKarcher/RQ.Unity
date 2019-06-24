using Assets.Source.UI;
using RQ.Common.Components;
using RQ.Common.Container;
using RQ.Common.Controllers;
using RQ.Entity.Common;
using RQ.Entity.Data;
using RQ.Messaging;
using RQ.Model.Item;
using RQ.Model.UI;
using RQ.Physics.Components;
using RQ.Render;
using RQ2.Controller.ManageScene;
using System;
using System.Collections.Generic;
using RQ.Entity.Skill;
using UnityEngine;

namespace RQ.UI
{
    [AddComponentMenu("RQ/UI/HUD Controller")]
    public class HudController : ComponentPersistence<HudController>, IHudController
    {
        //public override string UniqueId { get { return "UI Manager"; } set { } }
        public DisplayDebugInfo DisplayDebugInfo;
        public FPSCounter FPSCounter;
        public SpriteBaseComponent PortraitEntity;
        public GameObject FloatingLabel;
        public UILabel HPLabel;
        public UIProgressBar HPBar;
        public UILabel MPLabel;
        public UILabel LevelLabel;
        public UIProgressBar MPBar;
        public UILabel GoldLabel;
        public Transform HudRoot;
        public UITexture HudSkill;
        private List<HUDText> hudTextList;
        [SerializeField]
        private AudioClip _levelUp;

        private RQ.CameraClass _camera;
        //private bool _togglingEnabled = true;

        public RQUIToggleGroup ShardGroup;

        public override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying)
                return;

            hudTextList = new List<HUDText>();
        }

        public override void Update()
        {
            base.Update();
            if (DisplayDebugInfo != null && FPSCounter != null)
            {
                DisplayDebugInfo.AverageFPS = FPSCounter.AverageFPS;
            }
        }

        public override void StartListening()
        {
            MessageDispatcher2.Instance.StartListening("SetCamera", _componentRepository.UniqueId, (data) =>
            {
                //_componentRepository.camera
                _camera = data.ExtraInfo as RQ.CameraClass;
                if (_camera == null)
                    return;
                var uiFollowTargets = HudRoot.GetComponentsInChildren<UIFollowTarget>(true);
                foreach (var uiFollowTarget in uiFollowTargets)
                {
                    uiFollowTarget.gameCamera = _camera.GetComponent<Camera>();
                }
            });
            MessageDispatcher2.Instance.StartListening("AddHudText", _componentRepository.UniqueId, (data) =>
            {
                var hudTextSetupData = (HudTextSetupData)data.ExtraInfo;
                GameObject child = NGUITools.AddChild(HudRoot.gameObject, hudTextSetupData.HudText);
                var hudText = child.GetComponentInChildren<HUDText>();
                hudTextList.Add(hudText);
                var uiFollowTarget = child.AddComponent<UIFollowTarget>();
                uiFollowTarget.target = hudTextSetupData.Target;
                if (_camera == null)
                    _camera = LocateCamera();
                //uiFollowTarget.gameCamera = GameController.Instance.GetCamera().GetComponent<Camera>();
                //if (_camera != null)
                uiFollowTarget.gameCamera = _camera.GetComponent<Camera>();
                MessageDispatcher2.Instance.DispatchMsg("SetHudText", 0f, this.UniqueId,
                    data.SenderId, hudText);
            });
            MessageDispatcher2.Instance.StartListening("RemoveHudText", _componentRepository.UniqueId, (data) =>
            {
                var hudText = (HUDText)data.ExtraInfo;
                hudTextList.Remove(hudText);
                //var hud = HudRoot.GetComponentsInChildren<HUDText>().FirstOrDefault(i => i == hudText);

                if (hudText != null)
                    NGUITools.Destroy(hudText.gameObject);
                //GameObject.Destroy(hud.gameObject);
            });
            MessageDispatcher2.Instance.StartListening("LevelUp", _componentRepository.UniqueId, (data) =>
            {
                if (PortraitEntity == null)
                    return;
                MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
                var entityStats = data.ExtraInfo as EntityStatsData;
                SetHealth(entityStats.CurrentHP, entityStats.MaxHP);
                SetMP(entityStats.CurrentSP, entityStats.MaxSP);
                SetLevel(entityStats.Level);
                MessageDispatcher2.Instance.DispatchMsg("PlayOneShot", 0f, this.UniqueId, "Game Controller", _levelUp);
            });
            MessageDispatcher2.Instance.StartListening("SetMold", _componentRepository.UniqueId, (data) =>
            {
                var mold = data.ExtraInfo as MoldConfig;
                GameDataController.Instance.CurrentMold = mold;
                // The +1 is for the plain mold
                //ShardHudInfo.CurrentShardCount = mold.ShardConfigs.Length + 1;
                Debug.Log("Setting Mold Shard count to " + mold.ShardConfigs.Length + 1);
                ShardGroup.SetShardCount(mold.ShardConfigs.Length + 1);
                //ShardHudInfo.ShardQuantities = new ItemInInventoryData[mold.ShardConfigs.Length];
                //for (int i = 0; i < mold.ShardConfigs.Length; i++)
                //{
                //    var shardConfig = mold.ShardConfigs[i].ItemConfig;
                //    var itemInInventory = GameDataController.Instance.Data.Inventory.GetItem(shardConfig.UniqueId);
                //    // Shift 1 for the Plain mold
                //    ShardHudInfo.Shards[i + 1].Quantity = itemInInventory;
                //}
                UpdateShardQuantities();
            });
            MessageDispatcher2.Instance.StartListening("UpdateHud", _componentRepository.UniqueId, (data) =>
            {
                if (PortraitEntity == null)
                    return;
                var entityStats = data.ExtraInfo as EntityStatsData;
                SetHealth(entityStats.CurrentHP, entityStats.MaxHP);
                SetMP(entityStats.CurrentSP, entityStats.MaxSP);
                SetLevel(entityStats.Level);

                //SetOrbs(entityStats);
                UpdateShardQuantities();
                SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("UpdateStatsInHud", _componentRepository.UniqueId, (data) =>
            {
                if (PortraitEntity == null)
                    return;
                var entityStats = data.ExtraInfo as EntityStatsData;
                if (entityStats == null)
                {
                    var mainCharacter = EntityContainer._instance.GetMainCharacter();
                    if (mainCharacter != null)
                        entityStats = mainCharacter.Components.GetComponent<EntityStatsComponent>().GetEntityStats();
                }
                if (entityStats == null)
                {
                    Debug.LogError("Could not locate Main Character's Entity Stats Component");
                    return;
                }
                SetHealth(entityStats.CurrentHP, entityStats.MaxHP);
                SetMP(entityStats.CurrentSP, entityStats.MaxSP);
                SetLevel(entityStats.Level);
                //SetOrbs(entityStats);
                UpdateShardQuantities();
                SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("SetGold", _componentRepository.UniqueId, (data) =>
            {
                var gold = (int)data.ExtraInfo;
                GoldLabel.text = gold.ToString();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("SetHUDSkill", _componentRepository.UniqueId, (data) =>
            {
                //var gold = (int)data.ExtraInfo;
                var itemUniqueId = (string)data.ExtraInfo;
                //var itemConfig = ConfigsContainer.Instance.GetConfig<ItemConfig>(itemUniqueId);
                if (itemUniqueId == null)
                {
                    HudSkill.mainTexture = null;
                    return;
                }
                var itemConfig = GameDataController.Instance.GetGameConfig().GetAsset<ItemConfig>(itemUniqueId);
                HudSkill.mainTexture = itemConfig == null ? null : itemConfig.GridTexture;
                SetHUDSkillColor();
                //MessageDispatcher2.Instance.DispatchMsg("LevelUp", 0f, this.UniqueId, PortraitEntity.UniqueId, null);
            });
            MessageDispatcher2.Instance.StartListening("AddFollowingLabel", _componentRepository.UniqueId, (data) =>
            {
                var followingLabelData = data.ExtraInfo as FollowingLabelData;
                AddFollowingLabel(followingLabelData.Text, followingLabelData.Target);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("AddHudText", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("RemoveHudText", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("SetCamera", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("LevelUp", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("UpdateHud", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("UpdateStatsInHud", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("SetGold", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("SetHUDSkill", _componentRepository.UniqueId, -1);
            MessageDispatcher2.Instance.StopListening("AddFollowingLabel", _componentRepository.UniqueId, -1);
        }

        private RQ.CameraClass LocateCamera()
        {
            return GameObject.FindObjectOfType<RQ.CameraClass>();
        }

        private void SetHUDSkillColor()
        {
            Color color;
            //if (Utility.IsSelectedSkillAffordable())
            color = new Color(1, 1, 1, 1);
            //else
            //    color = new Color(.8f, 0, 0, .9f);
            HudSkill.color = color;
        }

        public void SetHealth(float current, float max)
        {
            HPBar.value = current / max;
            var iCurrent = (int)current;
            HPLabel.text = String.Format("{0:D3}", iCurrent);
        }

        public void SetMP(float current, float max)
        {
            MPBar.value = current / max;
            var iCurrent = (int)current;
            MPLabel.text = String.Format("{0:D3}", iCurrent);
        }

        public void SetLevel(int level)
        {
            LevelLabel.text = level.ToString();
        }

        public void UpdateShardQuantities()
        {
            //if (ShardHudInfo.ShardQuantities == null)
            //{
            //    Debug.LogError("(HudController) No Shard Quantities array");
            //    return;
            //}
            //if (GameController.Instance.GetSceneSetup().SceneConfig.SceneMaterialConfig == null)
            //{
            //    Debug.LogError("Scene Config " + GameController.Instance.GetSceneSetup().SceneConfig.name + " has no Scene Material Config, cannot update HUD");
            //    return;
            //}
            //var sceneMaterials = GameController.Instance.GetSceneSetup().SceneConfig.SceneMaterialConfig.Materials;
            var shards = this.ShardGroup.GetItems();
            var mold = GameDataController.Instance.CurrentMold as MoldConfig;
            if (mold == null)
            {
                Debug.Log("(HudController) UpdateShardQuantities - No mold selected");
                return;
            }
            for (int i = 0; i < shards.Length; i++)
            {
                var shard = shards[i];
                var itemInInventory = GameDataController.Instance.Data.Inventory.GetItem(mold.ShardConfigs[i].ItemConfig.UniqueId);
                shard.SetQuantity(itemInInventory.Quantity);
            }
        }

        //public void SetOrb(ShardHudData shardData, ItemInInventoryData shardQuantity)
        //{
        //    //if (shardQuantity == null)
        //    //{
        //    //    NGUITools.SetActive(shardData.Shard, false);
        //    //    return;
        //    //}
        //    //NGUITools.SetActive(shardData.Shard, true);
        //    //var itemInInventory = GameDataController.Instance.Data.Inventory.GetItem(orbItem.UniqueId);
        //    int count;
        //    count = shardQuantity.Quantity;
        //    //return;
        //    shardData.Label.text = count.ToString();
        //}

        public override bool HandleMessage(Telegram msg)
        {
            if (base.HandleMessage(msg))
                return true;

            switch (msg.Msg)
            {
                case RQ.Enums.Telegrams.SetCurrentHealth:
                    var entityStatsData = msg.ExtraInfo as EntityStatsData;
                    //if (health < 0)
                    //    health = 0;
                    SetHealth(entityStatsData.CurrentHP, entityStatsData.MaxHP);
                    SetMP(entityStatsData.CurrentSP, entityStatsData.MaxSP);
                    //LifeBar.transform.localScale = new Vector3(health, 1, 1);
                    break;
                    //case Enums.Telegrams.SetStartSequenceOnConversationEnd:
                    //    StartSequenceOnConversationEnd = msg.ExtraInfo as USSequencer;
                    //    break;
            }
            return false;
        }

        public void AddFollowingLabel(string text, Transform target)
        {
            var floatingLabel = NGUITools.AddChild(HudRoot.gameObject, FloatingLabel);
            //FloatingLabel
            var label = floatingLabel.GetComponent<UILabel>();

            label.text = text;
            var uiFollowTarget = floatingLabel.AddComponent<UIFollowTarget>();
            uiFollowTarget.gameCamera = GameController.Instance.GetCamera().GetCamera();
            uiFollowTarget.target = target;
        }

        public void SetTogglingEnabled(bool enabled)
        {
            ShardGroup.TogglingEnabled = enabled;
        }

        public bool GetTogglingEnabled()
        {
            return ShardGroup.TogglingEnabled;
        }
    }
}
