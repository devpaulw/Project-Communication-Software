using PCS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PCS
{
    class ResponseResetEvent
    {
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
            while (!isFilled) ; // TODO IDEA Timeout system here

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
