namespace LogTest
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class AsyncLog : ILog
    {
        private BlockingCollection<LogLine> _lines = new BlockingCollection<LogLine>();

        private readonly LogsStorage _logsStorage;

        public AsyncLog(LogsStorage logsStorage)
        {
            _logsStorage = logsStorage;

            _logsStorage.CreateNewLogFile();

            try
            {
               Task.Run(() => MainLoop());
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
                    _logsStorage.WriteFormattedLineToLog(logLine);

                    await Task.Delay(10);
                }
            }
            catch (ObjectDisposedException)
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
            var logLine = new LogLine { DisplayedText = text, LineTimestamp = DateTimeProvider.Current.DateTimeNow };

            _lines.Add(logLine);
        }
    }
}