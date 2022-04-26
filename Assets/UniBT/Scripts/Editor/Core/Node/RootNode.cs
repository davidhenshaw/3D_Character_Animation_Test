using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace UniBT.Editor
{
    public sealed class RootNode : BehaviorTreeNode
    {
        public readonly Port Child;

        private BehaviorTreeNode cache;

        public RootNode() 
        {
            SetBehavior(typeof(Root));
            title = "Root";
            Child = CreateChildPort();
            outputContainer.Add(Child);

            RootSO rootSO = ScriptableObject.CreateInstance<RootSO>();
            rootSO.name = "New Behavior Root";
        }

        protected override void AddParent()
        {
        }

        protected override void AddDescription()
        {
        }

        protected override void OnRestore()
        {
            (NodeBehavior as Root).UpdateEditor = ClearStyle;
        }

        protected override bool OnValidate(Stack<BehaviorTreeNode> stack)
        {
            if (!Child.connected)
            {
                return false;
            }
            stack.Push(Child.connections.First().input.node as BehaviorTreeNode);
            return true;
        }
        protected override void OnCommit(Stack<BehaviorTreeNode> stack)
        {
            var child = Child.connections.First().input.node as BehaviorTreeNode;
            var newRoot = new Root();
            newRoot.Child = child.ReplaceBehavior();
            newRoot.UpdateEditor = ClearStyle;
            NodeBehavior = newRoot;
            stack.Push(child);
            cache = child;
        }

        public void PostCommit(BehaviorTree tree)
        {
            tree.Root = (NodeBehavior as Root); 
        }

        public void PostCommit(RootSO rootSO)
        {
            rootSO.SetRoot(NodeBehavior as Root);
        }

        protected override void OnClearStyle()
        {
            cache?.ClearStyle();
        }
    }
}