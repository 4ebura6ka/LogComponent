using System.Collections.Concurrent;
using System.Threading;

namespace LogTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;


    public class AsyncLog : ILog
    {
        private const string DefaultLogPath = @"C:\LogTest";

        private readonly string _logPath;

        private string _logFullPath;

        private BlockingCollection<LogLine> _lines = new BlockingCollection<LogLine>();

        private bool _exit;

        private bool _quitWithFlush;

        private DateTime _lastLogFileCreationDateTime;

        public AsyncLog(string logPath = DefaultLogPath)
        {
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            _logPath = logPath;

            CreateLogFile();

            try
            {
                Task.Run(() => MainLoop());
            }
            catch (AggregateException e)
            {
                Console.WriteLine("There were {0} exceptions", e.InnerExceptions.Count);
            }
        }

        private void CreateLogFile()
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

        private void MainLoop()
        {
            while (!_exit)
            {
                if (_lines.Count == 0)
                {
                    continue;
                }

                var logLine = _lines.Take();

                WriteFormattedLineToLog(logLine);

                _exit = _quitWithFlush && _lines.Count == 0;

                Thread.Sleep(50);
            }
        }

        private void WriteFormattedLineToLog(LogLine logLine)
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

        public void StopWithoutFlush()
        {
            _exit = true;
        }

        public void StopWithFlush()
        {
            _quitWithFlush = true;
        }

        public void Write(string text)
        {
            Task.Run(() =>
            {
                var logLine = new LogLine {Text = text, Timestamp = DateTimeProvider.Current.DateTimeNow};

                _lines.Add(logLine);
            });
        }
    }
}