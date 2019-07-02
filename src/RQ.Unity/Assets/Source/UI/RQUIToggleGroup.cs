using UnityEngine;

namespace Assets.Source.UI
{
    public enum ToggleDirection
    {
        Right = 0,
        Left = 1
    }

    [AddComponentMenu("RQ/UI/Toggle Group")]
    public class RQUIToggleGroup : MonoBehaviour
    {
        public string AxisName;
        [SerializeField]
        private RQUIToggleShard[] _items;
        public float Deadzone = 0.3f;
        public ToggleDirection ToggleDirection = ToggleDirection.Right;

        private Rewired.Player _player;
        private int _currentShardCount;
        public int CurrentItem;
        private bool _axisExtendedLeft = false;
        private bool _axisExtendedRight = false;
        public bool TogglingEnabled { get; set; }

        public void Awake()
        {
            //_items[CurrentItem].Selected();
        }

        public void Start()
        {
            _player = Rewired.ReInput.players.GetPlayer(0);
            SetCurrentItem(-1);
        }

        public void Update()
        {
            var currentAxis = _player.GetAxis(AxisName);
            int newItem;
            if (currentAxis < -Deadzone && !_axisExtendedLeft)
            {
                newItem = ToggleDirection == ToggleDirection.Right ? CurrentItem - 1 : CurrentItem + 1;
                SetCurrentItem(newItem);
                _axisExtendedLeft = true;
                _axisExtendedRight = false;
            }
            if (currentAxis > -Deadzone && currentAxis < Deadzone)
            {
                _axisExtendedLeft = false;
                _axisExtendedRight = false;
            }
            if (currentAxis > Deadzone && !_axisExtendedRight)
            {
                newItem = ToggleDirection == ToggleDirection.Right ? CurrentItem + 1 : CurrentItem - 1;
                SetCurrentItem(newItem);
                _axisExtendedLeft = false;
                _axisExtendedRight = true;
            }
        }

        private void CheckBounds()
        {
            if (CurrentItem < 0)
                CurrentItem = _currentShardCount - 1;
            if (CurrentItem > _currentShardCount - 1)
                CurrentItem = 0;
        }

        public void SetShardCount(int count)
        {
            _currentShardCount = count;
            for (int i = 0; i < _items.Length; i++)
            {
                var go = _items[i].gameObject;
                if (i < count)
                    NGUITools.SetActive(go, true);
                else
                    NGUITools.SetActive(go, false);
            }
            // Perform a bounds check in case the shard slot is no longer available
            SetCurrentItem(CurrentItem);
        }

        public void SetCurrentItem(int index)
        {
            _items[CurrentItem].UnSelected();
            CurrentItem = index;
            CheckBounds();
            _items[CurrentItem].Selected();
        }

        public RQUIToggleShard[] GetItems()
        {
            return _items;
        }
    }
}
