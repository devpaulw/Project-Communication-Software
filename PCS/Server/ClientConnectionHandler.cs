using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    internal class ClientConnectionHandler
    {
        private readonly Member m_signedInMember = Member.Unknown;
        private readonly bool signedIn = false;

        public ClientConnectionHandler(PcsClient client, 
            Action<Member> signIn, 
            Action<BroadcastMessage> addMessage, 
            Action<PcsClient, Member> disconnect)
        {
            bool connected = true;
            while (connected)
            {
                try
                {
                    Packet packet = client.ReceivePacket();

                    switch (packet)
                    {
                        case SignInPacket signInPacket:
                            {
                                m_signedInMember = signInPacket.Member;
                                signIn(m_signedInMember);
                                signedIn = true;
                            }
                            break;
                        case MessagePacket messagePacket when signedIn:
                            {
                                var message = new BroadcastMessage(messagePacket.Message, DateTime.Now, m_signedInMember);
                                addMessage(message);
                            }
                            break;
                        case DisconnectPacket _ when signedIn:
                            {
                                connected = false;
                            }
                            break;
                        case TaskPacket taskPacket:
                            Console.WriteLine("HELLO TASK");
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
