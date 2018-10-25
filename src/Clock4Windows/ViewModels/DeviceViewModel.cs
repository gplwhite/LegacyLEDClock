using Caliburn.Micro;
using ClockLib;

namespace Clock4Windows.ViewModels
{
    public class DeviceViewModel : PropertyChangedBase
    {
        private readonly PortManager _portManager;

        private readonly ClockConnection _deviceConnection;
        private ClockViewModel _assignedClock;

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceViewModel"/> class.
        /// </summary>
        public DeviceViewModel(PortManager portManager)
        {
            _portManager = portManager;
            _deviceConnection = new ClockConnection();
            _deviceConnection.ConnectedChanged += DeviceConnection_ConnectedChanged;

            _status = DeviceStatus.NotAttempted;
            ClockName = "<None Assigned>";

            AvailablePorts = new BindableCollection<string>();

            BuildPortList();
        }

        private void BuildPortList()
        {
            var comPorts = _portManager.GetComPorts();

            foreach (var port in comPorts)
            {
                AvailablePorts.Add(port);
            }

            // TODO: Listen for change notifications from the PortManager

        }

        #endregion


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

        public IObservableCollection<string> AvailablePorts { get; }



        private string _clockName;
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



        public bool CanConnectOrDisconnect => Status != DeviceStatus.Connecting && !string.IsNullOrEmpty(PortName);

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
