using System.Collections.Generic;
using System.Linq;

namespace pillont.CommonTools.Core.Enumerables
{
    public static class DictionariesHelper
    {
        public static IDictionary<TKey, TValue> ConcatDictionaries<TKey, TValue>(params Dictionary<TKey, TValue>[] dictionnaries)
        {
            var result = dictionnaries.Select(dict => dict.AsNotNull())
                                      .SelectMany(pair => pair)
                                      .ToDictionary(pair => pair.Key,
                                                   pair => pair.Value);
            return result;
        }
    }
}