using RQ.Common.Container;
using RQ.Messaging;
using RQ.Physics.Components;
using UnityEngine;

namespace RQ.Controller.UI
{
    [AddComponentMenu("RQ/UI/Minimap")]
    public class Minimap : MessagingObject
    {
        [SerializeField]
        private Camera _minimapCamera;
        //private Vector2 _maxTilemapDimensions;
        [SerializeField]
        private tk2dSprite _miniMap;
        [SerializeField]
        private Transform _playerIcon;
        //[SerializeField]
        //private bool _toggleOn;
        private Vector2 _minimapScale;
        private CameraClass _camera;
        private bool isActive = true;

        //public UITexture _texture;
        //public Vector2 _offset;
        //public Vector2 _scale;

        void LateUpdate()
        {
            if (!Application.isPlaying)
                return;
            // Camera
            _minimapCamera.transform.position = new Vector3(_camera.transform.position.x * _minimapScale.x, 
                _camera.transform.position.y * _minimapScale.y, _minimapCamera.transform.position.z);

            // Player
            var mainCharacter = EntityContainer._instance.GetMainCharacter();
            var footPosition = mainCharacter.Components.GetComponent<PhysicsComponent>().GetFeetWorldPosition();
            //var mainCharacterPosition = (Vector2)mainCharacter.transform.position;

            _playerIcon.transform.position = new Vector3(footPosition.x * _minimapScale.x, footPosition.y * _minimapScale.y,
                _playerIcon.transform.position.z);

            //_texture.material.SetTextureOffset("offset", _offset);
            //_texture.material.SetTextureScale("scale", _scale);
            //_texture.mainTexture.
        }

        public override void Start()
        {
            base.Start();
            var sceneSetup = SceneSetup.Instance;
            if (sceneSetup != null)
            {
                //var tileMap = sceneSetup.TileMap;
                //SetDimensions();
                _camera = GameObject.FindObjectOfType<CameraClass>();
                MessageDispatcher2.Instance.DispatchMsg("ShowMinimap", 0f, this.UniqueId, "UI Manager", true);
            }
            //_camera = GetComponent<CameraClass>();
        }

        public override void StartListening()
        {
            base.StartListening();
            MessageDispatcher2.Instance.StartListening("HandleInput", this.UniqueId, (data) =>
            {
                var rawInput = (RawInput)data.ExtraInfo;
                if (rawInput.IsButtonPressed(Input.Data.Button.ToggleMinimap))
                {
                    isActive = !isActive;
                    MessageDispatcher2.Instance.DispatchMsg("ShowMinimap", 0f, this.UniqueId, "UI Manager", isActive);
                }
            });
        }

        public override void StopListening()
        {
            base.StopListening();
            MessageDispatcher2.Instance.StopListening("HandleInput", this.UniqueId, -1);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            MessageDispatcher2.Instance.DispatchMsg("ShowMinimap", 0f, this.UniqueId, "UI Manager", false);
        }

        public void SetDimensions(Vector2 tileMapSize)
        {
            //var tileMapSize = new Vector2((float)tileMap.width * 0.16f, (float)tileMap.height * 0.16f);
            var mapSize = GameController.Instance.GetCamera().GetMaxBounds();
            var minimapSize = (Vector2)_miniMap.GetUntrimmedBounds().size;
            _minimapScale = new Vector2(minimapSize.x / mapSize.x, minimapSize.y / mapSize.y);
        }
    }
}
