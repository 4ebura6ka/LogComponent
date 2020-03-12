using LogTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace LogComponentTestsXUnit
{
    public class AsyncLogTests
    {
        private const string logDir = @"C:\LogTest\";
        private const string demoText = "TestWrite to log";

        [Fact]
        public void WriteToLogFile()
        {
            var log = new AsyncLog();

            log.Write(demoText);
            Thread.Sleep(50);

            log.StopWithoutFlush();

            Assert.True(Directory.Exists(logDir));

            var files = Directory.GetFiles(logDir);
            var textFound = false;

            foreach (var file in files)
            {
                using (StreamReader streamReader = File.OpenText(file))
                {
                    streamReader.ReadLine();
                    var readline = streamReader.ReadLine();
                    if (readline.Contains(demoText))
                    {
                        textFound = true;
                        break;
                    }
                }
            }

            Assert.True(textFound);
        }

        [Fact]
        public void NewFilesCreatedAfterMidnight()
        {
            var now = DateTimeProvider.Current.DateTimeNow;

            var timeMock = new Mock<IDateTimeProvider>();

            timeMock.SetupGet(x => x.DateTimeNow).Returns(new DateTime(2020, 3, 12, 11, 59, 00));

            DateTimeProvider.Current = timeMock.Object;

            var log = new AsyncLog();

            log.Write(demoText);
            Thread.Sleep(50);

            Assert.True(Directory.Exists(logDir));

            var files = Directory.GetFiles(logDir);

            int numberOfFilesBeforeMidnight = files.Count();

            timeMock.SetupGet(x => x.DateTimeNow).Returns(new DateTime(2020, 3, 13, 00, 01, 00));
            log.Write(demoText);
            Thread.Sleep(50);

            int numberOfFilesAfterMidnight = Directory.GetFiles(logDir).Count();

            Assert.True(numberOfFilesBeforeMidnight < numberOfFilesAfterMidnight);
        }
    }
}
