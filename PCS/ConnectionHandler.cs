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

        //Member signedInMember;

        //public ConnectionHandler(PcsClient client, Action<Message> addMessage, Action<PcsClient> clientDisconnect)
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            string receivedData = client.Receive();

        //            //for (Member signedInMember = DataPacket.TryGetSignedInMember(receivedData); 
        //            //    signedInMember != null;
        //            //    signedInMember == null)
        //            //{

        //            //}

        //            Member signedInMember = DataPacket.TryGetSignedInMember(receivedData);
        //            string text = DataPacket.TryGetText(receivedData);
        //            bool shouldDisconnect = DataPacket.TryDisconnect(receivedData);

        //            if (signedInMember != null)
        //            {
        //                this.signedInMember = signedInMember;
        //            }
        //            else if (text != null)
        //            {
        //                var message = new Message(signedInMember, text);
        //                addMessage(message);

        //                Console.WriteLine("{0} sent: {1}", signedInMember, text);
        //            }
        //            else if (shouldDisconnect != false)
        //            {
        //                break;
        //            }
        //        }
        //        catch
        //        {
        //            break;
        //        }
        //    }

        //    client.Disconnect();
        //    clientDisconnect(client);

        //    Console.WriteLine("{0} disconnected.", signedInMember);
        //}
    }
}
