using GTA.Math;
using GTA;
using System;
using System.Collections.Generic;
using GTA.UI;
using System.Drawing;
using NpcHandler;

namespace SlavysMod
{
    public class VehicleNpc
    {
        private readonly string displayName;
        private readonly Vehicle currentVehicle;
        private readonly List<Npc> attackers;
        private DateTime? deathTime;

        public VehicleNpc(string name, VehicleHash vehicleHash)
        {
            this.displayName = name;
            this.attackers = CreateAttackers(4, PedHash.BallaSout01GMY);
            this.currentVehicle = SpawnVehicle(vehicleHash);
            SetAttackersIntoVehicle();
        }

        // Returns a list of Npc Attackers to place in the vehicle
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

            Ped character = Game.Player.Character;
            Vector3 spawnPosition = character.Position + new Vector3(0.0f, 0.0f, 5.0f) + character.ForwardVector * 10;
            float spawnHeading = Game.Player.Character.Heading + 90.0f;

            Vehicle vehicle = World.CreateVehicle(vehicleModel, spawnPosition, spawnHeading);

            if (vehicle == null)
            {
                Logger.Log("Failed to create vehicle: Vehicle object returned NULL");
                return null;
            }

            vehicleModel.MarkAsNoLongerNeeded();
            return vehicle;
        }

        // Sets the attackers into the vehicle of this class instance
        private void SetAttackersIntoVehicle()
        {
            if (attackers != null && currentVehicle != null)
            {
                int seat = -1;

                foreach (Npc attacker in attackers)
                {
                    if (seat < 3)
                    {
                        attacker.PutIntoVehicle(currentVehicle, (VehicleSeat)seat);
                        attacker.CurrentPed.Task.VehicleShootAtPed(Game.Player.Character);
                        seat++;
                    }
                }
            }
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

        // Sets the death time of the vehicle
        public void SetDeathTime()
        {
            if (currentVehicle.IsDead && deathTime == null)
                deathTime = DateTime.Now;
        }

        public DateTime? GetDeathTime()
        {
            return deathTime;
        }

        // Deletes the object from the world 
        public void Delete()
        {
            currentVehicle?.Delete();
            if (attackers != null)
            {
                foreach (Npc attacker in attackers)
                {
                    attacker.CurrentPed.Delete();
                }
            }
        }

        public bool GetIsDead()
        {
            return currentVehicle != null && currentVehicle.IsDead;
        }
    }
}
