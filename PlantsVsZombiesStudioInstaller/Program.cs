using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PlantsVsZombiesStudioInstaller
{
    class Program
    {
        static void Main()
        {
            Process[] processes = Process.GetProcessesByName("PlantsVsZombiesStudio");

            if (processes.Any())
            {
                foreach(Process item in processes)
                {
                    item.WaitForExit();
                }
            }

            foreach (FileInfo item in EnumerateFiles(Directory.GetCurrentDirectory()))
            {
                if (item.FullName.EndsWith(".update"))
                {
                    string path = item.FullName.Replace(".update", string.Empty);
                    File.Delete(path);
                    File.Move(item.FullName, path); 
                }
            }

            Process.Start("PlantsVsZombiesStudio.exe");
            static FileInfo[] EnumerateFiles(string path)
            {
                List<FileInfo> files = new();
                DirectoryInfo directoryInfo = new(path);
                files.AddRange(directoryInfo.GetFiles());

                foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
                {
                    files.AddRange(EnumerateFiles(directory.FullName));
                }

                return files.ToArray();
            }
        }
    }
}
