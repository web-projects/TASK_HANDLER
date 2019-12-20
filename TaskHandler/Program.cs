using System;
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

        static async void ProcessCardInfo()
        {
            try
            {
                SimulatorDevice simulator = new SimulatorDevice();

                // Main Processor
                await simulator.ProcessCardInfo();
            }
            catch(Exception e)
            {
                Console.WriteLine($"Executing exception='{e.Message}'");
            }
        }
    }
}
