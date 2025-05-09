﻿using Pathfinding;
using RQ2.Controller.tk2d_Extensions;
//using RQ.Controller.tk2d_Extensions;
using RQ2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tk2dEditor.TileMap;
using UnityEditor;
using UnityEngine;

namespace RQ2.Editor._2D_Toolkit
{
    public class Tilemap_Importer
    {
        public static TilemapSetup _tileMapSetup;
        public static tk2dTileMap _tileMap;
        //public static AstarPath _astarPath;

        public static void Init()
        {
            _tileMapSetup = GameObject.FindObjectOfType<TilemapSetup>();
            _tileMap = _tileMapSetup.TileMapTest;
            //var astarGraphs = GameObject.FindObjectsOfType<AstarPath>();
            //if (astarGraphs != null)
            //{
            //    _astarPath = astarGraphs.FirstOrDefault(i => i.graphs.Any());
            //}
        }

        //[MenuItem("Tools/Delete All Tilemap Layers")]
        //public static void DeleteAllLayers()
        //{
        //    //Init();
        //    if (_tileMap == null)
        //        return;
        //    //tileMap.BeginEditMode();
        //    //tileMap.data.tileMapLayers.Clear();      
        //    for (int i = _tileMap.data.tileMapLayers.Count - 1; i >= 0; i--)
        //        TileMapUtility.DeleteLayer(_tileMap, i);
        //    //tileMap.EndEditMode();

        //    //tileMap.ForceBuild();
        //    //Build(tileMap, true);
        //    //tileMap.
        //}

        //[MenuItem("Tools/Set Tilemap Layer Settings")]
        public static void LayerSettings()
        {
            //Init();
            //if (_tileMap == null)
            //    return;
            tk2dUtilities.SetTilemapLayers(_tileMap);
            //_tileMap.ForceBuild();
        }

        //[MenuItem("Tools/Delete select Tilemap Layers")]


        //[MenuItem("Tools/Collider Converter")]
        //public static void ColliderConverter()
        //{
        //    Init();

        //    EdgeCollider2D[] edgeColliders = _tileMap.renderData.GetComponentsInChildren<EdgeCollider2D>();
        //    var polygonColliders = _tileMap.renderData.GetComponentsInChildren<PolygonCollider2D>();

        //    // Only destroy the polygon colliders if there are edge colliders to convert
        //    // This prevents the user from accidently removing all tile colliders from the map
        //    // ie, first run turns all edge colliders into polygon colliders.  Second run deletes
        //    // all polygon colliders.
        //    if (edgeColliders.Any())
        //    {
        //        for (int i = polygonColliders.Count() - 1; i > 0; i--)
        //        {
        //            GameObject.DestroyImmediate(polygonColliders[i]);
        //        }
        //    }

        //    foreach (var coll in edgeColliders)
        //    {
        //        // myPoints.Clear();
        //        Vector2[] myPoints = coll.points;
        //        int myEdges = coll.edgeCount;
        //        PolygonCollider2D myPoly = coll.gameObject.AddComponent<PolygonCollider2D>();
        //        myPoly.points = myPoints;
        //        myPoly.pathCount = 1;
        //        myPoly.SetPath(0, myPoints);
        //        //if (convertToTriggers) myPoly.isTrigger = true;
        //        GameObject.DestroyImmediate(coll);
        //    }
        //}

        [MenuItem("Tools/Wall Collider Converter")]
        public static void WallColliderConverter()
        {
            Init();
            _tileMapSetup.ColliderConverter(_tileMapSetup.TileMapTest);
        }

        //[MenuItem("Tools/Ground Collider Converter")]
        //public static void GroundColliderConverter()
        //{
        //    Init();
        //    _tileMapSetup.CreateGroundCollider(_tileMapSetup.TileMapTest);
        //}

        [MenuItem("Tools/Delete Converted Tiles")]
        public static void DeleteConvertedColliders()
        {
            Init();
            //for (int layerIndex = 0; layerIndex < _tileMap.data.Layers.Count(); layerIndex++)
            //{
            //    var layer = _tileMap.data.Layers[layerIndex];
            //    if (!layer.name.Contains("Collider"))
            //        continue;

            //    GameObject go = new GameObject("poly collider");
            //    if (layer.name.Contains("Level 1"))
            //        go.layer = LayerMask.NameToLayer("Level 1 TC");
            //    if (layer.name.Contains("Level 2"))
            //        go.layer = LayerMask.NameToLayer("Level 2 TC");
            //    if (layer.name.Contains("Shared"))
            //        go.layer = LayerMask.NameToLayer("Shared TC");

            foreach (Transform layergo in _tileMap.renderData.transform)
            {
                foreach (Transform collidergo in layergo)
                {
                    if (collidergo.name == "poly collider")
                    {
                        GameObject.DestroyImmediate(collidergo.gameObject);
                        //go.transform.SetParent(collidergo);
                        //break;
                    }
                }
            }
            //}
        }

        //[MenuItem("Tools/Run All")]
        //public static void RunAll()
        //{
        //    EditorUtility.DisplayDialog("Import", "Deleting all layers", "Ok");
        //    DeleteAllLayers();
        //    EditorUtility.DisplayDialog("Import", "Importing Tile Map", "Ok");
        //    //if (!ImportTileMap())
        //    //    return;
        //    EditorUtility.DisplayDialog("Import", "Layer Settings", "Ok");
        //    LayerSettings();
        //    EditorUtility.DisplayDialog("Import", "Deleting unecessary layers", "Ok");
        //    DeleteLayers();
        //    //EditorUtility.DisplayDialog("Import", "Collider Converter", "Ok");
        //    //ColliderConverter();
        //    //EditorUtility.DisplayDialog("Import", "Set path settings", "Ok");
        //    //SetPathSettings();

            //    //UpdateSpriteCollectionData();
            //}

            //[MenuItem("Tools/Setup Animations")]
            //public static void UpdateSpriteCollectionData()
            //{
            //    Init();
            //    var collection = _sceneSetup.AnimatedSpriteCollectionsConfig;
            //    foreach (var spriteCollection in collection.SpriteCollections)
            //    {
            //        //spriteCollection.altMaterials
            //        foreach (var material in collection.Materials)
            //        {
            //            if (material.AnimatedTiles.Count == 0)
            //                continue;

            //            var tileCountX = Mathf.RoundToInt(material.Material.mainTexture.width / material.Frames / 16f);
            //            var tileCountY = Mathf.RoundToInt(material.Material.mainTexture.height / 16f);
            //            Debug.Log("Material " + material.Material.mainTexture.name + " tile count: " + 
            //                tileCountX + "x" + tileCountY);

            //            for (int i = 0; i < material.AnimatedTiles.Count; i++)
            //            {
            //                var animatedTile = material.AnimatedTiles[i];
            //                var tileName = material.Name + "/" + animatedTile.Index;
            //                var texParam = spriteCollection.textureParams.FirstOrDefault(p => p.name == tileName);
            //                if (texParam != null)
            //                {
            //                    var materialIndex = GetMaterialIndex(spriteCollection, material.Material);
            //                    Debug.Log("Material index " + materialIndex);
            //                    var tileX = 1f / material.Frames / tileCountX;
            //                    var tileY = 1f / tileCountY;
            //                    //var uvx = animatedTile.x / tileCountX / material.Frames;
            //                    var uvx = tileX * animatedTile.x;
            //                    //var uvy = animatedTile.y / tileCountY;
            //                    var uvy = tileY * animatedTile.y;
            //                    Vector2 pos = new Vector2(uvx, uvy);
            //                    Vector2 size = new Vector2(tileX, tileY);

            //                    Rect uvRect = new Rect(pos, size);
            //                    Debug.Log("uv: " + uvRect);
            //                    //rect.bottom = 1f;

            //                    texParam.UseCustomUV = true;
            //                    //texParam.CustomUV = new Vector2[] {new Vector2(0,0),
            //                    //                   new Vector2(1,0),
            //                    //                   new Vector2(0,1),
            //                    //                   new Vector2(1,1)};
            //                    texParam.CustomUV = new Vector2[] {new Vector2(uvRect.xMin,uvRect.yMin),
            //                                       new Vector2(uvRect.xMax,uvRect.yMin),
            //                                       new Vector2(uvRect.xMin,uvRect.yMax),
            //                                       new Vector2(uvRect.xMax,uvRect.yMax)};
            //                    texParam.materialId = materialIndex;
            //                }
            //                //else
            //                //{
            //                //    texParam.UseCustomUV = false;
            //                //    texParam.CustomUV = new Vector2[] {new Vector2(0,0),
            //                //                       new Vector2(0,0),
            //                //                       new Vector2(0,0),
            //                //                       new Vector2(0,0)};
            //                //    texParam.materialId = 0;
            //                //}
            //            }
            //        }
            //    }


            //    //var spriteCollection = _sceneSetup.spriteCollection.spriteCollection;

            //    //foreach (var definition in spriteCollection.spriteDefinitions)
            //    //{
            //    //    if (definition.name == "Fire Tileset/0")
            //    //    {
            //    //        Debug.Log("Found Fire Tileset/0");
            //    //        definition.materialId = materialId;
            //    //        definition.material = _sceneSetup.testMaterial;

            //    //        definition.uvs = new Vector2[] {new Vector2(0,0),
            //    //                                   new Vector2(1,0),
            //    //                                   new Vector2(0,1),
            //    //                                   new Vector2(1,1)};
            //    //    }
            //    //    //definition.materialId
            //    //}
            //    //foreach (var definition2 in _sceneSetup.spriteCollection.textureParams)
            //    //{
            //    //    if (definition2.name == "Fire Tileset/0")
            //    //    {
            //    //        definition2.UseCustomUV = true;
            //    //        definition2.CustomUV = new Vector2[] {new Vector2(0,0),
            //    //                                   new Vector2(1,0),
            //    //                                   new Vector2(0,1),
            //    //                                   new Vector2(1,1)};
            //    //        definition2.materialId = materialId;
            //    //    }
            //    //}
            //    //_sceneSetup.TileMap.ForceBuild();
            //}

        [MenuItem("Tools/Tilemap/Import Test")]
        public static bool ImportTestTileMap()
        {
            Init();
            //var tileMapPath = _sceneSetup.GetTilemapPath();
            //if (String.IsNullOrEmpty(tileMapPath))
            //{
            //    EditorUtility.DisplayDialog("Error", "No tile map path to import.", "Ok");
            //    return false;
            //}
            //Importer.Import(_tileMap, @"C:\Users\Brian\Google Drive\Work\Rolans Quest\Assets\Area\South\Forest Town\Sections\Day\Section 2\Section 2.tmx", Importer.Format.TMX);
            //tk2dUtilities.SetTilemapLayers(_tileMap);

            var tileMap = _tileMapSetup.TileMapTest;
            tk2dUtilities.TilemapLoad(tileMap, _tileMapSetup.GetTileMapTmxData());
            tk2dUtilities.DeleteUnusedLayers(tileMap);
            tileMap.Build();
            var renderData = tileMap.renderData;
            SetTagRecursive(renderData, "Tilemap");
            //renderData.tag = "Tilemap";
            

            //Importer.Import(_tileMap, tileMapPath, Importer.Format.TMX);
            return true;
        }

        private static void SetTagRecursive(GameObject go, string tag)
        {
            go.tag = tag;
            foreach (Transform child in go.transform)
            {
                SetTagRecursive(child.gameObject, tag);
            }
        }

        [MenuItem("Tools/Tilemap/Clear Test")]
        public static void ClearTestTileMap()
        {
            Init();
            tk2dUtilities.TileMapDeleteAllLayers(_tileMap);
        }

        [MenuItem("Tools/Tilemap/Clear Main")]
        public static void ClearMainTileMap()
        {
            Init();
            tk2dUtilities.TileMapDeleteAllLayers(_tileMapSetup.TileMap);
        }

        [MenuItem("Tools/Tilemap/Clear Main Prefabs")]
        public static void ClearMainTileMapPrefabs()
        {
            Init();
            _tileMapSetup.TileMap.data.tilePrefabs = new GameObject[0];
            EditorUtility.SetDirty(_tileMapSetup.TileMap.data);
        }

        [MenuItem("Tools/Tilemap/Clear Test Prefabs")]
        public static void ClearTestTileMapPrefabs()
        {
            Init();
            _tileMapSetup.TileMapTest.data.tilePrefabs = new GameObject[0];
            EditorUtility.SetDirty(_tileMapSetup.TileMapTest.data);
        }

        ////////[MenuItem("Tools/Set AStar settings")]
        ////////public static void SetPathSettings()
        ////////{
        ////////    Init();
        ////////    GridGraphEditor utility = new GridGraphEditor();
        ////////    var count = _sceneSetup.GetAStar().graphs.Count();
        ////////    EditorUtility.DisplayDialog("Data", "Graph count: " + count, "Ok");
        ////////    foreach (var graph in _sceneSetup.GetAStar().graphs.Select(i => (GridGraph)i))
        ////////    {
        ////////        utility.SnapSizeToNodes(_tileMap.width, _tileMap.height, graph);
        ////////        //graph.Width = _tileMap.width;
        ////////        //graph.Depth = _tileMap.height;
        ////////    }
        ////////}

        //public static void Build(tk2dTileMap tileMap, bool force, bool incremental)
        //{
        //    if (force)
        //    {
        //        //if (buildKey != tileMap.buildKey)
        //        //tk2dEditor.TileMap.TileMapUtility.CleanRenderData(tileMap);

        //        tk2dTileMap.BuildFlags buildFlags = tk2dTileMap.BuildFlags.EditMode;
        //        if (!incremental) buildFlags |= tk2dTileMap.BuildFlags.ForceBuild;
        //        tileMap.Build(buildFlags);
        //    }
        //}

        //public static void Build(tk2dTileMap tileMap, bool force) { Build(tileMap, force, false); }
    }
}
