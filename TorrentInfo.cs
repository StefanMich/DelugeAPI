using System.Collections.Generic;
using System.Linq;

namespace DelugeAPI
{
    /// <summary>
    /// Represents a torrent managed by a deluge server.
    /// </summary>
    public class TorrentInfo
    {
        private string name;
        private string id;
        private long size;
        private double progress;

        private FileInfo[] files;

        internal TorrentInfo(string name, string id, long size, double progress, IEnumerable<FileInfo> files)
        {
            this.name = name;
            this.id = id;
            this.size = size;
            this.progress = progress;

            this.files = files.ToArray();
        }

        /// <summary>
        /// Gets the name of the torrent.
        /// </summary>
        public string Name
        {
            get { return name; }
        }
        /// <summary>
        /// Gets the identifier of the torrent.
        /// </summary>
        public string ID
        {
            get { return id; }
        }
        /// <summary>
        /// Gets the approximate size of the torrent in bytes.
        /// </summary>
        public long Size
        {
            get { return size; }
        }
        /// <summary>
        /// Gets the progress of downloading the torrent in the range [0-1] where 0 represents "just started" and 1 represents "completed".
        /// </summary>
        public double Progress
        {
            get { return progress; }
        }

        /// <summary>
        /// Gets a collection of the files that are part of this torrent.
        /// </summary>
        public IEnumerable<FileInfo> Files
        {
            get { foreach (var f in files) yield return f; }
        }

        /// <summary>
        /// Represents a single file in a <see cref="TorrentInfo"/>.
        /// </summary>
        public class FileInfo
        {
            private string path;
            private long size;
            private double progress;

            internal FileInfo(string path, long size, double progress)
            {
                this.path = path;
                this.size = size;
                this.progress = progress;
            }

            /// <summary>
            /// Gets the path of the file, relative to the download-path of the torrent.
            /// </summary>
            public string Path
            {
                get { return path; }
            }
            /// <summary>
            /// Gets the approximate size of the file in bytes.
            /// </summary>
            public long Size
            {
                get { return size; }
            }
            /// <summary>
            /// Gets the progress of downloading the file in the range [0-1] where 0 represents "just started" and 1 represents "completed".
            /// </summary>
            public double Progress
            {
                get { return progress; }
            }
        }
    }
}
