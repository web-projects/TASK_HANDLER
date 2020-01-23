using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TaskHandler.Devices.Simulator.Helpers;
using TaskHandler.Handler;

namespace TaskHandler.Devices.Simulator
{
    public class SimulatorDevice
    {
        readonly TaskEventHandler device;

        public SimulatorDevice()
        {
            device = new TaskEventHandler();
        }

        public void ProcessCardInfo(CancellationTokenSource cancellationTokenSource, int timeout)
        {
            Stopwatch stopWatch = new Stopwatch();

            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: (0) Transaction Start  - ################");

                stopWatch.Start();

                // task 1
                Task<(int, int)> result = device.ProcessContactlessTransaction(cancellationTokenSource.Token, timeout);

                if (result.Result.Item2 == 0x9000)
                {
                    Console.WriteLine("{0}: (2) SimulatorDevice:ProcessCLessTrans  - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Result.Item1, result.Result.Item2);

                    // task 2
                    result = device.ContinueContactlessTransaction(cancellationTokenSource.Token, timeout);

                    if (result.Result.Item2 == 0x9000)
                    {
                        Console.WriteLine("{0}: (4) SimulatorDevice:ContinueCLessTrans - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Result.Item1, result.Result.Item2);

                        // task 3
                        result = device.CompleteContactlessTransaction(cancellationTokenSource.Token, timeout);

                        if (result.Result.Item2 == 0x9000)
                        {
                            Console.WriteLine("{0}: (6) SimulatorDevice:CompleteCLessTrans - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Result.Item1, result.Result.Item2);
                        }
                    }
                }
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"SimulatorDevice::ProcessCardInfo() - {e.GetType().Name}='{e.Message}'");
                }
            }
            finally
            {
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds);
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: TRANS elapsed time     - {elapsedTime}");
            }
        }

        public void GetZip(CancellationTokenSource cancellationTokenSource, int timeout)
        {
            Stopwatch stopWatch = new Stopwatch();

            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: (0) Transaction Start  - ################");

                stopWatch.Start();

                // task 1
                Task<(DeviceInfoObject, int)> deviceInfo = device.GetDeviceInfo(cancellationTokenSource.Token, timeout);

                if (deviceInfo.Result.Item2 == 0x9000)
                {
                    Console.WriteLine("{0}: (2) SimulatorDevice:GetDeviceInfo - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), (int)deviceInfo.Result.Item1.Status, deviceInfo.Result.Item2);

                    // task 2
                    Task<(int, int)> result = device.GetZip(cancellationTokenSource.Token, timeout);

                    if (result.Result.Item2 == 0x9000)
                    {
                        Console.WriteLine("{0}: (4) SimulatorDevice:GetZip - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Result.Item1, result.Result.Item2);
                    }
                }
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"SimulatorDevice::GetZip - {e.GetType().Name}='{e.Message}'");
                }
            }
            finally
            {
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds);
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: TRANS elapsed time     - {elapsedTime}");
            }
        }
    }
}