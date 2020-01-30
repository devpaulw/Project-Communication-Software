using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    internal class ClientConnectionHandler
    {
        private readonly Member m_signedInMember = Member.Unknown;

        public ClientConnectionHandler(PcsClient client, 
            Action<Member> signIn, 
            Action<Message> addMessage, 
            Action<PcsClient, Member> disconnect)
        {
            while (true)
            {
                try
                {
                    string receivedData = client.Receive();

                    var dataPacket = new DataPacket(receivedData);

                    if (dataPacket.Type == DataPacketType.ClientSignIn)
                    {
                        Member signedInMember = dataPacket.GetMember();

                        m_signedInMember = signedInMember;
                        signIn(m_signedInMember);
                    }
                    else if (dataPacket.Type == DataPacketType.ClientMessage)
                    {
                        var clientMessage = dataPacket.GetMessage();

                        var message = new Message(clientMessage.Text, clientMessage.ChannelTitle, DateTime.Now, m_signedInMember);
                        addMessage(message);
                    }
                    else if (dataPacket.Type == DataPacketType.ClientDisconnect)
                    {
                        break;
                    }
                }
                catch (SocketException) // When An existing connection was forcibly closed by the remote host
                { // BUG DOESN'T WORK ANYMORE IN WPF (Task does not finish ?)
                    break;
                }
            }

            client.Disconnect();
            disconnect(client, m_signedInMember);
        }
    }
}
