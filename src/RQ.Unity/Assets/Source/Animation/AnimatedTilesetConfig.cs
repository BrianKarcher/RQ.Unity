using RQ.Animation.Tile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RQ2.Animation.Tile
{
    [Serializable]
    public class AnimatedTilesetConfig : ScriptableObject
    {
        public List<tk2dSpriteCollection> SpriteCollections;
        public Texture TileSet;
        public int TileSetWidth;
        public int TileSetHeight;
        public List<AnimatedMaterial> AnimatedMaterials;
    }
}
