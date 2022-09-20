using System;
using System.Diagnostics;

namespace RewriteNginxConf
{
    public static class ShellHelper
    {
        public static string DockerComposeBash(this string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/usr/local/bin/docker-compose",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}
