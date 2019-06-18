using RQ.Model.Item;
using System;

namespace Assets.Source.UI
{
    [Serializable]
    public class ShardHudInfo
    {
        public ItemInInventoryData[] ShardQuantities { get; set; }
        public ShardHudData[] Shards;

        public int CurrentShardIndex { get; set; }
        public int CurrentShardCount { get; set; }
    }
}
