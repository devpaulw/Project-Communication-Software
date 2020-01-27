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

                    var dataPacket = new DataPacket(receivedData);

                    if (dataPacket.Type == DataPacketType.ClientSignIn)
                    {
                        Member signedInMember = dataPacket.GetSignedInMember();

                        m_signedInMember = signedInMember;
                        clientConnect(m_signedInMember);
                    }
                    else if (dataPacket.Type == DataPacketType.ClientMessage)
                    {
                        var clientMessage = dataPacket.GetClientMessage();

                        var message = new Message(clientMessage.Text, clientMessage.ChannelTitle, DateTime.Now, m_signedInMember); // TODO Should not be DateTime.Now but the dite sent by the client explicitely with FileTime
                        addMessage(message);
                    }
                    else if (dataPacket.Type == DataPacketType.ClientDisconnect)
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
