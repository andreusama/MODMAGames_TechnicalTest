using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TimeT
    {
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal TimeT(Int64 secondsSinceUnixEpoch)
        {
            this.SecondsSinceUnixEpoch = secondsSinceUnixEpoch;
        }

        public TimeT(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Supplied DateTime must be UTC");
            }

            this.SecondsSinceUnixEpoch = (Int64)dateTime.Subtract(UnixEpoch).TotalSeconds;
        }

        public DateTime DateTime {
            get {
                try
                {
                    // -1 is considered an error value with time_t; we use DateTime.MaxValue.
                    if (this.SecondsSinceUnixEpoch == -1)
                    {
                        return DateTime.MaxValue;
                    }
                    else
                    {
                        return UnixEpoch.AddSeconds(SecondsSinceUnixEpoch);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    return DateTime.MaxValue;
                }
            }
        }

        // Note that we're assuming 64-bit time_t.
        // See https://docs.microsoft.com/en-us/cpp/c-runtime-library/reference/time-time32-time64?view=vs-2019
        private readonly Int64 SecondsSinceUnixEpoch;
    }
}
