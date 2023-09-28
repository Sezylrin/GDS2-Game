using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeState
    {
        running,
        success,
        failure
    }
    public class Node
    {
        public Node parent;
        protected List<Node> children = new List<Node>();
        public Node()
        {
            parent = null;
        }
        public Node(List<Node> children)
        {
            foreach(Node node in children)
            {
                _Attach(node);
            }
        }

        private void _Attach(Node node)
        {
            node.parent = this;
            children.Add(this);
        }
 
        public virtual NodeState Evaluate() => NodeState.failure;
    }
}

