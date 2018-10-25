
using System;
using System.IO.Ports;

namespace ClockLib
{
    public class ClockHardwareRenderer
    {
        private const int BufferSize = 15;

        private readonly byte[] _outputBuffer;
        private readonly int _outputBufferLength;

        private ClockState _state;
        private object _syncRoot = new object();

        //private int _temp1;
        //private int _temp2;
        //private const int Temp2RangeStart = 122;
        //private const int Temp2RangeEnd = 127;

        public ClockHardwareRenderer(ClockState state)
        {
            _state = state;
            _outputBuffer = new byte[BufferSize];
            _outputBufferLength = _outputBuffer.Length;

            //_temp2 = Temp2RangeStart;
        }


        public void SetClock(ClockState state)
        {
            lock (_syncRoot)
            {
                _state = state;
            }
        }


        public void RenderToDevice(SerialPort devicePort)
        {
            ClockState state;
            lock (_syncRoot)
            {
                state = _state;
            }

            if (state.Mode == ClockMode.Standby)
                return;

            int byteCount;

            if (state.Mode == ClockMode.Zero)
            {
                byteCount = RenderZero(_outputBuffer);
            }
            else
            {
                byteCount = RenderClockTime(state, _outputBuffer, _outputBufferLength);
            }

            // Render to device
            devicePort.Write(_outputBuffer, 0, byteCount);
        }


        private int RenderZero(byte[] buffer)
        {
            var modeDuration = _state.GetModeDuration();

            var flashing = (modeDuration.TotalSeconds < 10);

            var position = 0;

            var mod = modeDuration.TotalMilliseconds % 700;
            var flash = (mod < 300);
            if (flash)
            {
                buffer[position++] = 15;
                buffer[position++] = CharacterSet.ZeroOffset + 4;

                return 0;
            }
            else
            {
                buffer[position++] = CharacterSet.ZeroOffset + 1;
                buffer[position++] = CharacterSet.ZeroOffset;
                buffer[position++] = CharacterSet.ZeroOffset;
                buffer[position++] = CharacterSet.ZeroOffset;
                buffer[position++] = CharacterSet.ZeroOffset;
                buffer[position++] = CharacterSet.ZeroOffset;

            }

            buffer[position++] = CharacterSet.LineTerminator1;
            buffer[position++] = CharacterSet.LineTerminator2;

            return position;
        }


        private int RenderClockTime(ClockState state, byte[] buffer, int bufferLength)
        {
            var time = state.GetCurrentClock();

            var position = 0;

            if (state.Mode == ClockMode.WallClock)
            {
                RenderTimeComponent(time.Hours, buffer, ref position);
                RenderTimeComponent(time.Minutes, buffer, ref position);
                RenderTimeComponent(time.Seconds, buffer, ref position);
            }
            else if (state.Mode == ClockMode.CountUp || state.Mode == ClockMode.CountDown)
            {

                if (time.TotalHours < 1)
                {
                    RenderTimeComponent(time.Hours, buffer, ref position);
                    RenderTimeComponent(time.Minutes, buffer, ref position);
                    RenderTimeComponent(time.Seconds, buffer, ref position);

                    buffer[position++] = CharacterSet.DecimalSeperator;
                    RenderMilliseconds(time.Milliseconds, buffer, ref position);
                }
                else
                {
                    RenderTimeComponent(time.Hours, buffer, ref position);
                    RenderTimeComponent(time.Minutes, buffer, ref position);
                    RenderTimeComponent(time.Seconds, buffer, ref position);
                }

            }
            else if (state.Mode == ClockMode.CountUpFormatted || state.Mode == ClockMode.CountDownFormatted)
            {
                // Force first character to 1 to ensure the entire display (including zeros) is used
                buffer[position++] = CharacterSet.ZeroOffset + 1;
                buffer[position++] = (byte)(CharacterSet.ZeroOffset + (time.Hours >= 10 ? (time.Hours - (time.Hours / 10) * 10) : time.Hours));

                //RenderTimeComponent(time.Hours, buffer, ref position);
                RenderTimeComponent(time.Minutes, buffer, ref position);
                RenderTimeComponent(time.Seconds, buffer, ref position);
            }



            buffer[position++] = CharacterSet.LineTerminator1;
            buffer[position] = CharacterSet.LineTerminator2;

            return position;

        }


        private void RenderTimeComponent(int time, byte[] buffer, ref int position)
        {
            if (time >= 10)
            {
                var digit = time / 10;
                buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit);

                var digit2 = time - digit * 10;
                buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit2);
            }
            else
            {
                buffer[position++] = CharacterSet.ZeroOffset;
                buffer[position++] = (byte)(CharacterSet.ZeroOffset + time);
            }
        }

        private void RenderMilliseconds(int ms, byte[] buffer, ref int position)
        {
            if (ms >= 100)
            {
                var digit = ms / 100;
                buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit);

                //var digit2 = (ms - (digit * 100)) / 10;
                //buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit2);

                //var digit3 = ms - (digit * 100 + digit2 * 10);
                //buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit3);
            }
            else if (ms >= 10)
            {
                buffer[position++] = CharacterSet.ZeroOffset;

                //var digit = ms / 10;
                //buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit);

                //var digit2 = ms - digit * 10;
                //buffer[position++] = (byte)(CharacterSet.ZeroOffset + digit2);
            }
            else
            {
                buffer[position++] = CharacterSet.ZeroOffset;
                //buffer[position++] = CharacterSet.ZeroOffset;
                //buffer[position++] = (byte)(CharacterSet.ZeroOffset + ms);
            }
        }





    }

    public class CharacterSet
    {
        public const byte LineTerminator1 = 13;
        public const byte LineTerminator2 = 10;

        public const byte ZeroOffset = 48;

        public const byte DecimalSeperator = 46;
        public const byte TimeSeperator = 58;
    }
}
