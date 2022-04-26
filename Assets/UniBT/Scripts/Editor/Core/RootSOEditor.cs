using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniBT.Editor
{
    [CustomEditor(typeof(RootSO))]
    public class RootSOEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Behavior Tree"))
            {
                var rootSO = target as RootSO;
                GraphEditorWindow.Show(rootSO);
            }
        }
    }
}