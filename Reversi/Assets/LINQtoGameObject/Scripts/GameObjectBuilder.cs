using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
   
    public class GameObjectBuilder
    {
        readonly GameObject original;
        readonly IEnumerable<GameObjectBuilder> children;

        
        public GameObjectBuilder(GameObject original, params GameObjectBuilder[] children)
            : this(original, (IEnumerable<GameObjectBuilder>)children)
        {

        }

       
        public GameObjectBuilder(GameObject original, IEnumerable<GameObjectBuilder> children)
        {
            this.original = original;
            this.children = children;
        }

        
        public GameObject Instantiate()
        {
            return Instantiate(TransformCloneType.KeepOriginal);
        }

       
        public GameObject Instantiate(bool setActive)
        {
            return Instantiate(TransformCloneType.KeepOriginal);
        }

       
        public GameObject Instantiate(TransformCloneType cloneType)
        {
            var root = UnityEngine.Object.Instantiate(original) as GameObject;
            InstantiateChildren(root, cloneType, null);
            return root;
        }

       
        public GameObject Instantiate(TransformCloneType cloneType, bool setActive)
        {
            var root = UnityEngine.Object.Instantiate(original) as GameObject;
            InstantiateChildren(root, cloneType, setActive);
            return root;
        }

        void InstantiateChildren(GameObject root, TransformCloneType cloneType, bool? setActive)
        {
            foreach (var child in children)
            {
                var childRoot = root.Add(child.original, cloneType, setActive);
                child.InstantiateChildren(childRoot, cloneType, setActive);
            }
        }
    }
}