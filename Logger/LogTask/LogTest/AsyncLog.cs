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

        private async void MainLoop()
        {
            try
            {
                foreach (var logLine in _lines.GetConsumingEnumerable())
                {
                    _logStorageOperations.WriteFormattedLineToLog(logLine);

                    await Task.Delay(10);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Collection was disposed");
            }
        }

        public void StopWithoutFlush()
        {
            _lines.Dispose();
        }

        public void StopWithFlush()
        {
            _lines.CompleteAdding();
        }

        public void Write(string text)
        {
            var logLine = new LogLine { Text = text, Timestamp = DateTimeProvider.Current.DateTimeNow };

            _lines.Add(logLine);
        }
    }
}