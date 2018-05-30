using RQ.Messaging;
using System.Collections.Generic;
using UnityEngine;

namespace RQ2.Render.UI
{
    [AddComponentMenu("RQ/UI/Paging")]
    public class Paging : MessagingObject
    {
        [SerializeField]
        private List<Transform> _pages;
        [SerializeField]
        private string _nextButton;
        [SerializeField]
        private string _previousButton;
        [SerializeField]
        private UIButton _nextButtonScript;
        [SerializeField]
        private UIButton _previousButtonScript;
        private int _currentPage = 0;

        void Start()
        {
            DisableAllPages();
            NGUITools.SetActive(_pages[_currentPage].gameObject, true);
        }

        private void DisableAllPages()
        {
            foreach (var page in _pages)
            {
                NGUITools.SetActive(page.gameObject, false);
            }
        }

        public override void StartListening()
        {
            base.StartListening();
            MessageDispatcher2.Instance.StartListening("ButtonClicked", this.UniqueId, (data) =>
            {
                NGUITools.SetActive(_pages[_currentPage].gameObject, false);
                var buttonName = (string)data.ExtraInfo;
                if (buttonName == _nextButton)
                {
                    _currentPage++;
                }
                else if (buttonName == _previousButton)
                {
                    _currentPage--;
                }
                if (_currentPage < 0)
                    _currentPage = 0;
                if (_currentPage >= _pages.Count)
                    _currentPage = _pages.Count - 1;
                if (_currentPage == 0)
                    NGUITools.SetActive(_previousButtonScript.gameObject, false);
                else
                    NGUITools.SetActive(_previousButtonScript.gameObject, true);

                if (_currentPage == _pages.Count - 1)
                    NGUITools.SetActive(_nextButtonScript.gameObject, false);
                else
                    NGUITools.SetActive(_nextButtonScript.gameObject, true);

                NGUITools.SetActive(_pages[_currentPage].gameObject, true);
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("ButtonClicked", this.UniqueId, -1);
        }
    }
}
