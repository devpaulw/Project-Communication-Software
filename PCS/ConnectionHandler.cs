using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class ConnectionHandler
    {
        readonly Member m_signedInMember = Member.Unknown;

        public ConnectionHandler(PcsClient client, Action<Message> addMessage, Action<PcsClient> clientDisconnect)
        {
            while (true)
            {
                try
                {
                    string receivedData = client.Receive();

                    Member signedInMember = DataPacket.TryGetSignedInMember(receivedData);
                    string text = DataPacket.TryGetText(receivedData);
                    bool shouldDisconnect = DataPacket.TryDisconnect(receivedData);

                    if (signedInMember != null)
                    {
                        m_signedInMember = signedInMember;

                        Console.WriteLine("{0} connected!", signedInMember);
                    }
                    else if (text != null)
                    {
                        var message = new Message(m_signedInMember, text);
                        addMessage(message);

                        Console.WriteLine("{0} sent: {1}", m_signedInMember, text);
                    }
                    else if (shouldDisconnect != false)
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }

            client.Disconnect();
            clientDisconnect(client);

            Console.WriteLine("{0} disconnected.", m_signedInMember);
        }
    }
}
