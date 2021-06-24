using System;
using System.Net;
using System.Threading.Tasks;

namespace PlantsVsZombiesStudio
{
    public class VersionHelper
    {
        public static readonly Version Version = new(1, 1, 0);
        public const string RemoteUrl = "https://gitee.com/pvzstudio/pvz-studio/raw/master/pvzstudio/version.txt";
        public const string FileUrl = "https://gitee.com/pvzstudio/pvz-studio/raw/master/pvzstudio/net5.0-windows.rar";
        public static bool CheckUpdate()
        {
            return CheckUpdateAsync().Result;
        }
        private static async Task<bool> CheckUpdateAsync()
        {
            return await Task.Factory.StartNew(delegate
            {
                var client = new WebClient();
                var version = new Version(client.DownloadString(RemoteUrl));
                return !version.Equals(Version);
            });
        }
    }
}
