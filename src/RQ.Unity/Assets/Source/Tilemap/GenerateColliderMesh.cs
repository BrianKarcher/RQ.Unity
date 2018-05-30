using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Source.Tilemap
{
    [AddComponentMenu("RQ/Pathfinding/Collider Mesh")]
    [ExecuteInEditMode]
    public class GenerateColliderMesh : MonoBehaviour
    {
        public Mesh _mesh;
        public MeshCollider _meshCollider;
        public tk2dTileMap _tileMap;

        public void Start()
        { 
            //if (_mesh == null)
            _mesh = Generate();
            if (_meshCollider == null)
                _meshCollider = GetComponent<MeshCollider>();
            if (_meshCollider == null)
                return;
            _meshCollider.sharedMesh = _mesh;
        }

        public void Update()
        {
            _mesh = Generate();
            //if (_meshCollider == null)
            //    _meshCollider = GetComponent<MeshCollider>();
            //if (_meshCollider == null)
            //    return;
            _meshCollider.sharedMesh = _mesh;
        }

        //public Mesh Generate()
        //{
        //    Mesh mesh = new Mesh();
        //    mesh.vertices = new Vector3[] { new Vector3(4f, 0f),
        //        new Vector3(0f, 4f),
        //        new Vector3(0f, 0f)
        //    };
        //    mesh.triangles = new int[] { 0, 1, 2 };
        //    mesh.RecalculateNormals();
        //    return mesh;
        //}

        public Mesh Generate()
        {
            if (_tileMap == null)
                return null;

            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            int numLayers = _tileMap.data.NumLayers;
            var tileSize = _tileMap.data.tileSize;
            var tileSizeHalf = tileSize / 2f;
            int vertexIndex = 0;

            for (int layerId = 0; layerId < numLayers; ++layerId)
            {
                var layer = _tileMap.Layers[layerId];
                var layerData = _tileMap.data.tileMapLayers[layerId];
                if (layer.IsEmpty)
                    continue;
                if (!layerData.name.Contains("TC") && !layerData.name.Contains("Collider"))
                    continue;
                for (int cellY = 0; cellY < _tileMap.height; ++cellY)
                {
                    //int baseY = cellY * layer.divY;
                    for (int cellX = 0; cellX < _tileMap.width; ++cellX)
                    {
                        var tileId = layer.GetTile(cellX, cellY);
                        if (tileId != -1)
                        {
                            var tilePos = _tileMap.GetTilePosition(cellX, cellY);
                            //var lowerLeft = new Vector3(tilePos.x - tileSizeHalf.x, tilePos.y - tileSizeHalf.y, 0f);
                            //var lowerRight = new Vector3(tilePos.x + tileSizeHalf.x, tilePos.y - tileSizeHalf.y, 0f);
                            //var upperLeft = new Vector3(tilePos.x - tileSizeHalf.x, tilePos.y + tileSizeHalf.y, 0f);
                            //var upperRight = new Vector3(tilePos.x + tileSizeHalf.x, tilePos.y + tileSizeHalf.y, 0f);

                            var lowerLeft = new Vector3(tilePos.x, tilePos.y, 0f);
                            var lowerRight = new Vector3(tilePos.x + tileSize.x, tilePos.y, 0f);
                            var upperLeft = new Vector3(tilePos.x, tilePos.y + tileSize.y, 0f);
                            var upperRight = new Vector3(tilePos.x + tileSize.x, tilePos.y + tileSize.y, 0f);

                            vertices.Add(lowerLeft);
                            var lowerLeftId = vertexIndex;
                            vertices.Add(lowerRight);
                            var lowerRightId = vertexIndex + 1;
                            vertices.Add(upperLeft);
                            var upperLeftId = vertexIndex + 2;
                            vertices.Add(upperRight);
                            var upperRightId = vertexIndex + 3;
                            triangles.AddRange(new int[] { lowerLeftId, upperLeftId, lowerRightId });
                            triangles.AddRange(new int[] { upperLeftId, upperRightId, lowerRightId });
                            vertexIndex += 4;
                            //layer.
                        }
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();

            //mesh.vertices = new Vector3[] { new Vector3(4f, 0f),
            //    new Vector3(0f, 4f),
            //    new Vector3(0f, 0f)
            //};
            //mesh.triangles = new int[] { 0, 1, 2 };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
