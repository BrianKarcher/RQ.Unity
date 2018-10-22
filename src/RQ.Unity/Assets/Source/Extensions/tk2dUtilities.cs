using RQ2.TileMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RQ2.Controller.tk2d_Extensions
{
    public static class tk2dUtilities
    {
        public static void TilemapLoad(tk2dTileMap tileMap, string data)
        {
            Importer.Import(tileMap, data, Importer.Format.TMX);
            SetTilemapLayers(tileMap);
            //tileMap.Build();
        }

        public static void TileMapDeleteAllLayers(tk2dTileMap tileMap)
        {
            //Init();
            if (tileMap == null)
                return;
            //tileMap.BeginEditMode();
            //tileMap.data.tileMapLayers.Clear();      
            //tileMap.layers
            for (int i = tileMap.data.tileMapLayers.Count - 1; i >= 0; i--)
            //for (int i = tileMap.Layers.Length - 1; i >= 0; i--)
                TileMapUtility.DeleteLayer(tileMap, i);
            //tileMap.EndEditMode();

            //tileMap.ForceBuild();
            //Build(tileMap, true);
            //tileMap.
        }

        public static void DeleteUnusedLayers(tk2dTileMap tileMap)
        {
            if (tileMap == null)
                return;
            //tileMap.BeginEditMode();
            //tileMap.data.tileMapLayers.Clear();      
            List<string> layersToDelte = new List<string>() {
                "Tile Layer 20",
                "Tile Layer 14",
                "Ramp Marks",
                "Note Collision Heights",
                "Note Collision Bottom",
                "Sprites",
                //"Red",
                "RED",
                "Reds",
                "Sprite",
                "Secret Door",
                "Copy of RED",
                //"Copy of red",
                "Tile Layer 19",
                //"Copy of Red",
                "Sprites2",
                "Tile Layer 11",
                "Tile Layer 21"
            };
            for (int i = tileMap.data.tileMapLayers.Count - 1; i >= 0; i--)
            {
                if (layersToDelte.Select(p => p.ToUpper()).Contains(tileMap.data.tileMapLayers[i].name.ToUpper()))
                    TileMapUtility.DeleteLayer(tileMap, i);
            }
            //tileMap.ForceBuild();
            //tileMap.EndEditMode();

            //tk2dEditor.TileMap.Importer.Import()
            //_tileMap.
        }

        public static void SetTilemapLayers(tk2dTileMap tileMap)
        {
            //tileMap.BeginEditMode();
            foreach (var layer in tileMap.data.Layers)
            {
                //float z = 0.01f;
                switch (layer.name)
                {
                    case "Water Level 1 Collider":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Water");
                        layer.skipMeshGeneration = true;
                        layer.generateCollider = true;
                        break;
                    case "Level 1 Colliders":
                    case "Level 1 Collider ":
                    case "Level 1 Collider":
                    case "Level 1 Collission":
                    case "Collider":
                    case "Colliders":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 1 TC");
                        layer.skipMeshGeneration = true;
                        layer.generateCollider = true;
                        break;
                    case "Level 2 Colliders":
                    case "Level 2 Collider":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        layer.skipMeshGeneration = true;
                        layer.generateCollider = true;
                        break;
                    case "Level 3 Collider":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 3 TC");
                        layer.skipMeshGeneration = true;
                        layer.generateCollider = true;
                        break;
                    case "Shared Colliders":
                    case "Shared Collider":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Shared TC");
                        layer.skipMeshGeneration = true;
                        layer.generateCollider = true;
                        break;
                    //case "Level 1 Collission":
                    //    layer.z = 0.001f;
                    //    layer.skipMeshGeneration = true;
                    //    break;
                    case "Level 2 Overlay 2":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        layer.generateCollider = false;
                        break;
                    case "Level 2 Overlay 1":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        layer.generateCollider = false;
                        break;
                    case "Level 2 Shadows":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        break;
                    case "Level 2 Overlay":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        break;
                    case "Level 2 overlay":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        break;
                    case "Level 2":
                        layer.z = 0.3f;
                        layer.generateCollider = false;
                        layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        break;
                    case "Level 3":
                        layer.z = 0.3f;
                        layer.generateCollider = false;
                        //layer.unityLayer = LayerMask.NameToLayer("Level 2 TC");
                        break;
                    case "Top (ex: Roof)":
                    case "Top Layer":
                    case "Top Level":
                    case "Top":
                        layer.z = 0.3f;
                        layer.generateCollider = false;
                        break;
                    case "No Collide (ex: Doors)":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Top Overlay 2":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Top Overlay (ex: Chimney)":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Top Layer Overlay 2":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Top Layer Overlay":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Top Shadows":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Shadows":
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                    case "Level 1 (ex: Terrains)":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 1 TC");
                        layer.generateCollider = false;
                        break;
                    case "Level 1 Overlay 2":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 1 TC");
                        layer.generateCollider = false;
                        break;
                    case "Level 1 Overlay 1":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 1 TC");
                        layer.generateCollider = false;
                        break;
                    case "Level 1 Overlay":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 1 TC");
                        layer.generateCollider = false;
                        break;
                    case "Level 1":
                        layer.z = 0.001f;
                        layer.unityLayer = LayerMask.NameToLayer("Level 1 TC");
                        layer.generateCollider = false;
                        break;
                    default:
                        layer.z = 0.001f;
                        layer.generateCollider = false;
                        break;
                }
                //layer.z = z;
            }

            var level2Shadows = tileMap.data.Layers.FirstOrDefault(i => i.name == "Level 2 Shadows");

            if (level2Shadows != null)
            {
                var level2 = tileMap.data.Layers.FirstOrDefault(i => i.name == "Level 2");
                level2.z = 0.001f;
                level2Shadows.z = 0.3f;
            }
            //Build(tileMap, true);
            
            //tileMap.Layers
            //tileMap.data.tileMapLayers.Clear();
            //tileMap.EndEditMode();
            //tileMap.
        }
    }
}
