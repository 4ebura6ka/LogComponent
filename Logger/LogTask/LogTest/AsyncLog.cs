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

        private readonly LogStorageOperations _logStorageOperations;

        private Task _mainLoopTask;

        public AsyncLog(LogStorageOperations logStorageOperations)
        {
            _logStorageOperations = logStorageOperations;

            _logStorageOperations.CreateLogFile();

            try
            {
                _mainLoopTask = Task.Run(() => MainLoop());
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
            }
        }

        public void StopWithoutFlush()
        {
            _exit = true;
            _mainLoopTask.Wait();
        }

        public void StopWithFlush()
        {
            _quitWithFlush = true;
            _mainLoopTask.Wait();
        }

        public void Write(string text)
        {
            var logLine = new LogLine {Text = text, Timestamp = DateTimeProvider.Current.DateTimeNow};

            _lines.Add(logLine);
        }
    }
}