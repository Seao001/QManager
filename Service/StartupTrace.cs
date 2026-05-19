using System;
using System.IO;

namespace QManager.Service
{
    public static class StartupTrace
    {
        private static readonly string TraceFilePath = Path.Combine(
            Path.GetTempPath(),
            "qmanager-startup.log");

        public static void Write(string message)
        {
            try
            {
                File.AppendAllText(
                    TraceFilePath,
                    $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}");
            }
            catch
            {
            }
        }
    }
}
