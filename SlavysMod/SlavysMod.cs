using System;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using NpcHandler;
using GTA.UI;
using System.Drawing;

namespace SlavysMod
{
    public class Main : Script
    {
        private readonly List<Npc> npcList = new List<Npc>();
        private readonly List<VehicleNpc> vehicleNpcList = new List<VehicleNpc>();
        private readonly EffectTracker effectTracker = new EffectTracker();
        private readonly Server httpServer = new Server();
        private readonly int maxNpcLimit = 25;
        private readonly int maxVehicleLimit = 10;
        private bool isServerRunning = false;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }
        private void OnTick(object sender, EventArgs e)
        {
            // Start the server if it's not running
            if (!isServerRunning)
            {
                httpServer.Start();
                isServerRunning = true;
            }

            CommandSystem(); // Handle commands from the server

            Utils.DrawTextOnScreen($"Gravity Duration: {effectTracker.GravityEffect.GetRemainingDuration()}" +
                $"\nSpeedBoost Duration: {effectTracker.SpeedBoostEffect.GetRemainingDuration()}",
                new PointF(GTA.UI.Screen.Width - 10, 50),
                Alignment.Right
            );

            Utils.AttackerSystem(npcList, maxNpcLimit); // Handle NPC attackers
            Utils.VehicleAttackerSystem(vehicleNpcList, maxVehicleLimit); // Handle Vehicle Npc attackers
            Utils.EffectSystem(effectTracker); // Handle effects
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Useful for debugging 
            if (e.KeyCode == Keys.NumPad3)
            {
                effectTracker.AddEffectDuration(EffectType.Gravity, 3);
            }

            if (e.KeyCode == Keys.NumPad1)
            {
                World.GravityLevel = 9.8f;
                if (npcList.Count > 0)
                {
                    npcList[0].CurrentPed.Delete();
                    npcList.RemoveAt(0);
                }
                if (vehicleNpcList.Count > 0)
                {
                    vehicleNpcList[0].Delete();
                    vehicleNpcList.RemoveAt(0);
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

            // Here is where the commands are processed and executed
            switch (cmd.command)
            {
                case "spawn_meleeattacker":
                    Npc meleeNpc = new Npc(cmd.username, 
                        Utils.GetRandomPedHash(Utils.MeleePedHashList), 
                        Utils.GetRandomWeaponHash(Utils.MeleeWeaponHashList),
                        420
                    );
                    npcList.Add(meleeNpc);
                    Logger.Log("Spawning attacker for: " + cmd.username);
                    break;

                case "spawn_armedattacker":
                    Npc armedNpc = new Npc(cmd.username, 
                        Utils.GetRandomPedHash(Utils.ArmedPedHashList),
                        Utils.GetRandomWeaponHash(Utils.FirearmWeaponHashList), 
                        500
                    );
                    npcList.Add(armedNpc);
                    Logger.Log("Spawning attacker for: " + cmd.username);
                    break;

                case "spawn_gangvehicle":
                    VehicleNpc vehicle = new VehicleNpc(cmd.username, VehicleHash.Baller);
                    vehicleNpcList.Add(vehicle);
                    Logger.Log("Spawning gang vehicle");
                    break;

                case "spawn_juggernaut":
                    Npc juggernaut = new Npc(cmd.command, PedHash.Juggernaut01M, WeaponHash.Minigun, 5000);
                    juggernaut.CurrentPed.CanSufferCriticalHits = false;
                    juggernaut.CurrentPed.CanRagdoll = false;
                    juggernaut.CurrentPed.IsExplosionProof = true;
                    juggernaut.CurrentPed.FiringPattern = FiringPattern.FullAuto;
                    npcList.Add(juggernaut);
                    Logger.Log("Spawning juggernaut for: " + cmd.username);
                    break;

                case "spawn_astro":
                    Npc alien = new Npc(cmd.username, PedHash.Movspace01SMM, WeaponHash.UpNAtomizer, 500);
                    alien.CurrentPed.CanSufferCriticalHits = false;
                    npcList.Add(alien);
                    Logger.Log("Spawning astro for: " + cmd.username);
                    break;

                case "spawn_pirate":
                    Npc pirate = new Npc(cmd.username, PedHash.Stbla02AMY, WeaponHash.RPG, 1000);
                    pirate.CurrentPed.CanSufferCriticalHits = false;
                    pirate.CurrentPed.CanRagdoll = false;
                    pirate.CurrentPed.IsExplosionProof = true;
                    npcList.Add(pirate);
                    Logger.Log("Spawning hancock for: " + cmd.username);
                    break;

                case "spawn_gravity":
                    //effectTracker.SpawnGravityEffect();
                    Logger.Log("Spawning gravity effect");
                    break;

                default:
                    break;
            }
        }
    }
}