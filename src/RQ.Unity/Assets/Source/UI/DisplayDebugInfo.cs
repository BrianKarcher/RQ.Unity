using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.ManageScene
{
    public class DisplayDebugInfo : MonoBehaviour, RQ.Controller.ManageScene.IDisplayDebugInfo
    {
        public int FirePressCount { get; set; }
        public string StoryScene { get; set; }
        public int AverageFPS { get; set; }

        [SerializeField]
        private UILabel _firePressLabel = null;

        [SerializeField]
        private UILabel _fpsLabel = null;

        [SerializeField]
        private UILabel _storySceneLabel = null;

        [SerializeField]
        private UIPanel _debugPanel = null;



        public void Start()
        {
            Display(true);
        }

        public void Display(bool isVisible)
        {
            if (_debugPanel == null)
                return;

            //if (_firePressLabel == null)
            //    return;
            NGUITools.SetActive(_debugPanel.gameObject, true);
        }

        public void LateUpdate()
        {
            if (_firePressLabel != null)
                _firePressLabel.text = FirePressCount.ToString();
            if (_storySceneLabel != null)
                _storySceneLabel.text = StoryScene;
            if (_fpsLabel != null)
                _fpsLabel.text = AverageFPS.ToString();
        }
    }
}
