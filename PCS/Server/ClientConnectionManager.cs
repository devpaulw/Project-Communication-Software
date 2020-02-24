using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    internal class ClientConnectionManager
    {
        public ClientConnectionManager(PcsClient client,
            Func<Member, bool> canSignIn,
            Action<BroadcastMessage> addMessage,
            Action<PcsClient, Member> disconnect)
        {
            Member signedInMember = Member.Unknown;
            bool signedIn = false;
            bool connected = true;

            while (connected)
            {
                try
                {
                    Packet packet = client.ReceivePacket();

                    switch (packet)
                    {
                        case SignInPacket signInPacket when !signedIn:
                            OnSignIn(signInPacket.Member);
                            break;
                        case MessagePacket messagePacket when signedIn:
                            OnMessageReceived(messagePacket.Message);
                            break;
                        case DisconnectPacket _ when signedIn:
                            connected = false;
                            break;
                    }
                }
                catch (SocketException) // When An existing connection was forcibly closed by the remote host
                {
                    connected = false;
                }
            }

            OnDisconnect();

            void OnSignIn(Member member)
            {
                signedInMember = member;

                if (canSignIn(member))
                {
                    signedIn = true;

                    client.SendPacket(new ResponsePacket(ResponseCode.SignInSucceeded));
                    Console.WriteLine(Messages.Server.ClientConnect, member, client.RemoteIP.Address.ToString());
                }
                else
                {
                    client.SendPacket(new ResponsePacket(ResponseCode.UnauthorizedLogin));
                    connected = false;
                }
            }

            void OnMessageReceived(Message message)
            {
                var broadcastMsg = new BroadcastMessage(message, DateTime.Now, signedInMember);
                addMessage(broadcastMsg);
            }

            void OnDisconnect()
            {
                client.Disconnect();
                disconnect(client, signedInMember);
                Console.WriteLine(Messages.Server.ClientDisconnect, signedInMember);
            }
        }
    }
}
