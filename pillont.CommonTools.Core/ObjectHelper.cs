using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace pillont.CommonTools.Core
{
    public static class ObjectHelper
    {
        private static MethodInfo MemberwiseCloneFnc { get; }

        static ObjectHelper()
        {
            MemberwiseCloneFnc = RuntimeReflectionExtensions
                .GetRuntimeMethods(typeof(object))
                .First((MethodInfo m) => m.Name == "MemberwiseClone");
        }

        public static T Clone<T>(this T value)
        {
            return (T)MemberwiseCloneFnc.Invoke(value, new object[0]);
        }

        public static List<T> CloneList<T>(this IEnumerable<T> value)
        {
            return value.Select((T v) => Clone(v)).ToList();
        }

        public static T JsonClone<T>(this T value)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
        }
    }
}