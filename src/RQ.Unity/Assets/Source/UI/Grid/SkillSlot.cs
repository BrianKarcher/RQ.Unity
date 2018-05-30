using RQ.Controller.ManageScene;
using RQ.Entity.StatesV2.Conditions;
using RQ.Messaging;
using RQ.Model.Item;
using RQ.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                var orbConfig = skillConfig.Orbs.FirstOrDefault(i => i.Orb == orbLabel.Orb);
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
