using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TaskHandler.Extensions;
using TaskHandler.Helpers;

namespace TaskHandler.Handler
{
    public class TaskEventHandler
    {
        private int _ResponseTagsHandlersSubscribed = 0;
        private int _ResponseCLessHandlersSubscribed = 0;

        public TaskCompletionSource<int> _CardStatusResult = null;
        public TaskCompletionSource<int> _CLessStatusResult = null;

        public delegate void ResponseTagsHandlerDelegate(int responseCode, bool cancelled = false);
        internal ResponseTagsHandlerDelegate ResponseTagsHandler = null;

        public delegate void ResponseContactlessHandlerDelegate(int responseCode, bool cancelled = false);
        internal ResponseContactlessHandlerDelegate ResponseCLessHandler = null;

        private MockEventProcessor processor = new MockEventProcessor();

        private void WriteSingleCmd(int command)
        {
            processor.MockEvent(ResponseTagsHandler, ResponseCLessHandler, command);
        }

        public int ProcessContactlessTransaction()
        {
            _CardStatusResult = new TaskCompletionSource<int>();
            _ResponseTagsHandlersSubscribed++;
            ResponseTagsHandler += CardStatusHandler;

            _CLessStatusResult = new TaskCompletionSource<int>();
            _ResponseCLessHandlersSubscribed++;
            ResponseCLessHandler += ContactlessStatusHandler;

            // 0x0003 - CardStatus
            // 0x9000 - CLessStatus
            int command = 0x9000;
            Console.WriteLine("{0}: ProcessCLessTrans - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), command);
            WriteSingleCmd(command);

            int cardStatus = _CardStatusResult.Task.Result;

            ResponseTagsHandler -= CardStatusHandler;
            _ResponseTagsHandlersSubscribed--;

            ResponseCLessHandler -= ContactlessStatusHandler;
            _ResponseCLessHandlersSubscribed--;

            return cardStatus;
        }

        public async Task<int> ContinueContactlessTransaction(int timeout)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: CLESS timer start - ----------------");

            int cardStatus = (int)StatusCodes.VipaSW1SW2Codes.Failure;

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                int command = 0x0003;
                Console.WriteLine("{0}: Continue CLess    - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), command);
                WriteSingleCmd(command);

                var cancelTokenSource = new CancellationTokenSource(5000);
                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancelTokenSource.Token, 2000);

                cardStatus = _CLessStatusResult.Task.Result;

                ResponseCLessHandler -= ContactlessStatusHandler;
                _ResponseCLessHandlersSubscribed--;

            }
            catch (Exception e)
            {
                Console.WriteLine("\r\n=========================== TIMEOUT ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("=====================================================================\r\n");
            }
            finally
            {
                stopWatch.Stop();
                TimeSpan ts = stopWatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds);
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: CLESS timer stop  - {elapsedTime}");
            }

            return cardStatus;
        }

        public void CardStatusHandler(int responseCode, bool cancelled = false)
        {
            _CardStatusResult?.TrySetResult(responseCode);
        }

        public void ContactlessStatusHandler(int responseCode, bool cancelled = false)
        {
            _CLessStatusResult?.TrySetResult(responseCode);
        }
    }
}
