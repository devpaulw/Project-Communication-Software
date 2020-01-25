using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class ConnectionHandler
    {
        private readonly Member m_signedInMember = Member.Unknown;

        public ConnectionHandler(PcsClient client, Action<ServerMessage> addMessage, Action<PcsClient> clientDisconnect)
        {
            while (true)
            {
                try
                {
                    string receivedData = client.Receive();

                    Member signedInMember = DataPacket.TryGetSignedInMember(receivedData);
                    var clientMessage = DataPacket.TryGetClientMessage(receivedData);
                    bool shouldDisconnect = DataPacket.TryDisconnect(receivedData);

                    if (signedInMember != null)
                    {
                        m_signedInMember = signedInMember;

                        Console.WriteLine("{0} connected!", signedInMember); // TODO: SHould be another palce
                    }
                    else if (clientMessage != null)
                    {
                        var message = new ServerMessage(m_signedInMember, clientMessage.Text, clientMessage.ChannelTitle);
                        addMessage(message);

                        Console.WriteLine("{0} sent from channel <{2}>: {1}", m_signedInMember, clientMessage, clientMessage.ChannelTitle);
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
