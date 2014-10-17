using System.Collections.Generic;
using System.Linq;

namespace DelugeAPI
{
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

        public string Name
        {
            get { return name; }
        }
        public string ID
        {
            get { return id; }
        }
        public long Size
        {
            get { return size; }
        }
        public double Progress
        {
            get { return progress; }
        }

        public IEnumerable<FileInfo> Files
        {
            get { foreach (var f in files) yield return f; }
        }

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

            public string Path
            {
                get { return path; }
            }
            public long Size
            {
                get { return size; }
            }
            public double Progress
            {
                get { return progress; }
            }
        }
    }
}
