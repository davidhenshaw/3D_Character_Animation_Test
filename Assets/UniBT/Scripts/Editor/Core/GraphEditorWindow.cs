using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniBT.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        // GraphView window per GameObject
        private static readonly Dictionary<int,GraphEditorWindow> cache = new Dictionary<int, GraphEditorWindow>();

        /// <summary>
        /// The instance ID of the object that created this window.
        /// This is not a reference so that ScriptableObjects are allowed to create windows
        /// </summary>
        private int key { get; set; }

        public static void Show(BehaviorTree bt)
        {
            var window = Create(bt);
            window.Show();
            window.Focus();
        }

        public static void Show(RootSO rootSO)
        {
            var window = Create(rootSO);
            window.Show();
            window.Focus();
        }

        private static GraphEditorWindow Create(BehaviorTree bt)
        {
            var key = bt.gameObject.GetInstanceID();
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var window = CreateInstance<GraphEditorWindow>();
            StructGraphView(window, bt);
            window.titleContent = new GUIContent($"BehaviorTree Editor({bt.gameObject.name})");
            window.key = key;
            cache[key] = window;
            return window;
        }

        private static GraphEditorWindow Create(RootSO root)
        {
            var key = root.GetInstanceID();
            if (cache.ContainsKey(key))
            {
                return cache[key];
            }

            var window = CreateInstance<GraphEditorWindow>();
            StructGraphView(window, root);
            window.titleContent = new GUIContent($"BehaviorTree Editor({root.name})");
            window.key = key;
            cache[key] = window;
            return window;
        }

        private static void StructGraphView(GraphEditorWindow window, BehaviorTree behaviorTree)
        {
            window.rootVisualElement.Clear();
            var graphView = new BehaviorTreeView(behaviorTree, window);
            graphView.Restore();
            window.rootVisualElement.Add(window.CreateToolBar(graphView));
            window.rootVisualElement.Add(graphView);
        }

        private static void StructGraphView(GraphEditorWindow window, RootSO behaviorTree)
        {
            window.rootVisualElement.Clear();
            var graphView = new RootSOTreeView(behaviorTree, window);
            graphView.Restore();
            window.rootVisualElement.Add(window.CreateToolBar(graphView));
            window.rootVisualElement.Add(graphView);
        }

        private void OnDestroy()
        {
            if (key != null && cache.ContainsKey(key))
            {
                cache.Remove(key);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            switch (playModeStateChange)
            {
                case PlayModeStateChange.EnteredEditMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingEditMode:
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    Reload();
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playModeStateChange), playModeStateChange, null);
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Reload();
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void Reload()
        {
            if (key != null)
            {
                var obj = EditorUtility.InstanceIDToObject(key);

                switch(obj)
                {
                    case RootSO so:
                        StructGraphView(this, so);
                        break;
                    case BehaviorTree tree:
                        StructGraphView(this, tree);
                        break;
                }
                Repaint();
            }
        }

        private VisualElement CreateToolBar(RootSOTreeView graphView)
        {
            return new IMGUIContainer(
                () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            var guiContent = new GUIContent();
                            if (graphView.Save())
                            {
                                guiContent.text = "Successfully updated.";
                                this.ShowNotification(guiContent);
                            }
                            else
                            {
                                guiContent.text = "Invalid tree. one or mode nodes have error.";
                                this.ShowNotification(guiContent);
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            );

        }

        private VisualElement CreateToolBar(BehaviorTreeView graphView)
        {
            return new IMGUIContainer(
                () =>
                {
                    GUILayout.BeginHorizontal(EditorStyles.toolbar);

                    if (!Application.isPlaying)
                    {
                        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
                        {
                            var guiContent = new GUIContent();
                            if (graphView.Save())
                            {
                                guiContent.text = "Successfully updated.";
                                this.ShowNotification(guiContent);
                            }
                            else
                            {
                                guiContent.text = "Invalid tree. one or mode nodes have error.";
                                this.ShowNotification(guiContent);
                            }
                        }
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            );

        }



    }
}