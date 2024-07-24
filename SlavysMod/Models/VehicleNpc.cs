using GTA.Math;
using GTA;
using System;
using System.Collections.Generic;
using GTA.UI;
using System.Drawing;
using NpcHandler;
using System.ComponentModel;

namespace SlavysMod
{
    public class VehicleNpc
    {
        private readonly string displayName;
        private readonly Vehicle currentVehicle;
        private readonly List<Npc> attackers;
        private DateTime? deathTime;

        public Vehicle CurrentVehicle => currentVehicle;
        public List<Npc> Attackers => attackers;

        public VehicleNpc(string name, VehicleHash vehicleHash)
        {
            this.displayName = name;
            this.currentVehicle = SpawnVehicle(vehicleHash);
            this.attackers = CreateAttackers(GetVehicleSeatCount(), PedHash.BallaSout01GMY);
            SetAttackersIntoVehicle();
        }

        // Spawns the attackers and returns the handle to them
        private List<Npc> CreateAttackers(int attackerCount, PedHash pedSkin)
        {
            List<Npc> tempAttackers = new List<Npc>();

            for (int i = 0; i < attackerCount; i++)
            {
                Npc attacker = new Npc("", pedSkin, WeaponHash.MicroSMG, 200);
                tempAttackers.Add(attacker);
            }

            return tempAttackers;
        }

        private Vehicle SpawnVehicle(VehicleHash vehicleHash)
        {
            Model vehicleModel = new Model(vehicleHash);
            vehicleModel.Request();

            DateTime timeout = DateTime.Now.AddSeconds(5);
            while (!vehicleModel.IsLoaded && DateTime.Now < timeout)
                Script.Wait(100);

            if (!vehicleModel.IsLoaded)
            {
                Logger.Log("Failed to load vehicle model.");
                return null;
            }

            // Create some distance from the player to safely spawn the vehicle
            Ped character = Game.Player.Character;
            Vector3 camVector = GameplayCamera.ForwardVector * 10;
            Vector3 spawnPosition = character.Position + new Vector3(camVector.X, camVector.Y, 5.0f);

            // Rotate the vehicle to be perpendicular to the player
            float spawnHeading = Game.Player.Character.Heading + 90.0f;
            
            Vehicle vehicle = World.CreateVehicle(vehicleModel, spawnPosition, spawnHeading);
            vehicle.IsEngineRunning = true;

            if (vehicle == null)
            {
                Logger.Log("Failed to create vehicle: Vehicle object returned NULL");
                return null;
            }

            vehicleModel.MarkAsNoLongerNeeded();
            return vehicle;
        }

        private void SetAttackersIntoVehicle()
        {
            if (attackers != null && currentVehicle != null)
            {
                int seat = -1; // driver seat starts at -1

                foreach (Npc attacker in attackers)
                {
                    // Check if the seat is free to avoid crashing the game
                    if (CurrentVehicle.IsSeatFree((VehicleSeat)seat))
                    {
                        attacker.PutIntoVehicle(currentVehicle, (VehicleSeat)seat);
                        attacker.CurrentPed.Task.VehicleShootAtPed(Game.Player.Character);
                        seat++;
                    }
                }
            }
        }

        private int GetVehicleSeatCount()
        {
            // -2 will never allow a seat to be taken
            if (currentVehicle == null)
                return -2;

            // Max seats ~ 12 (bus)
            int seatCount;
            for (seatCount = -1; seatCount < 11; seatCount++)
            {
                if (!currentVehicle.IsSeatFree((VehicleSeat)seatCount))
                    break;
            }

            return seatCount;
        }

        // Draws the name on the vehicle
        public void DrawName()
        {
            if (currentVehicle != null && !string.IsNullOrEmpty(displayName) && World.GetDistance(currentVehicle.Position, Game.Player.Character.Position) <= 30 && currentVehicle.IsOnScreen)
            {
                // Positions 
                Vector3 vehiclePos = currentVehicle.Position;
                Vector3 vehicleHead = vehiclePos + new Vector3(0, 0, 1);

                // Name tag
                PointF pointF = GTA.UI.Screen.WorldToScreen(vehiclePos, false);
                new TextElement(displayName, pointF, 0.4f, Color.White, GTA.UI.Font.Pricedown, Alignment.Center).Draw();

                // Health tag
                PointF healthPoint = GTA.UI.Screen.WorldToScreen(vehicleHead, false);
                new TextElement(currentVehicle.EngineHealth.ToString(), healthPoint, 0.4f, Color.Green, GTA.UI.Font.Pricedown, Alignment.Center).Draw();
            }
        }

        public void SetDeathTime()
        {
            // Death is when the vehicle explodes
            if (currentVehicle.IsDead && deathTime == null)
                deathTime = DateTime.Now;
        }

        public DateTime? GetDeathTime()
        {
            return deathTime;
        }

        // Deletes the attackers and the vehicle objects from the game
        public void Delete()
        {
            if (currentVehicle != null)
                currentVehicle?.Delete();

            if (attackers != null)
            {
                foreach (Npc attacker in attackers)
                {
                    attacker.Delete();
                }
            }
        }
        
        public bool GetIsDead()
        {
            return currentVehicle != null && currentVehicle.IsDead;
        }
    }
}
