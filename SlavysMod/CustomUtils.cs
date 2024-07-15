using System;
using System.Windows.Forms;
using System.Drawing;
using GTA;
using GTA.Native;
using GTA.Math;
using GTA.UI;
using System.Xml.Linq;

namespace CustomUtil
{
    public static class Util
    {
        public static Ped SpawnAttackingPedestrian(String name, Model pedModel, WeaponHash wpHash)
        {
            // Get the player's character
            Ped player = Game.Player.Character;

            // Get the player's position and add some height
            Vector3 spawnPosition = player.Position + new Vector3(0, 0, 3);

            // Define the pedestrian model (change the hash to spawn different models)
            Model pedestrianModel = new Model(PedHash.Fireman01SMY);

            // Request the model and wait until it's loaded
            pedestrianModel.Request(500);

            if (pedestrianModel.IsInCdImage && pedestrianModel.IsValid)
            {
                while (!pedestrianModel.IsLoaded)
                    Script.Wait(100);

                // Create the pedestrian at the spawn position
                Ped newPed = World.CreatePed(pedestrianModel, spawnPosition);
                newPed.Task.FightAgainst(player);

                // Ensure the pedestrian model is no longer needed
                pedestrianModel.MarkAsNoLongerNeeded();

                return newPed;
            }
            else return null;
        }

        public static void DrawName(Ped ped, String name)
        {
            
        }
    }
}
