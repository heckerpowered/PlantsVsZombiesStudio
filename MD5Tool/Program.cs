using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MD5Tool
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                string line = Console.ReadLine();

                if (line == null)
                {
                    return;
                }

                if (File.Exists(line))
                {
                    using FileStream fileStream = new(line, FileMode.Open, FileAccess.Read);
                    Console.WriteLine(HashTool.EncryptSha512(fileStream));
                    fileStream.Close();
                }
                else
                {
                    if (Directory.Exists(line))
                    {
                        Dictionary<string, string> dictionary = new();

                        FileInfo[] files = EnumerateFiles(line);
                        foreach (FileInfo file in files)
                        {
                            dictionary.Add(file.FullName.Replace(line, string.Empty).TrimStart('\\'), HashTool.EncryptSha512(File.OpenRead(file.FullName)));
                        }

                        File.WriteAllText(@"sha512.json",JsonConvert.SerializeObject(dictionary));

                        static FileInfo[] EnumerateFiles(string path)
                        {
                            List<FileInfo> files = new();
                            DirectoryInfo directoryInfo = new(path);
                            files.AddRange(directoryInfo.GetFiles());

                            foreach(DirectoryInfo directory in directoryInfo.GetDirectories())
                            {
                                files.AddRange(EnumerateFiles(directory.FullName));
                            }

                            return files.ToArray();
                        }
                    }
                    else
                    {
                        Console.WriteLine("File and directory not exists.");
                    }
                }
            }
        }  

        private string GetJsonValue(JToken node, string v1, string v2)
        {
            throw new NotImplementedException();
        }
    }
    public class HashTool
    {
        public static string EncryptSha512(Stream stream)
        {
            SHA512 sha512 = SHA512.Create();
            return BitConverter.ToString(sha512.ComputeHash(stream)).Replace("-", string.Empty);
        }
    }
}
