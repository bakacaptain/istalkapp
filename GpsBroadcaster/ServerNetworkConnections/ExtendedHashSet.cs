using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ServerNetworkConnections
{
    /// <summary>
    /// Just a HashSet I added some extra functionality to, so to avoid having it in my logic in controller classes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExtendedHashSet<T> : HashSet<T>
    {
        /// <summary>
        /// Removes all elements from the hash set
        /// </summary>
        /// <param name="elements"></param>
        public void RemoveAll(IEnumerable<T> elements)
        {
            foreach (T t in elements)
            {
                Remove(t);
            }
        }

        /// <summary>
        /// Adds all elements to the hash set
        /// </summary>
        /// <param name="elements"></param>
        public void AddAll(IEnumerable<T> elements)
        {
            foreach (T t in elements)
            {
                Add(t);
            }
        }

        /// <summary>
        /// Utility method to easily create a hash set with given elements.
        /// </summary>
        /// <param name="elements">The elements that the hash set should be created with</param>
        /// <returns>The created hash set</returns>
        public static ExtendedHashSet<T> CreateSetWith(IEnumerable<T> elements)
        {
            ExtendedHashSet<T> ret = new ExtendedHashSet<T>();
            ret.AddAll(elements);
            return ret;
        }

        /// <summary>
        /// Utility method to easily create a hash set with the given elements.
        /// 
        /// Since it creates a hash set of strings, the ToString() method of all
        /// elements is called to create the elements of the set.
        /// </summary>
        /// <param name="elements">The elements that the hash set should be created with</param>
        /// <returns>The created hash set</returns>
        public static ExtendedHashSet<String> CreateSetWith(IEnumerable elements)
        {
            ExtendedHashSet<String> ret = new ExtendedHashSet<String>();
            foreach (T t in elements)
            {
                ret.Add(t.ToString());
            }
            return ret;
        }

        /// <summary>
        /// Utility method to easilt create a hash set with the given element.
        /// </summary>
        /// <param name="t">The element to be initially added to the hash set</param>
        /// <returns>The created hash set</returns>
        public static ExtendedHashSet<T> CreateSetWith(T t)
        {
            ExtendedHashSet<T> ret = new ExtendedHashSet<T>();
            ret.Add(t);
            return ret;
        }
    }
}
