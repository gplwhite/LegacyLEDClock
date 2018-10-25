using Caliburn.Micro;

namespace Clock4Windows.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly ViewModelFactory _viewModelFactory;

        public IObservableCollection<ClockViewModel> Clocks { get; }
        public IObservableCollection<DeviceViewModel> Devices { get; }

        private DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get => _selectedDevice;
            set { _selectedDevice = value; NotifyOfPropertyChange(nameof(SelectedDevice)); }
        }

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel(ViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;

            DisplayName = "Clock 4 Windows";

            Clocks = new BindableCollection<ClockViewModel>();
            Devices = new BindableCollection<DeviceViewModel>();

            AddClock();
            AddDevice("Left Sign");
            AddDevice("Right Sign");
        }

        #endregion


        public void AddClock()
        {
            Clocks.Add(_viewModelFactory.Create<ClockViewModel>(c =>
            {
                c.ClockName = "Timer " + (Clocks.Count + 1);
            }));
        }

        public void RemoveClock()
        {
            // TODO:
        }


        public void AddDevice(string deviceName = null)
        {
            Devices.Add(_viewModelFactory.Create<DeviceViewModel>(d =>
            {
                d.DeviceName = deviceName ?? "Device " + (Devices.Count + 1);
            }));
        }

        public void RemoveDevice()
        {
            // TODO:
        }




        public void AddClockToDevice(ClockViewModel clock)
        {
            SelectedDevice?.AssignClock(clock);
        }


    }
}
