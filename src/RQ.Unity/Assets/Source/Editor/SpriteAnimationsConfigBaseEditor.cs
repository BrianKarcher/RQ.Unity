using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using RQ.Entity.Common;
using RQ.Entity.UI;
using RQ.Controller.ManageScene;
using RQ.Controller.Actions;
using RQ.Editor.UnityExtensions;
using RQ2.Editor.UI;
using RQ.Controller.Actions.Conditionals;
using RQ2.AnimationV2;
using RQ.Editor.Common;
using System.IO;
using RQ.Editor.List;
using RQ.Editor;
using RQ.Enums;
using RQ.AnimationV2;
using RQ;

namespace RQ2.Editor
{
    [CustomEditor(typeof(RQ2.AnimationV2.SpriteAnimationsConfig), true)]
    public class SpriteAnimationsConfigEditor : EditorBase
    {
        private RQ2.AnimationV2.SpriteAnimationsConfig agent;

        [MenuItem("Assets/Create/RQ/Sprite Animation Container")]
        public static void CreateNewAsset()
        {
            EditorBase.CreateAsset<RQ2.AnimationV2.SpriteAnimationsConfig>("SpriteAnimations.asset");
            //string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            //if (path == "")
            //{
            //    path = "Assets";
            //}
            //else if (Path.GetExtension(path) != "")
            //{
            //    path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            //}

            ////string path = "Assets/Items/SpriteAnimations.asset";
            ////if (Selection.activeObject != null)
            ////{
            ////    path = EditorUtility.GetAssetPath(Selection.activeGameObject) + "/SpriteAnimations.asset";
            ////}
            //SpriteAnimationsConfig sceneData = ScriptableObject.CreateInstance<SpriteAnimationsConfig>();
            ////AssetDatabase.CreateAsset(sceneData, path);
            //AssetDatabase.CreateAsset(sceneData, AssetDatabase.GenerateUniqueAssetPath(path + "/SpriteAnimations.asset"));
            //EditorUtility.FocusProjectWindow();
            //Selection.activeObject = sceneData;
        }

        //private SceneSetup _sceneSetup;

        public void OnEnable()
        {
            agent = target as RQ2.AnimationV2.SpriteAnimationsConfig;
            //_sceneSetup = GameObject.FindObjectOfType<SceneSetup>();
        }

        //public void OnSceneGUI()
        //{
        //    var ev = Event.current;
        //}

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            base.OnInspectorGUI();

            var spriteAnimationTypes = agent.GetStoredSpriteAnimations();
            //if (spriteAnimationTypes != null)

            //if (agent.)

            var clipNames = new List<KeyValuePair<string, string>>();

            clipNames.Add(new KeyValuePair<string, string>("", "Select Animation..."));

            var allClips = agent.GetAllClipNames();
            if (allClips.Any())
            {
                foreach (var clip in allClips.OrderBy(i => i))
                {
                    clipNames.Add(new KeyValuePair<string, string>(clip, clip));
                }

                //if (GUILayout.Button("Add Animation Type", GUILayout.Width(130)))
                //{
                //    //spriteAnimations.Add(new Animation.Common.SpriteAnimation());
                //    var newType = new SpriteAnimationType(); //KeyValuePair<string, List<SpriteAnimation>>(string.Empty, new List<SpriteAnimation>());
                //    newType.ID = Guid.NewGuid().ToString();
                //    newType.SpriteAnimations = new List<SpriteAnimation>();
                //    spriteAnimationTypes.Add(newType);
                //}

                ListEditor.DrawList(spriteAnimationTypes, (spriteAnimationType, index) =>
                {
                    GUILayout.Space(4.0f);
                    GUILayout.BeginVertical(tk2dEditorSkin.GetStyle("InspectorHeaderBG"));
                    spriteAnimationType.Type = EditorGUILayout.TextField("Type", spriteAnimationType.Type);
                    EditorGUILayout.LabelField("ID", spriteAnimationType.ID);

                    ListEditor.DrawList(spriteAnimationType.SpriteAnimations, (spriteAnimation, itemindex) =>
                    {
                        spriteAnimation.Direction = (Direction)EditorGUILayout.EnumPopup(spriteAnimation.Direction, GUILayout.Width(75));

                        spriteAnimation.AnimationName = ControlExtensions.Popup(String.Empty, spriteAnimation.AnimationName, clipNames, GUILayout.Width(200));

                        if (String.IsNullOrEmpty(spriteAnimation.AnimationName))
                        {
                            spriteAnimation.ClipId = -1;
                        }
                        else
                        {
                            spriteAnimation.ClipId = agent.GetClipIdByName(spriteAnimation.AnimationName);
                        }
                        //return spriteAnimation.AnimationName;
                        return new ListEditor.DrawItemData<SpriteAnimation>
                        {
                            name = spriteAnimation.AnimationName,
                            data = spriteAnimation
                        };
                    });

                    GUILayout.EndVertical();
                    //return spriteAnimationType.Type;
                    return new ListEditor.DrawItemData<SpriteAnimationType>
                    {
                        name = spriteAnimationType.Type,
                        data = spriteAnimationType
                    };
                },
                () =>
                {
                    var newType = new SpriteAnimationType(); //KeyValuePair<string, List<SpriteAnimation>>(string.Empty, new List<SpriteAnimation>());
                    newType.ID = Guid.NewGuid().ToString();
                    newType.SpriteAnimations = new List<SpriteAnimation>();
                    return newType;
                });

                //int removeAnimationTypeIndex = -1;

                //for (int i = 0; i < spriteAnimationTypes.Count(); i++)
                //{
                //    GUILayout.Space(4.0f);
                //    GUILayout.BeginVertical(tk2dExternal.Skin.Inst.GetStyle("InspectorHeaderBG"));

                //    var spriteAnimationType = spriteAnimationTypes[i];
                //    EditorGUILayout.BeginHorizontal();
                //    //EditorGUILayout.LabelField("Animation", GUILayout.Width(75));
                //    //spriteAnimation.Key = 
                //    //var newKey = EditorGUILayout.TextField("Type", spriteAnimationType.Key);
                //    spriteAnimationType.Type = EditorGUILayout.TextField("Type", spriteAnimationType.Type);

                //    //if (GUILayout.Button("", Utils.GetStyle("TilemapDeleteItem")))
                //    //{
                //    //    if (EditorUtility.DisplayDialog("Remove animation type?", "Remove animation type " + spriteAnimationType.Type + "?", "Yes", "No"))
                //    //    {
                //    //        removeAnimationTypeIndex = i;
                //    //    }
                //    //}
                //    EditorGUILayout.EndHorizontal();
                //    EditorGUILayout.LabelField("ID", spriteAnimationType.ID);

                //    //if (GUILayout.Button("Add Animation", GUILayout.Width(130)))
                //    //{
                //    //    //spriteAnimations.Add(new Animation.Common.SpriteAnimation());
                //    //    var newAnimation = new SpriteAnimation(); //KeyValuePair<string, List<SpriteAnimation>>(string.Empty, new List<SpriteAnimation>());
                //    //    spriteAnimationType.SpriteAnimations.Add(newAnimation);
                //    //}

                //    ListEditor.DrawList(spriteAnimationType.SpriteAnimations, (spriteAnimation) =>
                //    {
                //        spriteAnimation.Direction = (Direction)EditorGUILayout.EnumPopup(spriteAnimation.Direction, GUILayout.Width(75));

                //        spriteAnimation.AnimationName = ControlExtensions.Popup(String.Empty, spriteAnimation.AnimationName, clipNames, GUILayout.Width(200));

                //        if (String.IsNullOrEmpty(spriteAnimation.AnimationName))
                //        {
                //            spriteAnimation.ClipId = -1;
                //        }
                //        else
                //        {
                //            spriteAnimation.ClipId = agent.GetClipIdByName(spriteAnimation.AnimationName);
                //        }
                //        return spriteAnimation.AnimationName;
                //    });
                //    //},
                //    //() =>
                //    //{
                //    //    return new EntityPrefab()
                //    //    {
                //    //        UniqueId = Guid.NewGuid().ToString()
                //    //    };
                //    //});

                //    //var removeAnimationIndex = -1;

                //    //for (int k = 0; k < spriteAnimationType.SpriteAnimations.Count; k++)
                //    //{
                //    //    EditorGUILayout.BeginHorizontal();
                //    //    var spriteAnimation = spriteAnimationType.SpriteAnimations[k];

                //    //    spriteAnimation.Direction = (Direction)EditorGUILayout.EnumPopup(spriteAnimation.Direction, GUILayout.Width(75));

                //    //    spriteAnimation.AnimationName = ControlExtensions.Popup(String.Empty, spriteAnimation.AnimationName, clipNames, GUILayout.Width(200));

                //    //    if (String.IsNullOrEmpty(spriteAnimation.AnimationName))
                //    //    {
                //    //        spriteAnimation.ClipId = -1;
                //    //    }
                //    //    else
                //    //    {
                //    //        spriteAnimation.ClipId = agent.GetClipIdByName(spriteAnimation.AnimationName);
                //    //    }
                //    //    if (GUILayout.Button("", Utils.GetStyle("TilemapDeleteItem")))
                //    //    {
                //    //        if (EditorUtility.DisplayDialog("Remove animation?", "Remove animation " + spriteAnimation.AnimationName + "?", "Yes", "No"))
                //    //        {
                //    //            removeAnimationIndex = k;
                //    //        }
                //    //    }

                //    //    EditorGUILayout.EndHorizontal();
                //    //}

                //    //if (removeAnimationIndex != -1)
                //    //    spriteAnimationType.SpriteAnimations.RemoveAt(removeAnimationIndex);

                //    GUILayout.EndVertical();
                //}

                //if (removeAnimationTypeIndex != -1)
                //    spriteAnimationTypes.RemoveAt(removeAnimationTypeIndex);

            }

            if (GUI.changed)
            {
                Dirty();
            }
        }
    }
}
