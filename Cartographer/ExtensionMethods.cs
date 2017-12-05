using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib_K_Relay;
using Lib_K_Relay.Utilities;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Networking.Packets.DataObjects;

namespace Cartographer
{
    public static class ExtensionMethods
    {
        public static void TeleportToNexus(this Client client)
        {
            // Send an escape packet to the server
            client.SendToServer(Packet.Create(PacketType.ESCAPE));
        }

        public static void SendChatMessage(this Client client, string text)
        {
            // Create the packet
            PlayerTextPacket ptp = Packet.Create<PlayerTextPacket>(PacketType.PLAYERTEXT);
            // Set the text of the message
            ptp.Text = text;
            // Send the packet to the server
            client.SendToServer(ptp);
        }
    }
}