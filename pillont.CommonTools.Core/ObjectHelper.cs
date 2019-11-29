using Newtonsoft.Json;

namespace pillont.CommonTools.Core
{
    public static class ObjectHelper
    {
        public static T JsonClone<T>(this T value)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
        }
    }
}