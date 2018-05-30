using RQ;
using RQ2.Controller.tk2d_Extensions;
using UnityEngine;

namespace RQ2.UI
{
    [AddComponentMenu("RQ/UI/Tilemap Setup")]
    public class TilemapSetup : MonoBehaviour
    {
        public tk2dTileMap TileMap;
        public tk2dTileMap TileMapTest;
        public tk2dSpriteCollectionData SpriteCollectionData;
        [SerializeField]
        private string _tileMapVersion;
        [SerializeField]
        private string _tilemapPath;
        [SerializeField]
        private string _tilemapFileName;
        [SerializeField]
        private TextAsset _tileMapTmx;
        [SerializeField]
        private CameraClass _camera;


        public void Awake()
        {
            if (TileMap != null)
            {
                //TileMap.Editor__SpriteCollection = SpriteCollectionData;
                //TileMap.SpriteCollectionInst = SpriteCollection;
                tk2dUtilities.TileMapDeleteAllLayers(TileMap);
                TileMap.data.tilePrefabs = new GameObject[0];
                TileMap.ForceBuild();
                //tk2dUtilities.TileMapDeleteAllLayers(TileMap);
                tk2dUtilities.TilemapLoad(TileMap, _tileMapTmx.text);
                tk2dUtilities.DeleteUnusedLayers(TileMap);
                TileMap.ForceBuild();
                var renderData = TileMap.renderData;
                SetTagRecursive(renderData, "Tilemap");
            }
        }

        public void Start()
        {
            SetupCamera();
        }

        public void OnDestroy()
        {
            if (TileMap == null)
                return;
            tk2dUtilities.TileMapDeleteAllLayers(TileMap);
        }

        /// <summary>
        /// TODO Make this a GameObject extension method
        /// </summary>
        /// <param name="go"></param>
        /// <param name="tag"></param>
        private static void SetTagRecursive(GameObject go, string tag)
        {
            go.tag = tag;
            foreach (Transform child in go.transform)
            {
                SetTagRecursive(child.gameObject, tag);
            }
        }

        public void SetupCamera()
        {
            //CameraClass cameraClass = GameController.Instance.GetCamera();
            //var cameraClass = GameController.Instance.GetSceneSetup().camera

            if (_camera != null && TileMap != null)
            {
                //cameraClass.Init();
                _camera.SetMaxBounds(TileMap.width, TileMap.height);
            }


            //if (cameraClass.CameraMode == RQ.CameraMode.FollowTarget)
            //{
            //    if (EntityContainer._instance.GetMainCharacter() == null)
            //        throw new Exception("Main Character is null in BeginSceneState - Setup Camera");
            //    cameraClass.Target = EntityContainer._instance.GetMainCharacter().transform;
            //}
        }

        public string GetTileMapTmxData()
        {
            return _tileMapTmx.text;
        }
    }
}
