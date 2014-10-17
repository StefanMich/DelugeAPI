using System;
using System.IO;

namespace DelugeAPI
{
    public static class DelugePathInfo
    {
        private static bool isSet;
        private static string directory;

        public static bool SetPath(string directory)
        {
            if (File.Exists(Path.Combine(directory, "deluge.exe")) &&
                File.Exists(Path.Combine(directory, "deluged.exe")) &&
                File.Exists(Path.Combine(directory, "deluge-console.exe")))
            {
                DelugePathInfo.directory = directory;
                return true;
            }
            return false;
        }

        public static string Directory
        {
            get
            {
                if (directory == null)
                    throw new NullReferenceException("No Deluge directory defined.");
                return directory;
            }
        }

        public static string DelugeConsoleExe
        {
            get { return Path.Combine(Directory, "deluge-console.exe"); }
        }
        public static string DelugedExe
        {
            get { return Path.Combine(Directory, "deluged.exe"); }
        }
    }
}
