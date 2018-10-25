using Caliburn.Micro;
using ClockLib;
using System;
using System.Threading;

namespace Clock4Windows.ViewModels
{
    public class ClockViewModel : PropertyChangedBase
    {

        private const int UpdateTimerIntervalMs = 200;
        private Timer _updateTimer;


        public ClockState Clock { get; }

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ClockViewModel"/> class.
        /// </summary>
        public ClockViewModel()
        {
            Clock = new ClockState();

            AvailableClockModes = new BindableCollection<ClockMode>
            {
                ClockMode.Standby,
                ClockMode.Zero,
                ClockMode.WallClock,
                ClockMode.CountUp,
                ClockMode.CountUpFormatted,
                ClockMode.CountDown,
                ClockMode.CountDownFormatted
            };

            UpdateModeSettings(ClockMode);
            StartStopUpdateTimer();
        }

        #endregion

        private string _clockName;
        public string ClockName
        {
            get => _clockName;
            set { _clockName = value; NotifyOfPropertyChange(nameof(ClockName)); }
        }


        private ClockMode _clockMode;
        public ClockMode ClockMode
        {
            get => _clockMode;
            set
            {
                _clockMode = value;
                NotifyOfPropertyChange(nameof(ClockMode));

                HasChanges = true;
                UpdateModeSettings(value);
            }
        }

        public IObservableCollection<ClockMode> AvailableClockModes { get; }


        private bool _modeContinuouslyUpdates;
        public bool ModeContinuouslyUpdates
        {
            get => _modeContinuouslyUpdates;
            set
            {
                _modeContinuouslyUpdates = value;
                NotifyOfPropertyChange(nameof(ModeContinuouslyUpdates));

                StartStopUpdateTimer();
            }
        }


        private bool _modeRequiresEpoch;
        public bool ModeRequiresEpoch
        {
            get => _modeRequiresEpoch;
            set
            {
                _modeRequiresEpoch = value;
                NotifyOfPropertyChange(nameof(ModeRequiresEpoch));

                StartStopUpdateTimer();
            }
        }


        private EpochMode _epochMode;
        public EpochMode EpochMode
        {
            get => _epochMode;
            set
            {
                _epochMode = value;
                NotifyOfPropertyChange(nameof(EpochMode));
            }
        }


        public TimeSpan AbsoluteTimeSpan
        {
            get;
            set;
        }



        private string _absoluteTime;
        public string AbsoluteTime
        {
            get => _absoluteTime;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    AbsoluteTimeSpan = TimeSpan.Parse(value);
                }

                _absoluteTime = value;
                NotifyOfPropertyChange(nameof(AbsoluteTime));
            }
        }

        public TimeSpan RelativeTimeSpan
        {
            get;
            set;
        }

        private string _relativeTime;
        public string RelativeTime
        {
            get => _relativeTime;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    RelativeTimeSpan = TimeSpan.Parse(value);
                }

                _relativeTime = value;
                NotifyOfPropertyChange(nameof(RelativeTime));
            }
        }



        public TimeSpan NowTime => DateTime.Now.TimeOfDay;



        public string ClockOutput
        {
            get
            {
                // NOTE: Use the current mode of the Clock, not the current mode of the viewmodel which may not be applied yet
                if (Clock.Mode == ClockMode.Standby)
                    return "- - - - - -";

                var clockTime = Clock.GetCurrentClock();

                return clockTime.ToString("hh\\:mm\\:ss");
            }
        }


        public bool CanEditMode
        {
            get
            {
                if (HasChanges) return false;

                return ClockMode == ClockMode.CountUp ||
                    ClockMode == ClockMode.CountUpFormatted ||
                    ClockMode == ClockMode.CountDown ||
                    ClockMode == ClockMode.CountDownFormatted;
            }
        }

        public void EditMode()
        {
            HasChanges = true;
            UpdateModeSettings(ClockMode);
        }


        private bool _hasChanges;
        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                _hasChanges = value;
                NotifyOfPropertyChange(nameof(HasChanges));
                NotifyOfPropertyChange(nameof(CanEditMode));
            }
        }



        public void ApplyChanges()
        {
            bool isFormatted;

            switch (ClockMode)
            {
                case ClockMode.Standby:
                    Clock.SetStandbyMode();
                    break;

                case ClockMode.Zero:
                    Clock.SetZeroMode();
                    break;

                case ClockMode.WallClock:
                    Clock.SetWallClockMode();
                    break;

                case ClockMode.CountUp:
                case ClockMode.CountUpFormatted:
                    isFormatted = ClockMode == ClockMode.CountUpFormatted;

                    // TODO: Parse Options
                    var now = DateTime.Now;
                    var nowTime = now.TimeOfDay;
                    var epoch = DateTime.Now;

                    if (EpochMode == EpochMode.Now)
                        epoch = now;

                    else if (EpochMode == EpochMode.AbsoluteTime)
                    {
                        // If its in the past, then it must be tomorrow 
                        if (AbsoluteTimeSpan > nowTime)
                        {
                            now = now.AddDays(-1);
                        }

                        epoch = new DateTime(now.Year, now.Month, now.Day, AbsoluteTimeSpan.Hours, AbsoluteTimeSpan.Minutes, AbsoluteTimeSpan.Seconds);
                    }
                    else if (EpochMode == EpochMode.RelativeTime)
                    {
                        epoch = now
                            .AddHours(-RelativeTimeSpan.Hours)
                            .AddMinutes(-RelativeTimeSpan.Minutes)
                            .AddSeconds(-RelativeTimeSpan.Seconds);
                    }


                    Clock.SetCountUpMode(epoch, formatted: isFormatted);
                    break;

                case ClockMode.CountDown:
                case ClockMode.CountDownFormatted:

                    isFormatted = ClockMode == ClockMode.CountDownFormatted;

                    // TODO: Parse Options

                    Clock.SetCountDownMode(DateTime.Now.AddMinutes(12), formatted: isFormatted);
                    break;
            }

            ModeRequiresEpoch = false;
            HasChanges = false;

            NotifyOfPropertyChange(nameof(ClockOutput));
        }


        private void UpdateModeSettings(ClockMode mode)
        {
            switch (mode)
            {
                case ClockMode.CountDown:
                case ClockMode.CountDownFormatted:
                case ClockMode.CountUp:
                case ClockMode.CountUpFormatted:
                    ModeRequiresEpoch = true;
                    ModeContinuouslyUpdates = true;
                    break;

                case ClockMode.WallClock:
                    ModeRequiresEpoch = false;
                    ModeContinuouslyUpdates = true;
                    break;

                default:
                    ModeRequiresEpoch = false;
                    ModeContinuouslyUpdates = false;
                    break;
            }
        }


        private void StartStopUpdateTimer()
        {

            var timerShouldBeRunning = _modeRequiresEpoch || _modeContinuouslyUpdates;

            if (timerShouldBeRunning)
            {
                if (_updateTimer == null)
                    _updateTimer = new Timer(UpdateTimer_Elapsed, null, 0, UpdateTimerIntervalMs);
            }
            else
            {
                if (_updateTimer != null)
                {
                    _updateTimer.Change(Timeout.Infinite, UpdateTimerIntervalMs);
                    _updateTimer.Dispose();
                    _updateTimer = null;
                }
            }
        }


        private void UpdateTimer_Elapsed(object sender)
        {
            if (HasChanges && ModeRequiresEpoch)
                NotifyOfPropertyChange(nameof(NowTime));


            NotifyOfPropertyChange(nameof(ClockOutput));
        }
    }

    public enum EpochMode
    {
        Now,
        RelativeTime,
        AbsoluteTime
    }
}
