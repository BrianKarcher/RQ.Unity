using RQ.Animation.Tile;
using RQ.Editor.List;
using RQ2.Animation.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor
{
    [CustomEditor(typeof(AnimatedTilesetConfig), true)]
    public class AnimatedTilesetConfigEditor : EditorBase
    {
        private AnimatedTilesetConfig agent;
        private VariablesEditor _variablesEditor;
        private bool[] materialSelected;
        //private bool[] tilesetSelected;

        //private bool _showAreas = false;

        [MenuItem("Assets/Create/RQ/Animated Tileset")]
        public static void CreateNewAsset()
        {
            EditorBase.CreateAsset<AnimatedTilesetConfig>("AnimatedTileset.asset");
        }

        public void OnEnable()
        {
            agent = target as AnimatedTilesetConfig;
            _variablesEditor = new VariablesEditor(this);
            //ResetAnimatedTilsetSelectedArray();
            ResetMaterialsSelectedArray(agent.AnimatedMaterials.Count);
        }

        //private void ResetAnimatedTilsetSelectedArray()
        //{
        //    Array.Resize(ref tilesetSelected, agent.AnimatedTilesets.Count());
        //}

        private void ResetMaterialsSelectedArray(int size)
        {
            Array.Resize(ref materialSelected, size);
        }

        public void OnSceneGUI()
        {
            var ev = Event.current;
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            base.OnInspectorGUI();



            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sprite Collections");
            EditorGUILayout.EndHorizontal();
            if (agent.SpriteCollections == null)
                agent.SpriteCollections = new List<tk2dSpriteCollection>();
            ListEditor.DrawList(agent.SpriteCollections, (spriteCollection, index) =>
            {
                EditorGUILayout.BeginVertical();
                spriteCollection = EditorGUILayout.ObjectField(spriteCollection, typeof(tk2dSpriteCollection), GUILayout.Width(180)) as tk2dSpriteCollection;
                GUI.enabled = false;
                foreach (var spriteSheet in spriteCollection.spriteSheets)
                {
                    EditorGUILayout.TextField("Sprite Sheet Name", spriteSheet.Name);
                }
                GUI.enabled = true;
                EditorGUILayout.EndVertical();
                return new ListEditor.DrawItemData<tk2dSpriteCollection>
                {
                    name = spriteCollection.name,
                    data = spriteCollection
                };
            });

            EditorGUILayout.BeginVertical();

            agent.TileSet = EditorGUILayout.ObjectField("Tileset", agent.TileSet, typeof(Texture)/*, GUILayout.Width(180)*/) as Texture;
            if (agent.TileSet != null)
            {
                //tileSet.Name = tileSet.TileSet.name;
                agent.TileSetHeight = agent.TileSet.height;
                agent.TileSetWidth = agent.TileSet.width;
            }

            GUI.enabled = false;
            EditorGUILayout.IntField("Tile Set Width", agent.TileSetHeight);
            EditorGUILayout.IntField("Tile Set Height", agent.TileSetWidth);
            GUI.enabled = true;


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Materials");
            EditorGUILayout.EndHorizontal();
            if (agent.AnimatedMaterials == null)
                agent.AnimatedMaterials = new List<AnimatedMaterial>();

            ListEditor.DrawList(agent.AnimatedMaterials, (material, index) =>
            {
                EditorGUILayout.BeginVertical();
                var materialName = material.Material == null ? "(null)" : material.Material.name;
                var name = "Material " + materialName;

                materialSelected[index] = GUILayout.Toggle(materialSelected[index], name, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(true));
                if (materialSelected[index])
                {
                    DrawMaterialInspector(material);
                }
                EditorGUILayout.EndVertical();

                return new ListEditor.DrawItemData<AnimatedMaterial>
                {
                    name = material.Name,
                    data = material
                };
            }, null, () => ResetMaterialsSelectedArray(agent.AnimatedMaterials.Count));
            //}

            if (GUILayout.Button("Process"))
            {
                foreach (var material in agent.AnimatedMaterials)
                {
                    ProcessTiles(material);
                }
            }

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                Dirty();
            }
        }

        private void DrawMaterialInspector(AnimatedMaterial material)
        {
            material.Name = EditorGUILayout.TextField("Name", material.Name);
            material.Material = EditorGUILayout.ObjectField("Material", material.Material, typeof(Material)/*, GUILayout.Width(180)*/) as Material;
            //material.TileSet = EditorGUILayout.ObjectField("Tileset", material.TileSet, typeof(Texture)/*, GUILayout.Width(180)*/) as Texture;
            //if (material.TileSet != null)
            //{
            //    material.Name = material.TileSet.name;
            //    material.TileSetHeight = material.TileSet.height;
            //    material.TileSetWidth = material.TileSet.width;
            //}

            //GUI.enabled = false;
            //EditorGUILayout.IntField("Tile Set Width", material.TileSetHeight);
            //EditorGUILayout.IntField("Tile Set Height", material.TileSetWidth);
            //GUI.enabled = true;

            material.Frames = EditorGUILayout.IntField("Frames", material.Frames);
            material.FirstIndex = EditorGUILayout.IntField("First Index", material.FirstIndex);
            if (material.Frames == 0)
            {
                return;
            }
            if (material.Material != null)
            {
                int singleFrameTileCountX;
                int singleFrameTileCountY;
                GetSingleFrameTileCounts(material.Material.mainTexture, material.Frames, out singleFrameTileCountX,
                    out singleFrameTileCountY);
                GUI.enabled = false;
                EditorGUILayout.TextField("Tile Count X", singleFrameTileCountX.ToString());
                EditorGUILayout.TextField("Tile Count Y", singleFrameTileCountY.ToString());
                GUI.enabled = true;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Animated Tiles");
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Reset"))
            {
                Reset(material);
            }
            if (GUILayout.Button("Process"))
            {
                ProcessTiles(material);
            }
            if (material.AnimatedTiles == null)
                material.AnimatedTiles = new AnimatedTile[0];
            for (int i = 0; i < material.AnimatedTiles.Count(); i++)
            {
                var animatedTile = material.AnimatedTiles[i];
                if (animatedTile != null)
                {
                    GUI.enabled = false;
                    EditorGUILayout.IntField("Index", animatedTile.Index);
                    EditorGUILayout.IntField("x", animatedTile.x);
                    EditorGUILayout.IntField("y", animatedTile.y);
                    GUI.enabled = true;
                }
            }
            //ListEditor.DrawList(material.AnimatedTiles, (animatedTile) =>
            //{
            //    animatedTile.Index
            //    //material.AnimatedTiles
            //    return new ListEditor.DrawItemData<AnimatedTile> 
            //    {
            //        name = animatedTile.Index.ToString(),
            //        data = animatedTile
            //    };
            //});
        }

        private void GetSingleFrameTileCounts(Texture texture, int Frames, out int singleFrameTileCountx,
            out int singleFrameTileCounty)
        {
            singleFrameTileCountx = Mathf.RoundToInt(texture.width / Frames / 16f);
            singleFrameTileCounty = Mathf.RoundToInt(texture.height / 16f);
        }

        private int GetFullTileCountX(int width)
        {
            return Mathf.RoundToInt(width / 16f);
        }

        //private int GetTilesetFullTileCountX(Texture texture)
        //{
        //    return Mathf.RoundToInt(material.mainTexture.width / 16f);
        //}

        private void Reset(AnimatedMaterial material)
        {
            var tileSetTileCountX = GetFullTileCountX(agent.TileSetWidth);
            int singleFrameTileCountX;
            int singleFrameTileCountY;
            GetSingleFrameTileCounts(material.Material.mainTexture, material.Frames, out singleFrameTileCountX,
            out singleFrameTileCountY);
            //Array.
            //material.AnimatedTiles.Remove
            //Array.Resize(ref material.AnimatedTiles, singleFrameTileCountX * singleFrameTileCountY);
            List<AnimatedTile> newAnimatedMaterialList = new List<AnimatedTile>();
            //var currentIndex = material.FirstIndex;
            //var rowIndex = 
            var rowCount = 0;
            for (int y = singleFrameTileCountY - 1; y >= 0; y--)
            {
                //var rowIndex = y;
                for (int x = 0; x < singleFrameTileCountX; x++)
                {
                    AnimatedTile animatedTile = new AnimatedTile();
                    //animatedTile.Index = currentIndex;
                    animatedTile.Index = material.FirstIndex + (rowCount * tileSetTileCountX) + x;
                    animatedTile.y = y;
                    animatedTile.x = x;
                    newAnimatedMaterialList.Add(animatedTile);
                    //currentIndex++;
                }
                //rowIndex += tileSetTileCountX;
                rowCount++;
            }

            material.AnimatedTiles = newAnimatedMaterialList.ToArray();
        }

        private void ProcessTiles(AnimatedMaterial material)
        {
            foreach (var spriteCollection in agent.SpriteCollections)
            {
                //spriteCollection.altMaterials
                //foreach (var material in agent.Materials)
                //{
                if (!material.AnimatedTiles.Any())
                    continue;

                var tileCountX = Mathf.RoundToInt(material.Material.mainTexture.width / material.Frames / 16f);
                var tileCountY = Mathf.RoundToInt(material.Material.mainTexture.height / 16f);
                Debug.Log("Material " + material.Material.mainTexture.name + " tile count: " +
                    tileCountX + "x" + tileCountY);

                for (int i = 0; i < material.AnimatedTiles.Length; i++)
                {
                    var animatedTile = material.AnimatedTiles[i];
                    var tileName = agent.TileSet.name + "/" + animatedTile.Index;
                    var texParam = spriteCollection.textureParams.FirstOrDefault(p => p.name == tileName);
                    if (texParam != null)
                    {
                        var materialIndex = GetMaterialIndex(spriteCollection, material.Material);
                        Debug.Log("Material index " + materialIndex);
                        var tileX = 1f / material.Frames / tileCountX;
                        var tileY = 1f / tileCountY;
                        //var uvx = animatedTile.x / tileCountX / material.Frames;
                        var uvx = tileX * animatedTile.x;
                        //var uvy = animatedTile.y / tileCountY;
                        var uvy = tileY * animatedTile.y;
                        Vector2 pos = new Vector2(uvx + .0005f, uvy + .0005f);
                        // .0005f is used to get rid of issues with rounding errors going beyond the frame
                        Vector2 size = new Vector2(tileX - .0005f, tileY - .0005f);

                        Rect uvRect = new Rect(pos, size);
                        Debug.Log("uv: " + uvRect);
                        //rect.bottom = 1f;

                        texParam.UseCustomUV = true;
                        //texParam.CustomUV = new Vector2[] {new Vector2(0,0),
                        //                   new Vector2(1,0),
                        //                   new Vector2(0,1),
                        //                   new Vector2(1,1)};
                        texParam.CustomUV = new Vector2[] {new Vector2(uvRect.xMin,uvRect.yMin),
                                               new Vector2(uvRect.xMax,uvRect.yMin),
                                               new Vector2(uvRect.xMin,uvRect.yMax),
                                               new Vector2(uvRect.xMax,uvRect.yMax)};
                        texParam.materialId = materialIndex;
                    }
                    //else
                    //{
                    //    texParam.UseCustomUV = false;
                    //    texParam.CustomUV = new Vector2[] {new Vector2(0,0),
                    //                       new Vector2(0,0),
                    //                       new Vector2(0,0),
                    //                       new Vector2(0,0)};
                    //    texParam.materialId = 0;
                    //}
                }
                //}
            }
        }

        private static int GetMaterialIndex(tk2dSpriteCollection spriteCollection, Material material)
        {
            // Locate the material index in the sprite collection
            //var materialIndex = 0;
            for (int i = 0; i < spriteCollection.altMaterials.Length; i++)
            {
                if (spriteCollection.altMaterials[i] == material)
                {
                    return i;
                    //break;
                }
            }
            Debug.Log("Creating material " + material.name);
            // Nothing found?  Create the new altMaterial
            Array.Resize(ref spriteCollection.altMaterials, spriteCollection.altMaterials.Length + 1);
            var lastIndex = spriteCollection.altMaterials.Length - 1;
            spriteCollection.altMaterials[lastIndex] = material;
            return lastIndex;
        }
    }
}
