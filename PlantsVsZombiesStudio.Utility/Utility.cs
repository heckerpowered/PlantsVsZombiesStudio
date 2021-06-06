using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVsZombiesStudio.Utility
{
    public static class Utility
    {
        public static readonly Version CURRENT_VERSION = new Version(1, 0, 0, 0);
        public static readonly Uri REMOTE_VERSION_URI = new Uri("https://link.jscdn.cn/sharepoint/aHR0cHM6Ly9xMDYtbXkuc2hhcmVwb2ludC5jb20vOnU6L2cvcGVyc29uYWwvbGVzbGllYV9vZF8zNjVwcHRfYXJ0L0VSS1NDaWF0QVJaTGg4Ty1pQ3I5US1VQjE5T3djOVdDc3V0bWRTajg2MmhOYXc_ZT1FY2pUeEg");
        public static Version GetRemoteVersion()
        {
            WebRequest Request = WebRequest.Create(REMOTE_VERSION_URI);
            Request.Method = "GET";
            Request.ContentType = "text/plain";
            Request.ContentLength = 0;
            WebResponse Response = Request.GetResponse();
            StreamReader Reader = new StreamReader(Response.GetResponseStream());
            return Version.Parse(Reader.ReadToEnd());
        }
        public static bool VerifyRemoteVersion(Version Version)
        {
            return CURRENT_VERSION.Equals(Version);
        }
    }
}
