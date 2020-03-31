namespace LogTest
{
    using System;
    using System.IO;
    using System.Text;

    public class LogsStorage
    {
        private const string DefaultLogDirPath = @"C:\LogTest";

        private readonly string _logDirPath;

        private string _currentLogFileFullPath;

        private DateTime _lastLogFileCreationDateTime;

        public LogsStorage(string logDirPath = DefaultLogDirPath)
        {
            if (!Directory.Exists(logDirPath))
            {
                Directory.CreateDirectory(logDirPath);
            }

            _logDirPath = logDirPath;
        }

        public void CreateNewLogFile()
        {
            _lastLogFileCreationDateTime = DateTimeProvider.Current.DateTimeNow;

            var logTimeStamp = _lastLogFileCreationDateTime.ToString("yyyyMMdd HHmmss fff");
            var logFileName = $"Log{logTimeStamp}.log";

            _currentLogFileFullPath = Path.Combine(_logDirPath, logFileName);

            using (var writer = File.AppendText(_currentLogFileFullPath))
            {
                var captionStringBuilder = new StringBuilder();
                captionStringBuilder.Append("LineTimestamp".PadRight(25, ' '));
                captionStringBuilder.Append("\t");
                captionStringBuilder.Append("Data".PadRight(15, ' '));
                captionStringBuilder.Append("\t");
                captionStringBuilder.Append(Environment.NewLine);

                writer.AutoFlush = true;

                writer.Write(captionStringBuilder.ToString());
            }
        }

        public void WriteFormattedLineToLog(LogLine logLine)
        {

            var afterMidnight =
                DateTimeProvider.Current.DateTimeNow.Day - _lastLogFileCreationDateTime.Day != 0;

            if (afterMidnight)
            {
                CreateNewLogFile();
            }

            using (var writer = File.AppendText(_currentLogFileFullPath))
            {
                var stringBuilder = new StringBuilder();

                stringBuilder.Append(logLine.LineTimestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
                stringBuilder.Append("\t");
                stringBuilder.Append(logLine.DisplayedText);
                stringBuilder.Append("\t");
                stringBuilder.Append(Environment.NewLine);

                writer.Write(stringBuilder.ToString());
            }
        }

    }
}
