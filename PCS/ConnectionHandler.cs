using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class ConnectionHandler
    {
        private readonly Member m_signedInMember = Member.Unknown;

        public ConnectionHandler(PcsClient client, Action<Member> clientConnect, Action<Message> addMessage, Action<PcsClient> clientDisconnect)
        {
            while (true)
            {
                try
                {
                    string receivedData = client.Receive();

                    Member signedInMember = DataPacket.TryGetSignedInMember(receivedData);
                    var clientMessage = DataPacket.TryGetClientMessage(receivedData);
                    bool shouldDisconnect = DataPacket.WishDisconnect(receivedData);

                    if (signedInMember != null)
                    {
                        m_signedInMember = signedInMember;
                        clientConnect(m_signedInMember);
                    }
                    else if (clientMessage != null) // TODO System can't send message while client not connected
                    {
                        var message = new Message(clientMessage.Text, clientMessage.ChannelTitle, DateTime.Now, m_signedInMember); // TODO Should not be DateTime.Now but the dite sent by the client explicitely with FileTime
                        addMessage(message);
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
