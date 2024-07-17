using System;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using NpcHandler;

namespace SlavysMod
{
    public class Main : Script
    {
        private List<Npc> npcList = new List<Npc>();
        private readonly Server httpServer = new Server();
        private readonly int maxNpcLimit = 10;
        private bool isServerRunning = false;

        public Main()
        {
            Tick += onTick;
            KeyDown += onKeyDown;
        }
        private void onTick(object sender, EventArgs e)
        {
            // Start the server if it's not running
            if (!isServerRunning)
            {
                httpServer.Start();
                isServerRunning = true;
            }

            CommandSystem(); // Handle commands from the server
            AttackerSystem(); // Handle NPC attackers
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            // Useful for debugging 
            if (e.KeyCode == Keys.NumPad3)
            {
                Npc npc = new Npc("Slavy", PedHash.Blackops01SMY, 420, WeaponHash.Pistol);
                npcList.Add(npc);
            }

            if (e.KeyCode == Keys.NumPad1)
            {
                if (npcList.Count > 0)
                {
                    npcList[0].Delete();
                    npcList.RemoveAt(0);
                }
            }
        }

        // Takes a command from our server and handles it accordingly
        private void CommandSystem()
        {
            // Get the command from the server
            Commands cmd = httpServer.GetCommand();

            // do nothing if there is no command to work on 
            if (cmd == null)
                return;

            // If there is a command, process it
            switch (cmd.command)
            {
                case "spawn_attacker":
                    Npc npc = new Npc(cmd.username, PedHash.Blackops01SMY, 420, WeaponHash.Pistol);
                    npcList.Add(npc);

                    Logger.Log("Spawning attacker for: " + cmd.username);
                    break;
                case "test":
                    if (npcList.Count > 0)
                    {
                        npcList[0].Delete();
                        npcList.RemoveAt(0);
                    }
                    break;
                default:
                    break;
            }
        }

        private void AttackerSystem()
        {
            try
            {
                // Prevent NPC list from exceeding the limit
                if (npcList.Count > maxNpcLimit)
                {
                    npcList[0].Delete();
                    npcList.RemoveAt(0);
                }

                // Loop each NPC in our list 
                foreach (Npc npc in npcList)
                {
                    // Draw the NPC's name 
                    npc.DrawName();

                    // NPC Death Checker 
                    if (npc.GetIsDead())
                    {
                        if (npc.GetDeathTime() == null)
                        {
                            npc.SetDeathTime();
                        }
                        else
                        {
                            // If dead longer than 5 seconds, delete the NPC
                            if (DateTime.Now.Subtract(npc.GetDeathTime().Value).TotalSeconds > 5)
                            {
                                npc.Delete();
                                npcList.Remove(npc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to draw attacker names - " + ex.Message);
            }
        }
    }
}