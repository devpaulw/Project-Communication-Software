using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class ConnectionHandler
    {

        public ConnectionHandler(PcsClient client, Action<Message> addMessage, Action<PcsClient> clientDisconnect)
        {
            Member identifiedMember = Member.FromTextData(client.Receive());

            Console.WriteLine("{0} connected!", identifiedMember);

            while (true)
            {
                try
                {
                    string receivedMsg = client.Receive();

                    Console.WriteLine("{0} sent: {1}", identifiedMember, receivedMsg);

                    var message = new Message(identifiedMember, receivedMsg);
                    addMessage(message);
                }
                catch
                {
                    break;
                }
            }

            client.Disconnect();
            clientDisconnect(client);

            Console.WriteLine("{0} disconnected.", identifiedMember);
        }

        //public ConnectionHandler(PcsClient client, Action<Message> addMessage, Action<PcsClient> clientDisconnect)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            string receivedMsg = client.Receive();

        //            Console.WriteLine("{0} sent: {1}", identifiedMember, receivedMsg);

        //            var message = new Message(identifiedMember, receivedMsg);
        //            addMessage(message);


        //        }
        //        catch
        //        {
        //            break;
        //        }
        //    }
        //}
    }
}
