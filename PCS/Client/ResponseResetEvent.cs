﻿using PCS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace PCS
{
    class ResponseResetEvent
    {
        const int timeoutSeconds = 4;
        readonly List<Response> waitingResponses = new List<Response>();
        bool isFilled = false;
        
        public ResponseResetEvent()
        {

        }

        public void SetResponse(Response response)
        {
            lock (waitingResponses)
            {
                waitingResponses.Add(response);
            }
            isFilled = true;
        }

        public Response WaitResponse()
        {
            DateTime initialTime = DateTime.Now;

            while (!isFilled)
            {
                if (DateTime.Now.Ticks - initialTime.Ticks > 10000000 * timeoutSeconds)
                    throw new TimeoutException(Messages.Exceptions.NoResponseTimeout);
            }

            Response response;
            lock (waitingResponses)
            {
                response = waitingResponses[0];
                waitingResponses.RemoveAt(0);

                isFilled = waitingResponses.Any();
            }

            return response;
        }
    }
}
