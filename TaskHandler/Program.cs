using System;
using System.Threading;
using TaskHandler.Devices.Simulator;
using TaskHandler.Handler;

namespace TaskHandler
{
    class Program
    {
        private static SimulatorDevice simulator = new SimulatorDevice();

        static void Main(string[] args)
        {
            GetCardData();

            GetZip();

            //Console.WriteLine("\r\nPress any key to exit...");
            //Console.ReadKey();
        }

        static void GetCardData()
        {
            try
            {
                // each task takes ~ 1000 ms to complete
                int timeout = 3500;
                var cancelTokenSource = new CancellationTokenSource(timeout);

                // number of tasks: 3
                simulator.GetCardData(cancelTokenSource, timeout);

                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine($"main: executing exception='{e.Message}'");
            }
        }

        static void GetZip()
        {
            try
            {
                // each task takes ~ 1000 ms to complete
                int timeout = 1500;
                var cancelTokenSource = new CancellationTokenSource(timeout);

                // number of tasks: 2
                simulator.GetZip(cancelTokenSource, timeout);

                cancelTokenSource.Cancel();
            }
            catch(Exception e)
            {
                Console.WriteLine($"main: executing exception='{e.Message}'");
            }
        }
    }
}
