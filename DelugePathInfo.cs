using System;
using System.IO;

namespace DelugeAPI
{
    /// <summary>
    /// Provides methods for retrieving path to deluge executables.
    /// The <see cref="SetPath"/> method must be called before any of the other methods.
    /// </summary>
    public static class DelugePathInfo
    {
        private static bool isSet;
        private static string directory;

        /// <summary>
        /// Sets the path of the directory where the deluge executables are located.
        /// This must be done before accessing any of the other properties or methods in this class.
        /// </summary>
        /// <param name="directory">The directory where the deluge executables are located.</param>
        /// <returns><c>true</c> if the path was successfully set (if the required executables were found); otherwise <c>false</c>.</returns>
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

        /// <summary>
        /// Gets the path to the directory containing the deluged executables.
        /// </summary>
        public static string Directory
        {
            get
            {
                if (directory == null)
                    throw new NullReferenceException("No Deluge directory defined.");
                return directory;
            }
        }

        /// <summary>
        /// Gets the path to the deluge-console executable.
        /// </summary>
        public static string DelugeConsoleExe
        {
            get { return Path.Combine(Directory, "deluge-console.exe"); }
        }
        /// <summary>
        /// Gets the path to the deluged executable.
        /// </summary>
        public static string DelugedExe
        {
            get { return Path.Combine(Directory, "deluged.exe"); }
        }
    }
}
