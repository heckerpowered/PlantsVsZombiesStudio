using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlantsVsZombiesStudio
{
    public class HashTool
    {
        public static string EncryptSha512(Stream stream)
        {
            SHA512 sha512 = SHA512.Create();
            return BitConverter.ToString(sha512.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
