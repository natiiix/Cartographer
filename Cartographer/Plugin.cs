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
        private static readonly Location NEXUS_TUTORIAL_TARGET_LOCATION = new Location(134f, 99.5f);

        private bool enabled;
        private FlashClient flash;
        private string currentMapName;
        private bool newMap;

        public string GetAuthor() => "natiiix";

        public string[] GetCommands() => new string[]
        {
            "/cart - Toggles the cartographer bot"
        };

        public string GetDescription() => "Cartographer bot by natiiix";

        public string GetName() => "Cartographer";

        public void Initialize(Proxy proxy)
        {
            enabled = false;

            // Plugin toggle command
            proxy.HookCommand("cart", delegate
            {
                // If enabled
                if (enabled = !enabled)
                {
                    Log("Starting");
                    flash = new FlashClient();
                    newMap = true;
                }
                // If disabled
                else
                {
                    Log("Stopping");
                    flash.StopMoving();
                }
            });

            proxy.HookPacket<MapInfoPacket>(OnMapInfo);
            proxy.HookPacket<UpdatePacket>(OnUpdate);
        }

        private void OnMapInfo(Client client, MapInfoPacket p)
        {
            // Map has changed
            currentMapName = p.Name;
            newMap = true;
        }

        private void OnUpdate(Client client, UpdatePacket p)
        {
            // If the plugin is enabled, player is on a new map and the client is connected
            if (enabled && newMap && client.Connected)
            {
                // Correct map
                if (currentMapName == "Nexus Explanation")
                {
                    // Start moving towards the target on a background thread
                    Log("Starting to move");
                    new Task(() => MoveToTarget(client)).Start();
                }
                // Any other map
                else
                {
                    // There needs to be some delay before the teleport to avoid an exception in K-Relay
                    Log("Teleporting to Nexus Explanation");
                    PluginUtils.Delay(1000, () => client?.SendChatMessage("/nexustutorial"));
                }

                // This is no longer a new map
                newMap = false;
            }
        }

        // Send the log message to the K-Relay log with this plugin's name as a source
        private void Log(string text) => PluginUtils.Log(GetName(), text);

        private void MoveToTarget(Client client)
        {
            // Get the current time and player position
            Location lastPlayerPos = client.PlayerData.Pos;
            DateTime dtLastMove = DateTime.Now;

            // Move towards the target location
            // Loop breaks when the target location is reached or when the client connection drops
            while (enabled && client.Connected && flash.MoveInDirection(
                NEXUS_TUTORIAL_TARGET_LOCATION.X - client.PlayerData.Pos.X,
                NEXUS_TUTORIAL_TARGET_LOCATION.Y - client.PlayerData.Pos.Y))
            {
                // Add some delay between the iterations
                Thread.Sleep(100);

                // Player has moved since the last iteration
                if (lastPlayerPos.X != client.PlayerData.Pos.X || lastPlayerPos.Y != client.PlayerData.Pos.Y)
                {
                    // Update the current time and player position
                    lastPlayerPos = client.PlayerData.Pos;
                    dtLastMove = DateTime.Now;
                }
                // Player hasn't moved for some time
                else if ((DateTime.Now - dtLastMove).TotalSeconds >= 0.5)
                {
                    // Stop moving to reset the key states
                    Log("Restarting movement");
                    flash.StopMoving();
                }
            }

            // Stop all movement
            Log("Stopping all movement");
            flash.StopMoving();

            // If the user wants to continue
            if (enabled)
            {
                // Teleport to Tutorial
                Log("Teleporting to Tutorial");
                client?.SendChatMessage("/tutorial");
            }
        }
    }
}