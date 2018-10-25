using Caliburn.Micro;

namespace Clock4Windows.ViewModels
{
    public class ShellViewModel : Screen
    {

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
        public ShellViewModel()
        {
            DisplayName = "Clock 4 Windows";

            Clocks = new BindableCollection<ClockViewModel>();
            Devices = new BindableCollection<DeviceViewModel>();

            Clocks.Add(new ClockViewModel()
            {
                ClockName = "Timer 1"
            });

            Devices.Add(new DeviceViewModel
            {
                DeviceName = "Left Sign"
            });

            Devices.Add(new DeviceViewModel
            {
                DeviceName = "Right Sign"
            });
        }

        #endregion


        public void AddClock()
        {
            Clocks.Add(new ClockViewModel()
            {
                ClockName = "Timer " + (Clocks.Count + 1)
            });
        }

        public void RemoveClock()
        {

        }


        public void AddDevice()
        {
            Devices.Add(new DeviceViewModel()
            {
                DeviceName = "Device " + (Devices.Count + 1)
            });
        }

        public void RemoveDevice()
        {

        }


  

        public void AddClockToDevice(ClockViewModel clockVM)
        {
            SelectedDevice?.AssignClock(clockVM);
        }


    }
}
