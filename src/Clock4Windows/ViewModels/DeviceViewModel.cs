using Caliburn.Micro;
using ClockLib;

namespace Clock4Windows.ViewModels
{
    public class DeviceViewModel : PropertyChangedBase
    {

        private ClockConnection _deviceConnection;
        private ClockViewModel _assignedClock;

        public DeviceViewModel()
        {
            _deviceConnection = new ClockConnection();
            _deviceConnection.ConnectedChanged += DeviceConnection_ConnectedChanged;

            _status = DeviceStatus.NotAttempted;
            ClockName = "<None Assigned>";

            AvailablePorts = new BindableCollection<string>();

            // TODO: Enumerate available COM ports
            AvailablePorts.Add("COM7");
            AvailablePorts.Add("COM8");
            AvailablePorts.Add("COM9");
        }


        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set { _deviceName = value; NotifyOfPropertyChange(nameof(DeviceName)); }
        }


        private string _portName;
        public string PortName
        {
            get => _portName;
            set
            {
                _portName = value;
                NotifyOfPropertyChange(nameof(PortName));
                NotifyOfPropertyChange(nameof(CanConnectOrDisconnect));
            }
        }

        public IObservableCollection<string> AvailablePorts { get; private set; }



        public string _clockName;
        public string ClockName
        {
            get => _clockName;
            set
            {
                _clockName = value;
                NotifyOfPropertyChange(nameof(ClockName));
            }
        }




        private DeviceStatus _status;
        public DeviceStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                NotifyOfPropertyChange(nameof(Status));
                NotifyOfPropertyChange(nameof(CanConnectOrDisconnect));
            }
        }



        public bool CanConnectOrDisconnect
        {
            get
            {
                return Status != DeviceStatus.Connecting && !string.IsNullOrEmpty(PortName);
            }
        }

        public void ConnectOrDisconnect()
        {
            if (Status == DeviceStatus.Connected)
            {
                _deviceConnection.Stop();
            }
            else
            {
                Status = DeviceStatus.Connecting;
                _deviceConnection.Start(PortName);
            }
        }



        private void DeviceConnection_ConnectedChanged(object sender, ConnectedChangedEventArgs e)
        {
            Status = e.Connected
                ? DeviceStatus.Connected
                : DeviceStatus.Disconnected;
        }




        public void AssignClock(ClockViewModel clock)
        {
            if (_assignedClock != null)
                _assignedClock.PropertyChanged -= AssignedClock_PropertyChanged;

            _assignedClock = clock;
            _assignedClock.PropertyChanged += AssignedClock_PropertyChanged;

            ClockName = clock.ClockName;

            _deviceConnection.ChangeClock(clock.Clock);
        }

        private void AssignedClock_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClockViewModel.ClockName))
            {
                ClockName = _assignedClock.ClockName;
            }
        }
    }
}
