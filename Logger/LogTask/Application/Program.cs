using System.Threading.Tasks;

namespace LogUsers
{
    using System;
    using LogTest;

    public class Program
    {
        public static void Main(string[] args)
        { 
            var flushLogger = new AsyncLog(new LogsStorage());
            for (var i = 0; i < 15; i++)
            {
                flushLogger.Write($"Number with Flush: {i}");
                Task.Delay(10).Wait();
            }
            flushLogger.StopWithFlush();

            var noFlushLogger = new AsyncLog(new LogsStorage());
            for (var i = 50; i > 0; i--)
            {
                noFlushLogger.Write($"Number with No flush: {i}");
                Task.Delay(5).Wait();
            }
            noFlushLogger.StopWithoutFlush();

            Console.ReadKey();
        }
    }
}
