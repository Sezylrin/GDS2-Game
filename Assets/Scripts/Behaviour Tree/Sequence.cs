using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        public override NodeState Evaluate()
        {
            NodeState temp = NodeState.success;
            foreach (Node node in children)
            {
                temp = node.Evaluate();
                if (temp == NodeState.failure)
                    break;
            }
            return temp;
        }
    }
}

