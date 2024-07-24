using GTA;
using System;
using GTA.UI;
using System.Drawing;
using GTA.Math;

namespace NpcHandler
{
    public class Npc
    {
        private readonly Ped currentPed;
        private readonly string displayName;
        private DateTime? deathTime;

        public Ped CurrentPed => currentPed;

        // Constructor spawns a pedestrian with a weapon and health
        public Npc(string name, PedHash pedHash, WeaponHash wpHash, int health)
        {
            this.displayName = name;
            this.currentPed = SpawnAttackingPedestrian(pedHash, wpHash, health);
        }

        // Spawns a pedestrian on top of the player and targets the player
        private Ped SpawnAttackingPedestrian(PedHash pedHash, WeaponHash wpHash, int health)
        {
            try
            {
                Ped character = Game.Player.Character;
                Vector3 camVector = GameplayCamera.ForwardVector * 3;
                Vector3 spawnPosition = character.Position + new Vector3(camVector.X, camVector.Y, 3);
                Model pedestrianModel = new Model(pedHash);

                pedestrianModel.Request(500);
                if (pedestrianModel.IsInCdImage && pedestrianModel.IsValid)
                {
                    DateTime timeout = DateTime.Now.AddSeconds(5);
                    while (!pedestrianModel.IsLoaded && DateTime.Now < timeout)
                        Script.Wait(100);

                    if (!pedestrianModel.IsLoaded)
                        throw new Exception("Model could not be loaded in time.");

                    Ped newPed = World.CreatePed(pedestrianModel, spawnPosition);
                    newPed.Task.FightAgainst(character); // Task needed to target player 
                    newPed.Weapons.Give(wpHash, 1000, true, true);
                    newPed.Health = health;
                    pedestrianModel.MarkAsNoLongerNeeded();

                    return newPed;
                }
            }
            catch (Exception ex)
            {
                Notification.Show($"Error spawning pedestrian: {ex.Message}");
            }
            return null;
        }

        // Draws the NPC's name on their body and health above their head (note: call per frame)
        public void DrawName()
        {
            if (currentPed != null && displayName != null && World.GetDistance(currentPed.Position, Game.Player.Character.Position) <= 30 && currentPed.IsOnScreen)
            {
                Vector3 entityPos = currentPed.Position;
                Vector3 entityHead = entityPos + new Vector3(0, 0, 1);

                PointF pointF = GTA.UI.Screen.WorldToScreen(entityPos, false);
                new TextElement(displayName, pointF, 0.4f, Color.White, GTA.UI.Font.Pricedown, Alignment.Center).Draw();

                PointF healthPoint = GTA.UI.Screen.WorldToScreen(entityHead, false);
                new TextElement(currentPed.Health.ToString(), healthPoint, 0.4f, Color.Green, GTA.UI.Font.Pricedown, Alignment.Center).Draw();
            }
        }

        public void SetDeathTime()
        {
            if (currentPed.IsDead && deathTime == null)
                deathTime = DateTime.Now;
        }

        public DateTime? GetDeathTime()
        {
            return deathTime;
        }

        public void PutIntoVehicle(Vehicle vehicle, VehicleSeat seat)
        {
            currentPed?.SetIntoVehicle(vehicle, seat);
        }

        public void Delete()
        {
            if (currentPed != null)
                currentPed?.Delete();
        }
    }
}
