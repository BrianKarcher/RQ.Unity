using RQ.Model.Item;
using System.Collections.Generic;
using RQ.Entity.Item;
using UnityEngine;

namespace RQ2.Controller.UI.Grid
{
    [AddComponentMenu("RQ/Item/Skill Slot")]
    public class SkillSlot : ItemSlot
    {
        public List<OrbLabel> OrbLabels;

        public override void Start()
        {
            base.Start();
            Qty.text = Value.ToString();
            if (OrbLabels == null)
                return;
            var skillConfig = ItemConfig as SkillConfig;
            foreach (var orbLabel in OrbLabels)
            {
                OrbCost orbConfig = null;
                foreach (var orb in skillConfig.Orbs)
                {
                    if (orb.Orb == orbLabel.Orb)
                    {
                        orbConfig = orb;
                        break;
                    }
                }
                //var orbConfig = skillConfig.Orbs.FirstOrDefault(i => i.Orb == orbLabel.Orb);
                // Could not locate the orb in the skill cost structure, this orb costs zero
                if (orbConfig == null)
                {
                    orbLabel.Label.text = "0";
                    continue;
                }
                orbLabel.Label.text = orbConfig.Cost.ToString();
            }
        }
    }
}
