using System.Diagnostics;

namespace DelugeAPI
{
    /// <summary>
    /// Exposes methods for starting/stopping a deluged server locally.
    /// </summary>
    public class DelugedProcess
    {
        private Process process;

        private static ProcessStartInfo getProcessInfo()
        {
            return new ProcessStartInfo(DelugePathInfo.DelugedExe);
        }

        private DelugedProcess()
        {
            this.process = null;
        }


        private static DelugedProcess server = new DelugedProcess();
        /// <summary>
        /// Gets the local deluged-server (will not register a process that is already running).
        /// </summary>
        public static DelugedProcess Server
        {
            get { return server; }
        }

        /// <summary>
        /// Starts the server-instance referenced by this <see cref="DelugedProcess"/>.
        /// </summary>
        public void Start()
        {
            if (!Running)
                this.process = Process.Start(getProcessInfo());
        }
        /// <summary>
        /// Stops the server-instance referenced by this <see cref="DelugedProcess"/>.
        /// </summary>
        public void Stop()
        {
            if (!Running)
                return;

            process.Kill();
            while (!process.HasExited) { }
            process = null;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DelugedProcess"/> is running.
        /// </summary>
        public bool Running
        {
            get { return process != null; }
        }
    }
}
