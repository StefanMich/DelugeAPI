using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelugeAPI
{
    public class Connection
    {
        private bool connected;
        private string remote;
        private int? port;

        public Connection(string remote, int? port = null)
        {
            this.connected = false;
            this.remote = remote;
            this.port = port;
        }

        public void Connect()
        {
            if (connected)
                return;

            string arg = "connect " + remote;
            if (port.HasValue)
                arg += " " + port.Value;

            string response = RunCommand(arg).Trim();

            if (response.Length > 0)
                throw new ApplicationException(response);

            connected = true;
        }

        public void Disconnect()
        {
            if (!connected)
                return;

            string response = RunCommand("exit").Trim();

            if (response.Length > 0)
                throw new ApplicationException(response);

            connected = false;
        }

        public string RunCommand(params string[] args)
        {
            Process p = Process.Start(new ProcessStartInfo(DelugePathInfo.DelugeConsoleExe, string.Join(" ", args))
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            p.WaitForExit();
            return p.StandardOutput.ReadToEnd();
        }
    }
}
