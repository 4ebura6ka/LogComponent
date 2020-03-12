using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTest
{
    public static class DateTimeProvider
    {
        private static IDateTimeProvider current = new DefaultTimeProvider();

        public static IDateTimeProvider Current
        {
            get
            {
                return current;
            }
            set
            {
                current = value ?? throw new ArgumentNullException("value");
            }
        }

        private class DefaultTimeProvider : IDateTimeProvider
        {
            public DateTime DateTimeNow
            {
                get
                {
                    return DateTime.Now;
                }
            }
        }
    }
}
