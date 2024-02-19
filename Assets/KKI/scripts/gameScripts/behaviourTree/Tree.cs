using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour, ILoadable
    {
        
        protected Node _root = null;

        public abstract void Init();

        public abstract void RestartTree();
        
        public void StopTree()
        {
            _root = null;
        }
        public abstract void SetupTree();
    }
}
