using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    
    public static partial class GameObjectExtensions
    {
       
        public static GameObject Parent(this GameObject origin)
        {
            if (origin == null) return null;

            var parentTransform = origin.transform.parent;
            if (parentTransform == null) return null;

            return parentTransform.gameObject;
        }

        
        public static GameObject Child(this GameObject origin, string name)
        {
            if (origin == null) return null;
            
            var child = origin.transform.FindChild(name);
            if (child == null) return null;
            return child.gameObject;
        }

       
        public static IEnumerable<GameObject> Children(this GameObject origin)
        {
            return ChildrenCore(origin, nameFilter: null, withSelf: false);
        }

       
        public static IEnumerable<GameObject> Children(this GameObject origin, string name)
        {
            return ChildrenCore(origin, nameFilter: name, withSelf: false);
        }


       
        public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin)
        {
            return ChildrenCore(origin, nameFilter: null, withSelf: true);
        }

        
        public static IEnumerable<GameObject> ChildrenAndSelf(this GameObject origin, string name)
        {
            return ChildrenCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> ChildrenCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform child in origin.transform)
            {
                if (nameFilter == null || child.name == nameFilter)
                {
                    yield return child.gameObject;
                }
            }
        }

       
        public static IEnumerable<GameObject> Ancestors(this GameObject origin)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: false);
        }

       
        public static IEnumerable<GameObject> Ancestors(this GameObject origin, string name)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: false);
        }

       
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin)
        {
            return AncestorsCore(origin, nameFilter: null, withSelf: true);
        }

        
        public static IEnumerable<GameObject> AncestorsAndSelf(this GameObject origin, string name)
        {
            return AncestorsCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> AncestorsCore(GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parentTransform = origin.transform.parent;
            while (parentTransform != null)
            {
                if (nameFilter == null || parentTransform.name == nameFilter)
                {
                    yield return parentTransform.gameObject;
                }
                parentTransform = parentTransform.parent;
            }
        }

       
        public static IEnumerable<GameObject> Descendants(this GameObject origin)
        {
            return DescendantsCore(origin, nameFilter: null, withSelf: false);
        }

       
        public static IEnumerable<GameObject> Descendants(this GameObject origin, string name)
        {
            return DescendantsCore(origin, nameFilter: name, withSelf: false);
        }


        
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin)
        {
            return DescendantsCore(origin, nameFilter: null, withSelf: true);
        }

        
        public static IEnumerable<GameObject> DescendantsAndSelf(this GameObject origin, string name)
        {
            return DescendantsCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> DescendantsCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            foreach (Transform item in origin.transform)
            {
                foreach (var child in DescendantsCore(item.gameObject, nameFilter, withSelf: true))
                {
                    if (nameFilter == null || child.name == nameFilter)
                    {
                        yield return child.gameObject;
                    }
                }
            }
        }

        
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, nameFilter: null, withSelf: false);
        }

         
        public static IEnumerable<GameObject> BeforeSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, nameFilter: name, withSelf: false);
        }

        
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin)
        {
            return BeforeSelfCore(origin, nameFilter: null, withSelf: true);
        }

             
        public static IEnumerable<GameObject> BeforeSelfAndSelf(this GameObject origin, string name)
        {
            return BeforeSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> BeforeSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;

            var parent = origin.transform.parent;
            if (parent == null) goto RETURN_SELF;

            foreach (Transform item in parent.transform)
            {
                var go = item.gameObject;
                if (go == origin)
                {
                    goto RETURN_SELF;
                }

                if (nameFilter == null || item.name == nameFilter)
                {
                    yield return go;
                }
            }

        RETURN_SELF:
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }
        }

        
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, nameFilter: null, withSelf: false);
        }

         
        public static IEnumerable<GameObject> AfterSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, nameFilter: name, withSelf: false);
        }

       
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin)
        {
            return AfterSelfCore(origin, nameFilter: null, withSelf: true);
        }

           
        public static IEnumerable<GameObject> AfterSelfAndSelf(this GameObject origin, string name)
        {
            return AfterSelfCore(origin, nameFilter: name, withSelf: true);
        }

        static IEnumerable<GameObject> AfterSelfCore(this GameObject origin, string nameFilter, bool withSelf)
        {
            if (origin == null) yield break;
            if (withSelf && (nameFilter == null || origin.name == nameFilter))
            {
                yield return origin;
            }

            var parent = origin.transform.parent;
            if (parent == null) yield break;

            var index = origin.transform.GetSiblingIndex() + 1;
            var parentTransform = parent.transform;
            var count = parentTransform.childCount;

            while (index < count)
            {
                var target = parentTransform.GetChild(index).gameObject;
                if (nameFilter == null || target.name == nameFilter)
                {
                    yield return target;
                }

                index++;
            }
        }
    }
}