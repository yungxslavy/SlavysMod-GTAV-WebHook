using System;
using System.Windows.Forms;
using System.Collections.Generic;
using GTA;
using GTA.Native;
using GTA.Math;
using NpcHandler;
using GTA.UI;
using System.Drawing;
using System.Threading.Tasks;

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
        private int deathCount = 0;
        private DateTime lastDeathTime = DateTime.Now;
        private TimeSpan timeAlive = TimeSpan.Zero;
        private TimeSpan bestTime = TimeSpan.Zero;

        public Main()
        {
            Tick += OnTick;
            KeyDown += OnKeyDown;
        }

        private void OnTick(object sender, EventArgs e)
        {
            StartServerIfNeeded();
            DisplayPlayerStats();
            CommandSystem();
            UpdateSystems();
        }

        private void StartServerIfNeeded()
        {
            if (!isServerRunning)
            {
                httpServer.Start();
                isServerRunning = true;
            }
        }

        private void DisplayPlayerStats()
        {
            Utils.DrawTextOnScreen(
                $"Health: {Game.Player.Character.Health}\n" +
                $"\nGravity Duration: {effectTracker.GravityEffect.GetRemainingDuration()}" +
                $"\nSpeedBoost Duration: {effectTracker.SpeedBoostEffect.GetRemainingDuration()}" +
                $"\n\nDeaths: {deathCount}" +
                $"\nTime Alive: {timeAlive:hh\\:mm\\:ss}" +
                $"\nLongest Alive: {bestTime:hh\\:mm\\:ss}",
                new PointF(10, 50),
                Alignment.Left
            );
        }

        private void UpdateSystems()
        {
            Utils.PlayerSystem(ref deathCount, ref timeAlive, ref bestTime, ref lastDeathTime);
            Utils.AttackerSystem(npcList, maxNpcLimit);
            Utils.VehicleAttackerSystem(vehicleNpcList, maxVehicleLimit);
            Utils.EffectSystem(effectTracker, npcList);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad3:
                    ResetEffects();
                    break;
                case Keys.NumPad1:
                    DeleteFirstNpc();
                    break;
            }
        }

        private void ResetEffects()
        {
            effectTracker.GravityEffect.Reset();
            effectTracker.SpeedBoostEffect.Reset();
            World.GravityLevel = 9.8f;
            Game.Player.Character.CancelRagdoll();
            Game.Player.SetRunSpeedMultThisFrame(1.0f);
        }

        private void DeleteFirstNpc()
        {
            if (npcList.Count > 0)
            {
                npcList[0].Delete();
                npcList.RemoveAt(0);
            }
            if (vehicleNpcList.Count > 0)
            {
                vehicleNpcList[0].Delete();
                vehicleNpcList.RemoveAt(0);
            }
        }

        private void CommandSystem()
        {
            Commands cmd = httpServer.GetCommand();
            if (cmd == null) return;

            switch (cmd.command)
            {
                case "spawn_meleeattacker":
                    SpawnRandomNpc(cmd, Utils.MeleePedHashList, Utils.MeleeWeaponHashList, 420);
                    break;
                case "spawn_armedattacker":
                    SpawnRandomNpc(cmd, Utils.ArmedPedHashList, Utils.FirearmWeaponHashList, 500);
                    break;
                case "spawn_gangvehicle":
                    SpawnVehicleNpc(cmd, VehicleHash.Baller);
                    break;
                case "spawn_carattack":
                    SpawnCarAttack(cmd);
                    break;
                case "spawn_speedboost":
                    AddEffect(cmd, EffectType.SpeedBoost, 10);
                    break;
                case "spawn_gravity":
                    AddEffect(cmd, EffectType.Gravity, 3);
                    break;
                case "spawn_astro":
                    SpawnAstro(cmd);
                    break;
                case "spawn_pirate":
                    SpawnPirate(cmd);
                    break;
                case "spawn_tank":
                    SpawnTank(cmd);
                    break;
                case "spawn_planecrash":
                    SpawnPlaneCrash(cmd);
                    break;
                case "spawn_juggernaut":
                    SpawnJuggernaut(cmd);
                    break;
            }
        }

        private void SpawnRandomNpc(Commands cmd, PedHash[] pedHashList, WeaponHash[] weaponHashList, int health, bool canSufferCriticalHits = true)
        {
            var npc = new Npc(cmd.username, Utils.GetRandomPedHash(pedHashList), Utils.GetRandomWeaponHash(weaponHashList), health);
            npc.CurrentPed.CanSufferCriticalHits = canSufferCriticalHits;
            npcList.Add(npc);
            Logger.Log($"Spawning attacker for: {cmd.username}");
        }

        private void SpawnVehicleNpc(Commands cmd, VehicleHash vehicleHash)
        {
            var vehicleNpc = new VehicleNpc(cmd.username, vehicleHash);
            vehicleNpcList.Add(vehicleNpc);
            Logger.Log($"Spawning gang vehicle for: {cmd.username}");
        }

        private void SpawnCarAttack(Commands cmd)
        {
            var caddy = new VehicleNpc(cmd.username, VehicleHash.Caddy);
            vehicleNpcList.Add(caddy);
            caddy.CurrentVehicle.Heading = Game.Player.Character.Heading + 180.0f;
            caddy.CurrentVehicle.Position = Game.Player.Character.Position + Game.Player.Character.ForwardVector * 10;
            caddy.CurrentVehicle.ForwardSpeed = 2000.0f;
            Logger.Log($"Spawning car attack for: {cmd.username}");
        }

        private void AddEffect(Commands cmd, EffectType effectType, int duration)
        {
            effectTracker.AddEffectDuration(effectType, duration);
            Logger.Log($"Spawning {effectType.ToString().ToLower()} effect for: {cmd.username}");
        }

        private void SpawnAstro(Commands cmd)
        {
            Npc astro = new Npc(cmd.username, PedHash.Movspace01SMM, WeaponHash.UpNAtomizer, 500);
            astro.CurrentPed.CanSufferCriticalHits = false;
            npcList.Add(astro);
            Logger.Log("Spawning astro for: " + cmd.username);
        }
        private void SpawnPirate(Commands cmd)
        {
            Npc pirate = new Npc(cmd.username, PedHash.Stbla02AMY, WeaponHash.RPG, 1000);
            pirate.CurrentPed.CanSufferCriticalHits = false;
            pirate.CurrentPed.CanRagdoll = false;
            pirate.CurrentPed.IsExplosionProof = true;
            npcList.Add(pirate);
            Logger.Log("Spawning pirate for: " + cmd.username);
        }

        private void SpawnJuggernaut(Commands cmd)
        {
            var juggernaut = new Npc(cmd.command, PedHash.Juggernaut01M, WeaponHash.Minigun, 5000)
            {
                CurrentPed = 
                {
                    CanSufferCriticalHits = false,
                    CanRagdoll = false,
                    IsExplosionProof = true,
                    FiringPattern = FiringPattern.FullAuto
                }
            };
            npcList.Add(juggernaut);
            Logger.Log($"Spawning juggernaut for: {cmd.username}");
        }

        private void SpawnTank(Commands cmd)
        {
            var tank = new VehicleNpc(cmd.username, VehicleHash.Rhino);
            vehicleNpcList.Add(tank);
            tank.Attackers[0].CurrentPed.Task.VehicleShootAtPed(Game.Player.Character);
            tank.Attackers[0].CurrentPed.FiringPattern = FiringPattern.FullAuto;
            Logger.Log($"Spawning tank for: {cmd.username}");
        }

        private void SpawnPlaneCrash(Commands cmd)
        {
            var plane = new VehicleNpc(cmd.username, VehicleHash.Jet);
            vehicleNpcList.Add(plane);
            var driver = plane.CurrentVehicle.GetPedOnSeat(VehicleSeat.Driver);
            driver?.Delete();
            Game.Player.Character.SetIntoVehicle(plane.CurrentVehicle, VehicleSeat.Driver);
            plane.CurrentVehicle.Position = new Vector3(90.0f, -1070.0f, 150.0f);
            plane.CurrentVehicle.Heading = 70.0f;
            plane.CurrentVehicle.ForwardSpeed = 2000;
            Logger.Log($"Spawning plane crash for: {cmd.username}");
        }
    }
}
