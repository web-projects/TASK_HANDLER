using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            _ResponseTagsHandlersSubscribed ++;
            ResponseTagsHandler += CardStatusHandler;

            _CLessStatusResult = new TaskCompletionSource<int>();
            _ResponseCLessHandlersSubscribed ++;
            ResponseCLessHandler += ContactlessStatusHandler;

            // 0x0003 - CardStatus
            // 0x9000 - CLessStatus
            int command = 0x9000;
            Console.WriteLine("cmd: ProcessCLessTrans - status=0x0{0:X4}", command);
            WriteSingleCmd(command);

            int cardStatus = _CardStatusResult.Task.Result;

            ResponseTagsHandler -= CardStatusHandler;
            _ResponseTagsHandlersSubscribed --;

            ResponseCLessHandler -= ContactlessStatusHandler;
            _ResponseCLessHandlersSubscribed --;

            return cardStatus;
        }

        public int ContinueContactlessTransaction()
        {
            _CLessStatusResult = new TaskCompletionSource<int>();
            _ResponseCLessHandlersSubscribed ++;
            ResponseCLessHandler += ContactlessStatusHandler;

            int command = 0x0003;
            Console.WriteLine("cmd: Continue CLess    - status=0x0{0:X4}", command);
            WriteSingleCmd(command);

            int cardStatus = _CLessStatusResult.Task.Result;

            ResponseCLessHandler -= ContactlessStatusHandler;
            _ResponseCLessHandlersSubscribed --;

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
