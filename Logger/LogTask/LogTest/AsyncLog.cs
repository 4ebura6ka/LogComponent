namespace LogTest
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;

    public class AsyncLog : ILog
    {
        private Thread _runThread;

        private List<LogLine> _lines = new List<LogLine>();

        private StreamWriter _writer;

        private bool _exit;

        private bool _quitWithFlush;

        private DateTime _lastLogFileCreationDateTime;

        public AsyncLog()
        {
            if (!Directory.Exists(@"C:\LogTest"))
            {
                Directory.CreateDirectory(@"C:\LogTest");
            }

            CreateLogFile();

            _runThread = new Thread(MainLoop);

            _runThread.Start();
        }

        private void CreateLogFile()
        {
            _lastLogFileCreationDateTime = DateTimeProvider.Current.DateTimeNow;

            var logTimeStamp = _lastLogFileCreationDateTime.ToString("yyyyMMdd HHmmss fff");
            var logFileName = $"Log{logTimeStamp}.log";
            _writer = File.AppendText(@"C:\LogTest\" + logFileName);

            var captionStringBuilder = new StringBuilder();
            captionStringBuilder.Append("Timestamp".PadRight(25, ' '));
            captionStringBuilder.Append("\t");
            captionStringBuilder.Append("Data".PadRight(15, ' '));
            captionStringBuilder.Append("\t");
            captionStringBuilder.Append(Environment.NewLine);

            _writer.Write(captionStringBuilder.ToString());

            _writer.AutoFlush = true;
        }

        private void MainLoop()
        {
            while (!_exit)
            {
                if (_lines.Count == 0)
                {
                    continue;
                }

                var linesWrittenToLog = new List<LogLine>();

                foreach (var logLine in _lines)
                {
                    if (_exit || !_quitWithFlush)
                    {
                        break;
                    }

                    WriteFormattedLineToLog(logLine);
                    linesWrittenToLog.Add(logLine);
                }

                foreach (var line in linesWrittenToLog)
                {
                    _lines.Remove(line);
                }

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
            stringBuilder.Append(logLine.LineText());
            stringBuilder.Append("\t");

            stringBuilder.Append(Environment.NewLine);

            _writer.Write(stringBuilder.ToString());
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
            _lines.Add(new LogLine { Text = text, Timestamp = DateTimeProvider.Current.DateTimeNow });
        }
    }
}