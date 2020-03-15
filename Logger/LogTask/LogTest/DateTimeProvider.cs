namespace LogTest
{
    using System;

    public static class DateTimeProvider
    {
        private static IDateTimeProvider _current = new DefaultTimeProvider();

        public static IDateTimeProvider Current
        {
            get => _current;
            set => _current = value ?? throw new ArgumentNullException(nameof(value));
        }

        private class DefaultTimeProvider : IDateTimeProvider
        {
            public DateTime DateTimeNow => DateTime.Now;
        }
    }
}
