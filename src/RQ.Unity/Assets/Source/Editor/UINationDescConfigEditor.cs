using RQ.Model.Item;
using RQ2.Editor.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace RQ.Editor.Item
{
    [CustomEditor(typeof(UINationDescConfig), true)]
    public class UINationDescConfigEditor : EditorBase
    {
        [MenuItem("Assets/Create/RQ/Nation Desc Config")]
        public static void CreateNewAsset()
        {
            EditorBase.CreateAsset<UINationDescConfig>("NewNation.asset");
            //ItemConfig sceneData = ScriptableObject.CreateInstance<ItemConfig>();            
            //AssetDatabase.CreateAsset(sceneData, "Assets/Items/NewItem.asset");
            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = sceneData;
        }
    }
}
