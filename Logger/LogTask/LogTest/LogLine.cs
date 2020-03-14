namespace LogTest
{
    using System;
    using System.Text;

    /// <summary>
    /// This is the object that the different loggers (FileLogger, ConsoleLogger etc.) will operate on. 
    /// The LineText() method will be called to get the text (formatted) to log
    /// </summary>
    public class LogLine
    {
        /// <summary>
        /// The text to be display in log line
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Timestamp is initialized when the log is added.
        /// </summary>
        public virtual DateTime Timestamp { get; set; }

        public LogLine()
        {
            Text = string.Empty;
        }

        public virtual string LineText()
        {
            StringBuilder sb = new StringBuilder();

            if (Text.Length > 0)
            {
                sb.Append(Text);
                sb.Append(". ");
            }

            sb.Append(CreateLineText());

            return sb.ToString();
        }

        public virtual string CreateLineText()
        {
            return string.Empty;
        }
    }
}