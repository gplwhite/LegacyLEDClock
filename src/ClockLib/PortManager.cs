using System;
using System.IO.Ports;

namespace ClockLib
{
    public class PortManager : IDisposable
    {

        private string[] _currentPorts = new string[0];


        public PortManager()
        {
        }



        public void Start()
        {
            // TODO: Monitor for changes

            _currentPorts = SerialPort.GetPortNames();
            
        }


        public void Stop()
        {
            // Todo: Stop Monitoring
        }





        public string[] GetComPorts()
        {
            return _currentPorts;
        }


        public void Dispose()
        {
            Stop();
        }

        
    }
}
