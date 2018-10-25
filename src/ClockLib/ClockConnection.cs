﻿using System;
using System.IO.Ports;
using System.Threading;

namespace ClockLib
{
    public class ClockConnection
    {
        private const int RenderLoopTimeout = 50;  // milliseconds
        
        private readonly ClockHardwareRenderer _hardwareRenderer;

        private SerialPort _port;

        private readonly ManualResetEvent _stopEvent;
        private Thread _renderingThread;


        public ClockConnection(ClockState state = null)
        {
            if (state == null)
                state = new ClockState();

            _hardwareRenderer = new ClockHardwareRenderer(state);
            _stopEvent = new ManualResetEvent(false);
        }


        public bool Start(string portName)
        {
            if (Connected) return true;

            // Make a serial connection
            _port = new SerialPort(portName, 4800, Parity.None, 8, StopBits.One);

            try
            {
                _port.Open();
                SetConnectionStatus(true);
            }
            catch (Exception e)
            {
                SetConnectionStatus(false);
                return false;
            }

            // Start the rendering thread
            _stopEvent.Reset();

            _renderingThread = new Thread(RenderLoop);
            _renderingThread.Start();

            return true;
        }
        
        public void Stop()
        {
            if (!Connected) return;

            // Signal to the rendering thread that is should stop
            _stopEvent.Set();
            _renderingThread.Join(5000);
            _renderingThread = null;

            // Close connection
            try
            {
                _port.Close();
            }
            catch (Exception e)
            { }

            SetConnectionStatus(false);
            _port = null;
        }


        public bool Connected { get; private set; }

        public event EventHandler<ConnectedChangedEventArgs> ConnectedChanged;
        
        private void SetConnectionStatus(bool connected)
        {
            Connected = connected;

            ConnectedChanged?.Invoke(this, new ConnectedChangedEventArgs(connected));
        }


        public void ChangeClock(ClockState state)
        {
            _hardwareRenderer.SetClock(state);
        }



        private void RenderLoop()
        {
           
            while (true)
            {
                if (_stopEvent.WaitOne(RenderLoopTimeout))
                {
                    // Stop signalled - exit the loop
                    break;
                }

                // TODO: Close the connection on error
                _hardwareRenderer.RenderToDevice(_port);

               
            }
        }



    }

    public class ConnectedChangedEventArgs : EventArgs
    {
        public bool Connected { get; }

        public ConnectedChangedEventArgs(bool connected)
        {
            Connected = connected;
        }
    }
}
