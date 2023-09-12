using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class BehaviourTree : MonoBehaviour
    {

        private Node _root = null;
        protected void Start()
        {
            _root = SetUpTree();

        }

        void Update()
        {
            _root.Evaluate();
        }

        

        protected abstract Node SetUpTree();
    }
}

