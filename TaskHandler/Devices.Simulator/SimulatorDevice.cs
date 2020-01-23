using System;
using System.Diagnostics;
using System.Threading;
using TaskHandler.Handler;

namespace TaskHandler.Devices.Simulator
{
    public class SimulatorDevice
    {
        public void ProcessCardInfo(CancellationTokenSource cancellationTokenSource, int timeout)
        {
            Stopwatch stopWatch = new Stopwatch();

            try
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: (0) Transaction Start  - ################");

                stopWatch.Start();

                TaskEventHandler device = new TaskEventHandler();

                (int, int) result = device.ProcessContactlessTransaction(cancellationTokenSource.Token, timeout);
                Console.WriteLine("{0}: (2) ProcessCLessTrans  - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Item1, result.Item2);

                if (result.Item2 == 0x9000)
                {
                    result = device.ContinueContactlessTransaction(cancellationTokenSource.Token, timeout);
                    Console.WriteLine("{0}: (4) ContinueCLessTrans - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Item1, result.Item2);
                }

                result = device.CompleteContactlessTransaction(cancellationTokenSource.Token, timeout);
                Console.WriteLine("{0}: (6) CompleteCLessTrans - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Item1, result.Item2);
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

                TaskEventHandler device = new TaskEventHandler();
                (int, int) result = device.GetZip(cancellationTokenSource.Token, timeout);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine($"SimulatorDevice::GetZip() - {e.GetType().Name}='{e.Message}'");
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