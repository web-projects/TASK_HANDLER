using System;
using TaskHandler.Handler;

namespace TaskHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: Transaction Start - ################");

            TaskEventHandler handler = new TaskEventHandler();
            int result = handler.ProcessContactlessTransaction();

            Console.WriteLine("{0}: ReadCard          - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result);

            if (result == 0x9000)
            {
                result = handler.ContinueContactlessTransaction(5000).Result;
                Console.WriteLine("{0}: CLess Transaction - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), result);
            }

            //Console.WriteLine("\r\nPress any key to exit...");
            //Console.ReadKey();
        }
    }
}
