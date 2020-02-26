using System.Collections.Generic;
using UnityEngine;

namespace Unity.Linq
{
    public static partial class GameObjectExtensions
    {
       
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Ancestors())
                {
                    yield return item2;
                }
            }
        }

       
        public static IEnumerable<GameObject> Ancestors(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Ancestors(name))
                {
                    yield return item2;
                }
            }
        }

        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.AncestorsAndSelf())
                {
                    yield return item2;
                }
            }
        }

      
        public static IEnumerable<GameObject> AncestorsAndSelf(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.AncestorsAndSelf(name))
                {
                    yield return item2;
                }
            }
        }

       
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Descendants())
                {
                    yield return item2;
                }
            }
        }

       
        public static IEnumerable<GameObject> Descendants(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Descendants(name))
                {
                    yield return item2;
                }
            }
        }

       
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.DescendantsAndSelf())
                {
                    yield return item2;
                }
            }
        }

        
        public static IEnumerable<GameObject> DescendantsAndSelf(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.DescendantsAndSelf(name))
                {
                    yield return item2;
                }
            }
        }

       
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Children())
                {
                    yield return item2;
                }
            }
        }

       
        public static IEnumerable<GameObject> Children(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.Children(name))
                {
                    yield return item2;
                }
            }
        }

        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.ChildrenAndSelf())
                {
                    yield return item2;
                }
            }
        }

        
        public static IEnumerable<GameObject> ChildrenAndSelf(this IEnumerable<GameObject> source, string name)
        {
            foreach (var item in source)
            {
                foreach (var item2 in item.ChildrenAndSelf(name))
                {
                    yield return item2;
                }
            }
        }

       
        public static void Destroy(this IEnumerable<GameObject> source, bool useDestroyImmediate = false)
        {
            foreach (var item in new List<GameObject>(source)) 
            {
                item.Destroy(useDestroyImmediate);
            }
        }

       
        public static IEnumerable<T> OfComponent<T>(this IEnumerable<GameObject> source)
            where T : UnityEngine.Component
        {
            foreach (var item in source)
            {
                var component = item.GetComponent<T>();
                if (component != null)
                {
                    yield return component;
                }
            }
        }
    }
}