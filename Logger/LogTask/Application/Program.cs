using System.Threading.Tasks;

namespace LogUsers
{
    using System;
    using LogTest;

    class Program
    {
        static void Main(string[] args)
        { 
            ILog  logger = new AsyncLog(new LogStorageOperations());
            for (int i = 0; i < 15; i++)
            {
                logger.Write($"Number with Flush: {i}");
                Task.Delay(10).Wait();
            }
            logger.StopWithFlush();

            ILog logger2 = new AsyncLog(new LogStorageOperations());
            for (int i = 50; i > 0; i--)
            {
                logger2.Write($"Number with No flush: {i}");
                Task.Delay(5).Wait();
            }
            logger2.StopWithoutFlush();

            Console.ReadKey();
        }
    }
}
