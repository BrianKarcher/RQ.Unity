using RQ.Model.Item;
using UnityEngine;

namespace Assets.Source.UI
{
    [AddComponentMenu("RQ/UI/Shard")]
    public class RQUIToggleShard : MonoBehaviour
    {
        public UILabel Label;
        public GameObject Arrow;
        public bool IsHammer;
        /// <summary>
        /// Ties a shard to an Item Config
        /// </summary>
        public ItemConfig ItemConfig { get; set; }

        public void Awake()
        {
            NGUITools.SetActive(Arrow, false);
        }

        public void SetQuantity(int quantity)
        {
            if (Label != null)
                Label.text = quantity.ToString();
        }

        public void Selected()
        {
            NGUITools.SetActive(Arrow, true);
        }

        public void UnSelected()
        {
            NGUITools.SetActive(Arrow, false);
        }
    }
}
