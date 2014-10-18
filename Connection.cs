using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DelugeAPI
{
    /// <summary>
    /// Provides methods for querying a Deluged server.
    /// </summary>
    public class Connection
    {
        private static CultureInfo doublesCulture = new CultureInfo("en-US");

        private static long parseSize(string sizeString)
        {
            Match m = Regex.Match(sizeString, @"(?<size>[0-9]+\.[0-9]+) (?<sizeinfo>[KMG]iB)");

            double size = double.Parse(m.Groups["size"].Value, doublesCulture);
            switch (m.Groups["sizeinfo"].Value.ToLower())
            {
                case "kib": size *= 1024; break;
                case "mib": size *= 1024 * 1024; break;
                case "gib": size *= 1024 * 1024 * 1024; break;
                default:
                    throw new ArgumentException("Unknown size-modifier: " + m.Groups["sizeinfo"].Value);
            }

            return (long)Math.Round(size);
        }

        private bool connected;
        private string remote;
        private int? port;

        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="remote">The remote destination. This is a network location such as 'localhost' (default) or an IP address.</param>
        /// <param name="port">The port to use on the server running deluged, or <c><null/c> for the default port.</param>
        public Connection(string remote = "localhost", int? port = null)
        {
            this.connected = false;
            this.remote = remote;
            this.port = port;
        }

        /// <summary>
        /// Connects this instance of <see cref="Connection"/> to its deluged server.
        /// </summary>
        public void Connect()
        {
            if (connected)
                return;

            string arg = "connect " + remote;
            if (port.HasValue)
                arg += " " + port.Value;

            string response = runCommand(arg);

            if (response.Length > 0)
                throw new ApplicationException(response);

            connected = true;
        }
        /// <summary>
        /// Disconnects this instance of <see cref="Connection"/> from its deluged server.
        /// </summary>
        public void Disconnect()
        {
            if (!connected)
                return;

            string response = runCommand("exit");

            if (response.Length > 0)
                throw new ApplicationException(response);

            connected = false;
        }

        /// <summary>
        /// Gets information about all the torrent managed by the deluged server.
        /// </summary>
        /// <returns>An array of the torrents managed by the deluged server.</returns>
        public TorrentInfo[] GetTorrentInfo()
        {
            string data = runCommand("info -v").Replace("\r", "");
            var matches = Regex.Matches(data, @"Name(:?[^:])*::(:?[^:])*::( *[^ \n][^\n]+\n?)+");

            TorrentInfo[] result = new TorrentInfo[matches.Count];
            for (int i = 0; i < matches.Count; i++)
                result[i] = parseTorrentInfo(matches[i].Value);

            return result;
        }

        private static TorrentInfo parseTorrentInfo(string torrentInfoString)
        {
            var m = Regex.Match(torrentInfoString, @"^(?<main>(:?[^:])*)::Files(?<files>(:?[^:])*)::");

            string[] info = m.Groups["main"].Value.Trim().Split('\n');

            string name = info[0].Substring("Name: ".Length);
            string id = info[1].Substring("ID: ".Length);
            long size = parseSize(Regex.Match(info[4], @"Size: ([0-9]+\.[0-9]+ [KMG]iB/)?(?<size>[0-9]+\.[0-9]+ [KMG]iB)").Groups["size"].Value);

            var progressM = info.Length < 8 ? null : Regex.Match(info[7], @"Progress: (?<progress>[0-9]+\.[0-9]+)%");
            double progress = (progressM != null && progressM.Success) ? double.Parse(progressM.Groups["progress"].Value) : 100.0;

            var files = from f in m.Groups["files"].Value.Trim().Split('\n')
                        select parseTorrentFileInfo(f.Trim());

            return new TorrentInfo(name, id, size, progress / 100.0, files);
        }
        private static TorrentInfo.FileInfo parseTorrentFileInfo(string torrentFileInfoString)
        {
            if (torrentFileInfoString == "")
                return null;

            var m = Regex.Match(torrentFileInfoString, @"(?<file>.*) \((?<size>[0-9]+\.[0-9]+ [KMG]iB)\) Progress: (?<progress>[0-9]+\.[0-9]+)% Priority: (?<priority>.+)");

            return new TorrentInfo.FileInfo(
                m.Groups["file"].Value,
                parseSize(m.Groups["size"].Value),
                double.Parse(m.Groups["progress"].Value, doublesCulture) / 100.0);
        }

        private string runCommand(params string[] args)
        {
            Process p = Process.Start(new ProcessStartInfo(DelugePathInfo.DelugeConsoleExe, string.Join(" ", args))
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd().Trim();
        }


        public bool AddTorrent(string path)
        {
            var torrentsBefore = (from t in GetTorrentInfo() select t.ID).ToArray();

            runCommand(string.Format("add {0}", path));

            int added = 0;
            waitForPredicate(
                () => added > 0,
                () => added = (from t in GetTorrentInfo() select t.ID).Except(torrentsBefore).Count()
                );

            if (added > 1)
                throw new InvalidOperationException("More than one torrent was added.");
            else if (added == 0)
                return false;
            else
                return true;
        }

        public bool RemoveTorrent(string hash)
        {
            runCommand(String.Format("del {0}", hash));

            bool contains = true;

            return waitForPredicate(
                () => contains == false,
                () => contains = (from t in GetTorrentInfo() select t.ID).Contains(hash)
                );
        }

        private bool waitForPredicate(Func<bool> predicate, Action update, int attempts = 3)
        {
            update();

            bool ok;
            while (ok = predicate() && attempts > 0)
            {
                attempts--;
                System.Threading.Thread.Sleep(100);
                update();
            }

            return ok;
        }
    }
}
