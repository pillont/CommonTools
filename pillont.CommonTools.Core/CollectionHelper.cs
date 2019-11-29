using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pillont.CommonTools.Core
{
    public static class CollectionHelper
    {
        public static bool AllAreDistinct<T>(this IEnumerable<T> p_List, IEqualityComparer<T> p_Comparer)
        {
            if (p_List == null)
                throw new NullReferenceException("list are null");

            if (p_Comparer == null)
                throw new ArgumentNullException(nameof(p_Comparer));

            return p_List.Distinct(p_Comparer).Count() == p_List.Count();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> p_Source, Func<TSource, TKey> p_KeySelector)
        {
            HashSet<TKey> v_SeenKeys = new HashSet<TKey>();
            foreach (TSource v_Element in p_Source)
            {
                var v_Key = p_KeySelector(v_Element);
                bool v_KeyDidntExists = v_SeenKeys.Add(v_Key);

                if (v_KeyDidntExists)
                {
                    yield return v_Element;
                }
            }
        }

        public static bool IsNullOrEmpty(this IEnumerable collection)
        {
            return collection == null
                            || !collection.GetEnumerator().MoveNext();
        }

        /// <summary>
        /// collect the first key associated to the argument predicate
        /// </summary>
        /// <param name="p_Predicate">predicate to select the value</param>
        public static TKey KeyByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> p_Source, Func<TValue, bool> p_Predicate)
        {
            var v_Pair = p_Source.PairsByValue(p_Predicate).FirstOrDefault();

            return v_Pair.Equals(default(TKey)) == false
                            ? v_Pair.Key
                            : default(TKey);
        }

        /// <summary>
        /// collect the first key associated to the argument value
        /// </summary>
        /// <param name="p_Wanted">value to find</param>
        public static TKey KeyByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> p_Source, TValue p_Wanted)
        {
            var v_Pair = p_Source.PairsByValue(p_Wanted).FirstOrDefault();

            return v_Pair.Equals(default(TKey)) == false
                        ? v_Pair.Key
                        : default(TKey);
        }

        /// <summary>
        /// collect the first pair associated to the argument predicate
        /// </summary>
        /// <param name="p_Predicate"> Predicate to find value </param>
        public static KeyValuePair<TKey, TValue> PairByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> p_Source, Func<TValue, bool> p_Predicate)
        {
            return p_Source.PairsByValue(p_Predicate).FirstOrDefault();
        }

        /// <summary>
        /// collect the first pair associated to the argument value
        /// </summary>
        /// <param name="p_Wanted"> value to find</param>
        public static KeyValuePair<TKey, TValue> PairByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> p_Source, TValue p_Wanted)
        {
            return p_Source.PairsByValue(p_Wanted).FirstOrDefault();
        }

        /// <summary>
        /// collect all the pairs associated to the argument Predicate
        /// </summary>
        /// <param name="p_Predicate"> Predicate to find the value </param>
        public static IEnumerable<KeyValuePair<TKey, TValue>> PairsByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> p_Source, Func<TValue, bool> p_Predicate)
        {
            return p_Source.Where(p_Pair => p_Predicate(p_Pair.Value));
        }

        /// <summary>
        /// collect all the pairs associated to the argument value
        /// </summary>
        /// <param name="p_Wanted"> value to find</param>
        public static IEnumerable<KeyValuePair<TKey, TValue>> PairsByValue<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> p_Source, TValue p_Wanted)
        {
            return p_Source.Where(p_Pair => p_Wanted.Equals(p_Pair.Value));
        }
    }
}