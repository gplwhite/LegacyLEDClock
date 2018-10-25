
using System;

namespace ClockLib
{
    public class ClockState
    {
    
        public ClockMode Mode { get; private set; }

        public long ModeEpochTicks { get; private set; }

        public long EpochTicks { get; set; }

        #region Mode Change

        public void SetStandbyMode()
        {
            SetMode(ClockMode.Standby);
        }

        public void SetZeroMode()
        {
            SetMode(ClockMode.Zero);
        }

        public void SetWallClockMode()
        {
            SetMode(ClockMode.WallClock);
        }

        public void SetCountUpMode(DateTime? epoch = null, bool formatted = true)
        {
            if (epoch == null)
                epoch = DateTime.Now;

            EpochTicks = epoch.Value.Ticks;
            SetMode(formatted ? ClockMode.CountUpFormatted : ClockMode.CountUp);
        }

        public void SetCountDownMode(DateTime targetTime, bool formatted = true)
        {
            EpochTicks = targetTime.Ticks;
            SetMode(formatted ? ClockMode.CountDownFormatted : ClockMode.CountDown);
        }

        private void SetMode(ClockMode mode)
        {
            Mode = mode;
            ModeEpochTicks = DateTime.Now.Ticks;
        }

        #endregion

        /// <summary>
        /// Gets the length of time the clock has been in the current mode.
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetModeDuration()
        {
            var now = DateTime.Now;

            var x = now.AddTicks(-ModeEpochTicks);
            return x.TimeOfDay;
        }

        public TimeSpan GetCurrentClock()
        {
            var now = DateTime.Now;

            switch (Mode)
            {
                case ClockMode.WallClock:
                    return now.TimeOfDay;

                case ClockMode.CountUp:
                case ClockMode.CountUpFormatted:

                    var x = now.AddTicks(-EpochTicks);
                    return x.TimeOfDay;

                case ClockMode.CountDown:
                case ClockMode.CountDownFormatted:

                    // Reached zero
                    if (now.Ticks > EpochTicks)
                    {
                        Mode = ClockMode.Zero;
                        return TimeSpan.Zero;
                    }

                    var y = new DateTime(EpochTicks) - now;
                    return y;

                default:
                    return TimeSpan.Zero;
            }
        }

    }
}
