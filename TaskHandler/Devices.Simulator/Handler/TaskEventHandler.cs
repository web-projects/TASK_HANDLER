using System;
using System.Threading;
using System.Threading.Tasks;
using TaskHandler.Devices.Simulator.Helpers;
using TaskHandler.Extensions;
using TaskHandler.Helpers;

namespace TaskHandler.Handler
{
    public class TaskEventHandler
    {
        #region --- attributes ---
        private const int TASK_DELAY = 1000;

        private int _ResponseTagsHandlersSubscribed = 0;
        private int _ResponseCLessHandlersSubscribed = 0;

        public TaskCompletionSource<int> _DeviceIdentifier = null;
        public TaskCompletionSource<int> _CardStatusResult = null;
        public TaskCompletionSource<int> _CLessStatusResult = null;

        public delegate void ResponseTagsHandlerDelegate(int responseCode, bool cancelled = false);
        internal ResponseTagsHandlerDelegate ResponseTagsHandler = null;

        public delegate void ResponseContactlessHandlerDelegate(int responseCode, bool cancelled = false);
        internal ResponseContactlessHandlerDelegate ResponseCLessHandler = null;

        private MockEventProcessor processor = new MockEventProcessor();
        #endregion --- attributes ---

        private async Task WriteSingleCmdAsync(int command)
        {
            await Task.Run(async delegate
            {
                await Task.Delay(TASK_DELAY);
                processor.MockEvent(ResponseTagsHandler, ResponseCLessHandler, command);
                return Task.CompletedTask;
            });
        }

        public async Task<(DeviceInfoObject, int)> GetDeviceInfo(CancellationToken cancellationToken, int timeout)
        {
            (DeviceInfoObject, int) deviceInfo = (null, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _DeviceIdentifier = new TaskCompletionSource<int>();
                _ResponseTagsHandlersSubscribed++;
                ResponseTagsHandler += GetDeviceInfoResponseHandler;

                // 0x0003 - CardStatus
                // 0x9000 - DeviceInfoStatus
                int command = 0x9000;
                _ = WriteSingleCmdAsync(command);

                await TaskCompletionSourceExtension.WaitAsync(_DeviceIdentifier, cancellationToken, timeout, true);

                var deviceResult = _DeviceIdentifier.Task.Result;
                Console.WriteLine("{0}: (1) TaskEventHandler::GetDeviceInfo - result=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), deviceResult);

                if (deviceResult == command)
                {
                    deviceInfo.Item1 = new DeviceInfoObject()
                    {
                        Status = StatusCodes.VipaSW1SW2Codes.Success
                    };
                    deviceInfo.Item2 = (int)StatusCodes.VipaSW1SW2Codes.Success;
                }

                ResponseTagsHandler -= GetDeviceInfoResponseHandler;
                _ResponseTagsHandlersSubscribed--;
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("\r\n=========================== GETDEVICEINFO ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("===============================================================================\r\n");
            }
            catch (OperationCanceledException op)
            {
                Console.WriteLine("{0}: (1) TaskEventHandler::GetDeviceInfo - EXCEPTION=[{1}]", DateTime.Now.ToString("yyyyMMdd:HHmmss"), op.Message);
            }

            return deviceInfo;
        }

        public async Task<(int, int)> ProcessContactlessTransaction(CancellationToken cancellationToken, int timeout)
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
                _ = WriteSingleCmdAsync(command);

                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancellationToken, timeout, true);

                var deviceResult = _CLessStatusResult.Task.Result;
                Console.WriteLine("{0}: (1) TaskEventHandler::ProcessCLessTrans - result=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), deviceResult);

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
                Console.WriteLine("\r\n=========================== PROCESSCONTACTLESSTRANSACTION ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("===============================================================================================\r\n");
            }
            catch (OperationCanceledException op)
            {
                Console.WriteLine("{0}: (1) TaskEventHandler::ProcessContactlessTransaction - EXCEPTION=[{1}]", DateTime.Now.ToString("yyyyMMdd:HHmmss"), op.Message);
            }

            return cardStatus;
        }

        public async Task<(int, int)> ContinueContactlessTransaction(CancellationToken cancellationToken, int timeout)
        {
            (int, int) cardStatus = (0, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                int command = 0x0003;
                _ = WriteSingleCmdAsync(command);

                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancellationToken, timeout, true);

                var deviceResult = _CLessStatusResult.Task.Result;
                Console.WriteLine("{0}: (3) TaskEventHandler::ContinueContactlessTransaction - result=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), deviceResult);

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
                Console.WriteLine("\r\n=========================== CONTINUECONTACTLESSTRANSACTION ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("================================================================================================\r\n");
            }
            catch (OperationCanceledException op)
            {
                Console.WriteLine("{0}: (3) TaskEventHandler::ContinueContactlessTransaction - EXCEPTION=[{1}]", DateTime.Now.ToString("yyyyMMdd:HHmmss"), op.Message);
            }

            return cardStatus;
        }

        public async Task<(int, int)> CompleteContactlessTransaction(CancellationToken cancellationToken, int timeout)
        {
            (int, int) cardStatus = (0, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                int command = 0x0003;
                _ = WriteSingleCmdAsync(command);

                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancellationToken, timeout, true);

                var deviceResult = _CLessStatusResult.Task.Result;
                Console.WriteLine("{0}: (5) TaskEventHandler::CompleteContactlessTransaction - result=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), deviceResult);

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
                Console.WriteLine("\r\n=========================== COMPLETECONTACTLESSTRANSACTION ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("================================================================================================\r\n");
            }
            catch (OperationCanceledException op)
            {
                Console.WriteLine("{0}: (5) TaskEventHandler::CompleteContactlessTransaction - EXCEPTION=[{1}]", DateTime.Now.ToString("yyyyMMdd:HHmmss"), op.Message);
            }

            return cardStatus;
        }

        public async Task<(int, int)> GetZip(CancellationToken cancellationToken, int timeout)
        {
            (int, int) cardStatus = (0, (int)StatusCodes.VipaSW1SW2Codes.Failure);

            try
            {
                _CLessStatusResult = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                _ResponseCLessHandlersSubscribed++;
                ResponseCLessHandler += ContactlessStatusHandler;

                int command = 0x0003;
                _ = WriteSingleCmdAsync(command);

                await TaskCompletionSourceExtension.WaitAsync(_CLessStatusResult, cancellationToken, timeout, true);

                var deviceResult = _CLessStatusResult.Task.Result;
                Console.WriteLine("{0}: (1) TaskEventHandler::GetZip - result=0x0{1:X4}", DateTime.Now.ToString("yyyyMMdd:HHmmss"), deviceResult);

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
                Console.WriteLine("\r\n=========================== GETZIP ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("========================================================================\r\n");
            }
            catch (OperationCanceledException op)
            {
                Console.WriteLine("{0}: (1) TaskEventHandler::GetZip - EXCEPTION=[{1}]", DateTime.Now.ToString("yyyyMMdd:HHmmss"), op.Message);
            }

            return cardStatus;
        }

        #region --- delegate handlers ---
        public void GetDeviceInfoResponseHandler(int responseCode, bool cancelled = false)
        {
            try
            {
                _DeviceIdentifier?.TrySetResult(responseCode);
            }
            catch (TimeoutException e)
            {
                Console.WriteLine($"GetDeviceInfoResponseHandler: exception={e.Message}");
            }
        }

        public void CardStatusHandler(int responseCode, bool cancelled = false)
        {
            _CardStatusResult?.TrySetResult(responseCode);

        }

        public void ContactlessStatusHandler(int responseCode, bool cancelled = false)
        {
            _CLessStatusResult?.TrySetResult(responseCode);
        }
        #endregion --- delegate handlers ---
    }
}
