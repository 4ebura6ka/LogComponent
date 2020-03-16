namespace LogUsers
{
    using System;
    using System.Threading;
    using LogTest;

    class Program
    {
        static void Main(string[] args)
        { 
            ILog  logger = new AsyncLog(new LogStorageOperations());

            for (int i = 0; i < 15; i++)
            {
                logger.Write($"Number with Flush: {i}");
                Thread.Sleep(50);
            }

            logger.StopWithFlush();

            ILog logger2 = new AsyncLog(new LogStorageOperations());

            for (int i = 50; i > 0; i--)
            {
                logger2.Write($"Number with No flush: {i}");
                Thread.Sleep(30);
            }

            logger2.StopWithoutFlush();

            Console.ReadLine();
        }
    }
}
