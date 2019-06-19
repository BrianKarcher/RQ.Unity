using RQ.Model.Item;
using System;
using UnityEngine;

namespace Assets.Source.UI
{
    [Serializable]
    public class ShardHudData
    {
        public UILabel Label;
        public GameObject Shard;
        public GameObject Arrow;
        public bool IsHammer;
        public ItemInInventoryData Quantity { get; set; }
    }
}
