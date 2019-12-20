using System;
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

        private void WriteSingleCmd(int command, int delay)
        {
            Task.Delay(delay).ContinueWith(t =>
            {
                processor.MockEvent(ResponseTagsHandler, ResponseCLessHandler, command);
            });
        }

        private async Task WriteSingleCmdAsync(int command, int delay)
        {
            await Task.Delay(delay).ContinueWith(t =>
            {
                processor.MockEvent(ResponseTagsHandler, ResponseCLessHandler, command);
            });
        }

        public int GetDeviceInfo()
        {
            return 0;
        }

        public async Task<(int, int)> ProcessContactlessTransaction(int timeout)
        {
            (int, int) cardStatus = (0, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>();
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                // 0x0003 - CardStatus
                // 0x9000 - CLessStatus
                int command = 0x9000;
                Console.WriteLine("{0}: (1) ProcessCLessTrans  - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), command);
                await WriteSingleCmdAsync(command, timeout / 2);

                var cancelTokenSource = new CancellationTokenSource(timeout);
                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancelTokenSource.Token, timeout);

                var deviceResult = _CLessStatusResult.Task.Result;

                if (deviceResult == command)
                {
                    cardStatus.Item1 = 0x01;
                    cardStatus.Item2 = (int)StatusCodes.VipaSW1SW2Codes.Success;
                }

                ResponseCLessHandler -= ContactlessStatusHandler;
                _ResponseCLessHandlersSubscribed--;
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("\r\n=========================== TIMEOUT ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("=====================================================================\r\n");
            }
            catch (OperationCanceledException)
            {
            }

            return cardStatus;
        }

        public async Task<(int, int)> ContinueContactlessTransaction(int timeout)
        {
            (int, int) cardStatus = (0, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                int command = 0x0003;
                Console.WriteLine("{0}: (3) Continue CLess     - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), command);
                await WriteSingleCmdAsync(command, timeout - 1000);

                var cancelTokenSource = new CancellationTokenSource(timeout);
                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancelTokenSource.Token, timeout);

                var deviceResult = _CLessStatusResult.Task.Result;

                if (deviceResult == command)
                {
                    cardStatus.Item1 = 0x01;
                    cardStatus.Item2 = (int)StatusCodes.VipaSW1SW2Codes.Success;
                }

                ResponseCLessHandler -= ContactlessStatusHandler;
                _ResponseCLessHandlersSubscribed--;

            }
            catch (TimeoutException e)
            {
                Console.WriteLine("\r\n=========================== TIMEOUT ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("=====================================================================\r\n");
            }
            catch (OperationCanceledException)
            {
            }

            return cardStatus;
        }

        public async Task<(int, int)> CompleteContactlessTransaction(int timeout)
        {
            (int, int) cardStatus = (0, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                int command = 0x0003;
                Console.WriteLine("{0}: (5) Complete CLess     - status=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), command);
                await WriteSingleCmdAsync(command, timeout - 1000);

                var cancelTokenSource = new CancellationTokenSource(timeout);
                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancelTokenSource.Token, timeout);

                var deviceResult = _CLessStatusResult.Task.Result;

                if (deviceResult == command)
                {
                    cardStatus.Item1 = 0x01;
                    cardStatus.Item2 = (int)StatusCodes.VipaSW1SW2Codes.Success;
                }

                ResponseCLessHandler -= ContactlessStatusHandler;
                _ResponseCLessHandlersSubscribed--;

            }
            catch (TimeoutException e)
            {
                Console.WriteLine("\r\n=========================== TIMEOUT ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("=====================================================================\r\n");
            }
            catch (OperationCanceledException)
            {
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
