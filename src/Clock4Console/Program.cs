using ClockLib;
using System;
using System.Threading;

namespace Clock4Console
{
    class Program
    {
        static void Main()
        {

            var clock = new ClockState();

            var clockConnection = new ClockConnection(clock);

            Console.Write("Connecting to clock...");

            if (clockConnection.Start("COM9"))
            {
                Console.WriteLine("connected.");
            }
            else 
            {
                Console.WriteLine("FAILED.");
            }
            
            // Wait for a user to stop
            while (true)
            {
                if (!ProcessInput(clock))
                    break;
            }

            Console.Write("Stopping connection...");
            clockConnection.Stop();
            Console.WriteLine("done.");

            Thread.Sleep(500);
        }


        private static bool ProcessInput(ClockState clock)
        {
            Console.WriteLine("Choose mode:");
            Console.WriteLine("  <S> - Standby");
            Console.WriteLine("  <Z> - Zero");
            Console.WriteLine("  <W> - WallClock Mode");
            Console.WriteLine("  <U> - CountUp Mode");
            Console.WriteLine("  <D> - CountDown Mode");
            Console.WriteLine("  <X> - Exit/Shutdown");

            var line = Console.ReadLine();
            line = line?.ToLower();
            


            switch (line)
            {
                case "x":
                    return false;

                case "s":
                    clock.SetStandbyMode();
                    return true;
                case "z":
                    clock.SetZeroMode();
                    return true;
                case "w":
                    clock.SetWallClockMode();
                    return true;
                case "u":
                    clock.SetCountUpMode();
                    return true;
                case "d":
                    clock.SetCountDownMode(DateTime.Now.AddMinutes(3));
                    return true;
            }

            return true;
        }
    }
}
