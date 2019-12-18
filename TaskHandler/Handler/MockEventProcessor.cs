using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TaskHandler.Handler
{
    public class MockEventProcessor
    {
        internal TaskEventHandler.ResponseTagsHandlerDelegate ResponseTagsHandler = null;
        internal TaskEventHandler.ResponseContactlessHandlerDelegate ResponseContactlessHandler = null;

        int responseCode = 0x03;

        public void MockEvent(TaskEventHandler.ResponseTagsHandlerDelegate responseTagsHandler, 
            TaskEventHandler.ResponseContactlessHandlerDelegate responseContactlessHandler, int command)
        {
            ResponseTagsHandler = responseTagsHandler;
            ResponseContactlessHandler = responseContactlessHandler;

            Thread.Sleep(2000);

            if (ResponseTagsHandler != null)
            {
                ResponseTagsHandler?.Invoke(responseCode = command);
            }
            else if(ResponseContactlessHandler != null)
            {
                ResponseContactlessHandler.Invoke(responseCode = command);
            }

            responseCode = (responseCode == 0x03) ? 0x9000 : 0x03;
        }
    }
}
