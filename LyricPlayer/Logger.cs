using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LyricPlayer
{
    public static class Logger
    {
        public static string LogFileName { get; private set; }
        public static string LogFullFilePath => Path.Combine(ApplicationDirectory, LogFolderName, LogFileName);

        private static StreamWriter Writer { set; get; }
        private static string LogFolderName => "Log";
        private static string ApplicationDirectory => Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        static Logger()
        {
            LogFileName = $"log_{DateTime.Now:yyyy-mm-dd_hh-MM-ss}.txt";
            var directory = Path.GetDirectoryName(LogFullFilePath);
            Directory.CreateDirectory(directory);

            Writer = new StreamWriter(LogFullFilePath, true, Encoding.UTF8)
            {
                AutoFlush = true,
            };
        }

        public static void Information(string message)
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            var fullMessage = $"{assemblyName} {DateTime.Now:hh:mm:ss}] INFOMRATION: {message}";
            WriteToLogFile(fullMessage);
        }

        public static void Error(string message)
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            var fullMessage = $"{assemblyName} {DateTime.Now:hh:mm:ss}] Error: {message}";
            WriteToLogFile(fullMessage);
        }

        public static void Warning(string message)
        {
            var assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            var fullMessage = $"{assemblyName} {DateTime.Now:hh:mm:ss}] WARNING: {message}";
            WriteToLogFile(fullMessage);
        }



        private static void WriteToLogFile(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            Writer.WriteLine(message.Replace("\n", Environment.NewLine).Trim());
        }
    }
}
