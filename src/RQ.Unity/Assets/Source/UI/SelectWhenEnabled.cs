using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.UI
{
    [AddComponentMenu("RQ/UI/UI Select When Enabled")]
    public class SelectWhenEnabled : MonoBehaviour
    {
        void OnEnable()
        {
            UICamera.currentScheme = UICamera.ControlScheme.Controller;
            UICamera.hoveredObject = gameObject;
        }
    }
}
