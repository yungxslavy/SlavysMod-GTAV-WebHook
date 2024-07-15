using System;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using CustomUtil;
using NpcHandler;

namespace SlavysMod
{
    public class Main : Script
    {
        private List<Npc> npcList = new List<Npc>();
        private int maxNpcLimit = 10;

        public Main()
        {
            Tick += onTick;
            KeyDown += onKeyDown;
        }
        private void onTick(object sender, EventArgs e)
        {
            AttackerSystem();
        }
        private void onKeyDown(object sender, KeyEventArgs e)
        {
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