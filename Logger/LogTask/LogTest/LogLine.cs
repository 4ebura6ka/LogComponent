namespace LogTest
{
    using System;

    /// <summary>
    /// This is the object that the different loggers (FileLogger, ConsoleLogger etc.) will operate on. 
    /// </summary>
    public class LogLine
    {
        private string _text;

        public virtual string DisplayedText
        {
            get => $"{_text}. "; 
            set =>_text = value;
        }

        public virtual DateTime LineTimestamp { get; set; }
    }
}