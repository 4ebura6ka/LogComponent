namespace LogTest
{
    using System;
    using System.IO;
    using System.Text;

    public class LogStorageOperations
    {
        private const string DefaultLogPath = @"C:\LogTest";

        private readonly string _logPath;

        private string _logFullPath;

        private DateTime _lastLogFileCreationDateTime;

        public LogStorageOperations(string logPath = DefaultLogPath)
        {

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            _logPath = logPath;

        }

        public void CreateLogFile()
        {
            _lastLogFileCreationDateTime = DateTimeProvider.Current.DateTimeNow;

            var logTimeStamp = _lastLogFileCreationDateTime.ToString("yyyyMMdd HHmmss fff");
            var logFileName = $"Log{logTimeStamp}.log";
            _logFullPath = Path.Combine(_logPath, logFileName);

            using (var writer = File.AppendText(_logFullPath))
            {
                var captionStringBuilder = new StringBuilder();
                captionStringBuilder.Append("Timestamp".PadRight(25, ' '));
                captionStringBuilder.Append("\t");
                captionStringBuilder.Append("Data".PadRight(15, ' '));
                captionStringBuilder.Append("\t");
                captionStringBuilder.Append(Environment.NewLine);

                writer.Write(captionStringBuilder.ToString());

                writer.AutoFlush = true;
            }
        }

        public void WriteFormattedLineToLog(LogLine logLine)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var afterMidnight =
                DateTimeProvider.Current.DateTimeNow.Day - _lastLogFileCreationDateTime.Day != 0;

            if (afterMidnight)
            {
                CreateLogFile();

                stringBuilder.Append(Environment.NewLine);
            }

            stringBuilder.Append(logLine.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            stringBuilder.Append("\t");
            stringBuilder.Append(logLine.Text);
            stringBuilder.Append("\t");

            stringBuilder.Append(Environment.NewLine);

            using (var writer = File.AppendText(_logFullPath))
            {
                writer.Write(stringBuilder.ToString());
            }
        }

    }
}
