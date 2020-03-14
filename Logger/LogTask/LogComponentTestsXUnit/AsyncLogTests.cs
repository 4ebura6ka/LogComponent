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
        private string demoText = DateTime.Now.ToLongTimeString();

        [Fact]
        public async Task WriteToLogFile()
        {
            var dateProviderMock = new Mock<IDateTimeProvider>();
            var dateTime = new DateTime(2020, 3, 11, 18, 55, 00);
            dateProviderMock.SetupGet(x => x.DateTimeNow).Returns(dateTime);

            DateTimeProvider.Current = dateProviderMock.Object;

            var log = new AsyncLog();

            log.Write(demoText);
            Thread.Sleep(50);
            log.StopWithFlush();

            Assert.True(Directory.Exists(logDir));

            var files = Directory.GetFiles(logDir);
            var dateFormatted = dateTime.ToString("yyyyMMdd HHmmss fff");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                if (fileInfo.Name.Contains(dateFormatted))
                {
                    using (StreamReader streamReader = File.OpenText(file))
                    {
                        var fileContent = await streamReader.ReadToEndAsync();
                        Assert.Contains(fileContent, demoText);
                    }
                }
            }
        }

        [Fact]
        public void NewFilesCreatedAfterMidnight()
        {
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

        [Fact]
        public async Task StopWritingWithoutFlush()
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var dateTime = new DateTime(2020, 3, 14, 12, 30, 00);
            dateTimeProviderMock.SetupGet(x => x.DateTimeNow).Returns(dateTime);

            DateTimeProvider.Current = dateTimeProviderMock.Object;

            var log = new AsyncLog();
            log.Write(demoText);
            Thread.Sleep(50);
            log.StopWithoutFlush();

            Assert.True(Directory.Exists(logDir));
            var files = Directory.GetFiles(logDir);

            var formattedDate = dateTime.ToString("yyyyMMdd HHmmss fff");

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Name.Contains(formattedDate))
                {
                    using (var streamReader = File.OpenText(file))
                    {
                        var content = await streamReader.ReadToEndAsync();
                        Assert.DoesNotContain(demoText, content);
                    }
                    break;
                }
            }
        }

        [Fact]
        public async Task StopWritingWithFlush()
        {
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();
            var dateTime = new DateTime(2020, 3, 14, 12, 45, 00);
            dateTimeProviderMock.SetupGet(x => x.DateTimeNow).Returns(dateTime);

            DateTimeProvider.Current = dateTimeProviderMock.Object;

            var log = new AsyncLog();
            log.Write(demoText);
            Thread.Sleep(50);
            log.StopWithFlush();

            Assert.True(Directory.Exists(logDir));

            var files = Directory.GetFiles(logDir);
            var formattedDate = dateTime.ToString("yyyyMMdd HHmmss fff");
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                if (fileInfo.Name.Contains(formattedDate))
                {
                    using (StreamReader fileReader = File.OpenText(file))
                    {
                        var fileContent = await fileReader.ReadToEndAsync();
                        Assert.Contains(fileContent, demoText);
                    }
                    break;
                }
            }
        }
    }
}
