namespace LogTest
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


    public class AsyncLog : ILog
    {
        private BlockingCollection<LogLine> _lines = new BlockingCollection<LogLine>();

        private bool _exit;

        private bool _quitWithFlush;

        private LogStorageOperations _logStorageOperations;

        public AsyncLog(LogStorageOperations logStorageOperations)
        {
            _logStorageOperations = logStorageOperations;

            _logStorageOperations.CreateLogFile();

            try
            {
                Task.Run(() => MainLoop());
            }
            catch (AggregateException e)
            {
                Console.WriteLine("There were {0} exceptions", e.InnerExceptions.Count);
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

                _logStorageOperations.WriteFormattedLineToLog(logLine);

                _exit = _quitWithFlush && _lines.Count == 0;

                Thread.Sleep(50);
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