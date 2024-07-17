using GTA;
using System;
using GTA.UI;
using System.Drawing;
using GTA.Math;

namespace NpcHandler
{
    public class Npc
    {
        private readonly Ped Entity = null;
        private readonly string DisplayName = "null";
        private DateTime? DeathTime = null;


        public Npc(string name, PedHash pedHash, int health, WeaponHash wpHash)
        {
            this.DisplayName = name;
            this.Entity = SpawnAttackingPedestrian(pedHash, health, wpHash);
        }
           
        // Function spawns pedestrian on top of player and targets player 
        private Ped SpawnAttackingPedestrian(PedHash pedHash, int health, WeaponHash wpHash)
        {
            // Get the player's character
            Ped player = Game.Player.Character;

            // Get the player's position and add some height
            Vector3 spawnPosition = player.Position + new Vector3(0, 0, 3);

            // Define the pedestrian model (change the hash to spawn different models)
            Model pedestrianModel = new Model(pedHash);

            // Request the model and wait until it's loaded
            pedestrianModel.Request(500);

            if (pedestrianModel.IsInCdImage && pedestrianModel.IsValid)
            {
                while (!pedestrianModel.IsLoaded)
                    Script.Wait(100);

                // Create the pedestrian at the spawn position
                Ped newPed = World.CreatePed(pedestrianModel, spawnPosition);
                newPed.Task.FightAgainst(player); // Targets the player
                newPed.Weapons.Give(wpHash, 1000, true, true); // Gives the pedestrian a weapon
                newPed.Health = health; // Sets the pedestrian's health

                // Ensure the pedestrian model is no longer needed
                pedestrianModel.MarkAsNoLongerNeeded();

                return newPed;
            }
            else return null;
        }

        public void DrawName()
        {
            if (Entity != null && DisplayName != null && World.GetDistance(Entity.Position, Game.Player.Character.Position) <= 30 && Entity.IsOnScreen)
            {
                // Positions 
                Vector3 entityPos = Entity.Position;
                Vector3 entityHead = entityPos + new Vector3(0, 0, 1);
                   
                // Name tag
                PointF pointF = GTA.UI.Screen.WorldToScreen(entityPos, false);
                new TextElement(DisplayName, pointF, (float)0.6, Color.White, GTA.UI.Font.Pricedown, Alignment.Center).Draw();
                
                // Health tag
                PointF healthPoint = GTA.UI.Screen.WorldToScreen(entityHead, false);
                new TextElement(Entity.Health.ToString(), healthPoint, (float)0.6, Color.Green, GTA.UI.Font.Pricedown, Alignment.Center).Draw();
            }
        }

        public void SetDeathTime()
        {
            if (Entity.IsDead && DeathTime == null)
                DeathTime = DateTime.Now;
            
        }

        public DateTime? GetDeathTime()
        {
            return DeathTime;
        }

        // Deletes the object from the world 
        public void Delete()
        {
            Entity?.Delete();
        }

        public bool GetIsDead()
        {
            return Entity.IsDead;
        }
    }
}
