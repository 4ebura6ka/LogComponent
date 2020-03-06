using LogTest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    }
}
