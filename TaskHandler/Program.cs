using System;
using System.Threading;
using TaskHandler.Devices.Simulator;
using TaskHandler.Handler;

namespace TaskHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessCardInfo();

            //Console.WriteLine("\r\nPress any key to exit...");
            //Console.ReadKey();
        }

        static void ProcessCardInfo()
        {
            try
            {
                SimulatorDevice simulator = new SimulatorDevice();

                // each task takes ~ 1000 ms to complete
                int timeout = 3500;
                var cancelTokenSource = new CancellationTokenSource(timeout);

                // number of tasks: 3
                simulator.ProcessCardInfo(cancelTokenSource, timeout);

                cancelTokenSource.Cancel();
                cancelTokenSource.Dispose();

                // Transaction will timeout: task takes ~ 1000 ms to complete
                timeout = 1500;
                cancelTokenSource = new CancellationTokenSource(timeout);

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
