namespace LogTest
{
    using System;
    using System.Text;

    /// <summary>
    /// This is the object that the different loggers (FileLogger, ConsoleLogger etc.) will operate on. 
    /// The LineText() method will be called to get the _text (formatted) to log
    /// </summary>
    public class LogLine
    {
        private string _text;
        /// <summary>
        /// The _text to be display in log line
        /// </summary>
        public string Text
        {
            get { return $"{_text}. "; }
            set { _text = value; }
        }

        /// <summary>
        /// The Timestamp is initialized when the log is added.
        /// </summary>
        public virtual DateTime Timestamp { get; set; }

        public LogLine()
        {
        }
    }
}