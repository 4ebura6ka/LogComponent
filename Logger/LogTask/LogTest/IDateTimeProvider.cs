namespace LogTest
{
    using System;

    public interface IDateTimeProvider
    {
        DateTime DateTimeNow { get; }
    }
}
