using System.Diagnostics;

namespace DelugeAPI
{
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

        public static DelugedProcess Create()
        {
            return new DelugedProcess();
        }

        public void Start()
        {
            if (!Running)
                this.process = Process.Start(getProcessInfo());
        }
        public void Stop()
        {
            if (!Running)
                return;

            process.Kill();
            while (!process.HasExited) { }
            process = null;
        }

        public bool Running
        {
            get { return process != null; }
        }
    }
}
