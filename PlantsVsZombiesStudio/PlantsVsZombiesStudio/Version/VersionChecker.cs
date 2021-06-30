using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

using Newtonsoft.Json;

namespace PlantsVsZombiesStudio
{
    public class VersionChecker
    {
        private const string Address = "https://plants-vs-zombies-studio-1256953837.cos.ap-chengdu.myqcloud.com/Storage/sha512.json";

        public static Dictionary<string, string> GetExpiredFiles()
        {
            Dictionary<string, string> expiredFiles = new();
            HttpClient httpClient = new();
            string json = httpClient.GetStringAsync(Address).GetAwaiter().GetResult();
            Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            foreach (KeyValuePair<string, string> item in dictionary)
            {
                if (!File.Exists(item.Key) || HashTool.EncryptSha512(File.OpenRead(item.Key)) != item.Value)
                {
                    expiredFiles.Add(item.Key, item.Value);
                }
            }

            return expiredFiles;
        }
    }
}
