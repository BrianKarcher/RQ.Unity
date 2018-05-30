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
using RQ.UI;
using RQ2.UI;

namespace RQ.Editor
{
    [CustomEditor(typeof(UIManager), true)]
    public class UIManagerEditor : EditorBase
    {
        private UIManager agent;

        public void OnEnable()
        {
            agent = target as UIManager;
        }

        //public void OnSceneGUI()
        //{
        //    var ev = Event.current;
        //}

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            base.OnInspectorGUI();

            NGUIEditorTools.DrawEvents("On Click Save", agent, agent.onClickSave, false);
            NGUIEditorTools.DrawEvents("On Click New Save", agent, agent.onClickNewSave, false);
            NGUIEditorTools.DrawEvents("On Click Load", agent, agent.onClickLoad, false);

            if (GUI.changed)
            {
                Dirty();
            }
        }
    }
}
