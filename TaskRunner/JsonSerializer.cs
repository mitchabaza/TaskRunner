using Dance;
using Newtonsoft.Json;

namespace ScheduledTaskRunner
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T arg)
        {
            return JsonConvert.SerializeObject(arg);
        }

        public T Deserialize<T>(string arg)
        {
            return JsonConvert.DeserializeObject<T>(arg);
        }
    }
}