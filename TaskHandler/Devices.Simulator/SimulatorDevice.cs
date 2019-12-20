using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TaskHandler.Handler;

namespace TaskHandler.Devices.Simulator
{
    public class SimulatorDevice
    {
        public async Task ProcessCardInfo()
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: (0) Transaction Start  - ################");

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            TaskEventHandler handler = new TaskEventHandler();

            (int, int) result = await handler.ProcessContactlessTransaction(10000);
            Console.WriteLine("{0}: (2) ProcessCLessTrans  - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Item1, result.Item2);

            if (result.Item2 == 0x9000)
            {
                result = await handler.ContinueContactlessTransaction(5000);
                Console.WriteLine("{0}: (4) ContinueCLessTrans - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Item1, result.Item2);
            }

            result = await handler.CompleteContactlessTransaction(1000);
            Console.WriteLine("{0}: (6) CompleteCLessTrans - status=0x0{1:X4}, result=0x0{2:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result.Item1, result.Item2);

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: TRANS elapsed time - {elapsedTime}");
        }
    }
}