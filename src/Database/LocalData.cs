using System.Text.Json;

namespace Database
{

    public struct LogEntry
    {
        public string Type { get; }
        public string Message { get; }

        public LogEntry(string type, string message)
        {
            Type = type;
            Message = message;

        }
        public string ToJson() => JsonSerializer.Serialize(this);
    }

    public static class LocalData
    {
        public static void SaveLog(string fileName, LogEntry logEntry)
        {
            string path = GetFullPath(fileName);
            EnsureDirectoryExists(path);
            File.AppendAllText(path, logEntry.ToJson() + "\n");
            Console.WriteLine($"Log saved at: {path}");
        }

        public static void ClearLogs(string fileName)
        {
            string path = GetFullPath(fileName);
            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
                Console.WriteLine($"Logs cleared at: {path}");
            }
        }

        public static IEnumerable<LogEntry> LoadLogs(string fileName)
        {
            string path = GetFullPath(fileName);
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines)
                {
                    if (TryParseLogEntry(line, out LogEntry logEntry))
                    {
                        yield return logEntry;
                    }
                }
            }
        }

        private static string GetFullPath(string fileName)
        {
            string root = AppContext.BaseDirectory;
            string databaseDirectory = Path.Combine(root, ".database");
            return Path.Combine(databaseDirectory, $"{fileName}.json");
        }

        private static void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private static bool TryParseLogEntry(string json, out LogEntry logEntry)
        {
            try
            {
                logEntry = JsonSerializer.Deserialize<LogEntry>(json);
                return true;
            }
            catch
            {
                logEntry = default;
                return false;
            }
        }
    }
}
