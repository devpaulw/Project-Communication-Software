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
            bool connected = true;
            while (connected)
            {
                try
                {
                    var dataPacket = client.ReceivePacket();

                    switch (dataPacket.Type)
                    {
                        case PacketType.MemberSignIn:
                            {
                                m_signedInMember = (Member)dataPacket.Object;
                                signIn(m_signedInMember);
                            }
                            break;
                        case PacketType.Message:
                            {
                                var clientMessage = (Message)dataPacket.Object;
                                var serverMessage = new Message(clientMessage.Text, clientMessage.ChannelName, DateTime.Now, clientMessage.Author);
                                addMessage(serverMessage);
                            }
                            break;
                        case PacketType.ClientDisconnect:
                            {
                                connected = false;
                            }
                            break;
                    }
                }
                catch (SocketException) // When An existing connection was forcibly closed by the remote host
                {
                    connected = false;
                }
            }

            client.Disconnect();
            disconnect(client, m_signedInMember);
        }
    }
}
