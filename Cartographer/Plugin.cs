using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
    public class Plugin : IPlugin
    {
        private static readonly Location LOCATION_TOP_OF_NEXUS_TUTORIAL = new Location(134.5f, 98.5f);

        private string mapName = string.Empty;
        private bool firstUpdate = false;

        private bool blockNextGotoAck = false;

        public string GetAuthor() => "natiiix";

        public string[] GetCommands() => new string[0];

        public string GetDescription() => "Cartographer bot by natiiix";

        public string GetName() => "Cartographer Bot";

        public void Initialize(Proxy proxy)
        {
            proxy.HookPacket<MapInfoPacket>(OnMapInfo);
            proxy.HookPacket<UpdatePacket>(OnUpdate);
            proxy.HookPacket<GotoAckPacket>(OnGotoAck);
        }

        private void OnMapInfo(Client client, MapInfoPacket p)
        {
            mapName = p.Name;
            firstUpdate = true;
        }

        private void OnUpdate(Client client, UpdatePacket p)
        {
            if (firstUpdate && client.Connected)
            {
                firstUpdate = false;

                switch (mapName)
                {
                    case "Nexus Explanation":
                        PluginUtils.Delay(100, () => MoveTo(client, LOCATION_TOP_OF_NEXUS_TUTORIAL));
                        break;

                    default:
                        PluginUtils.Delay(100, () => client.SendChatMessage("/nexustutorial"));
                        break;
                }
            }
        }

        private void OnGotoAck(Client client, Packet p)
        {
            if (blockNextGotoAck)
            {
                p.Send = false;
                blockNextGotoAck = false;
            }
        }

        private void MoveTo(Client client, Location targetLocation)
        {
            DateTime dtLast = DateTime.Now;

            while (!StepTowardsTarget(client, targetLocation, DateTime.Now - dtLast) && client.Connected)
            {
                dtLast = DateTime.Now;
                Thread.Sleep(20);
            }

            client.SendChatMessage("/tutorial");
        }

        private bool StepTowardsTarget(Client client, Location targetLocation, TimeSpan deltaTime)
        {
            const double MIN_MOVE_SPEED = 4;
            const double MAX_MOVE_SPEED = 9.6;

            // Character speed in tiles per second
            double playerSpeed = MIN_MOVE_SPEED + ((client.PlayerData.Speed / 75.0) * (MAX_MOVE_SPEED - MIN_MOVE_SPEED));

            // Number of tiles the character can move in this tick
            //double maxTickDistance = playerSpeed * deltaTime.TotalSeconds;
            double maxTickDistance = playerSpeed * 1.3 * deltaTime.TotalSeconds;

            // Get the current position of the player
            Location currentPos = client.PlayerData.Pos;

            // Calculate the distance to target
            // Using double-precision methods instead of the built-in float distance method for maximum accuracy
            double distanceToTarget = Math.Sqrt(Math.Pow(targetLocation.X - currentPos.X, 2) + Math.Pow(targetLocation.Y - currentPos.Y, 2));

            // Create a new GOTO packet
            GotoPacket gp = Packet.Create<GotoPacket>(PacketType.GOTO);

            bool withinReach = distanceToTarget <= maxTickDistance;

            // Target is within the maximum move distance
            if (withinReach)
            {
                // Move to the target
                gp.Location = targetLocation;
            }
            // Target is too far to be reached in a single tick
            else
            {
                // Calculate the partial target
                double moveScale = maxTickDistance / distanceToTarget;

                double partialX = currentPos.X + ((targetLocation.X - currentPos.X) * moveScale);
                double partialY = currentPos.Y + ((targetLocation.Y - currentPos.Y) * moveScale);

                // Move to the partial target
                gp.Location = new Location((float)partialX, (float)partialY);
            }

            // Set the object ID to the ID of the player's character
            gp.ObjectId = client.ObjectId;

            // Bock the next GOTOACK packet
            blockNextGotoAck = true;

            // Send the GOTO packet to the client
            client.SendToClient(gp);

            return withinReach;
        }
    }
}