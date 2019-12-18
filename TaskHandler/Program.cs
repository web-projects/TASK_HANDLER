using System;
using TaskHandler.Handler;

namespace TaskHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskEventHandler handler = new TaskEventHandler();
            int result = handler.ProcessContactlessTransaction();

            Console.WriteLine("cmd: ReadCard          - status=0x0{0:X4}", result);

            if (result == 0x9000)
            {
                result = handler.ContinueContactlessTransaction();
                Console.WriteLine("cmd: CLess Transaction - status=0x0{0:X4}", result);
            }

            Console.WriteLine("\r\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
