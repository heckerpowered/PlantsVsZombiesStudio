using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PlantsVsZombiesStudio
{
    public class PVZVersion
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PVZVersionType Type { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string Version { get; set; }
        public string FileName { get; set; }
    }
    public enum PVZVersionType
    {
        Original,
        Modified
    }
    public static class AutoInstaller
    {
        public static IEnumerable<PVZVersion> GetPVZVersions()
        {
            HttpClient httpClient = new();
            string json = httpClient.GetStringAsync("https://plants-vs-zombies-studio-1256953837.cos.ap-chengdu.myqcloud.com/Storage/game/versions.json").GetAwaiter().GetResult();
            List<PVZVersion> versions = JsonConvert.DeserializeObject<List<PVZVersion>>(json);
            return versions;
        }
    }
}
