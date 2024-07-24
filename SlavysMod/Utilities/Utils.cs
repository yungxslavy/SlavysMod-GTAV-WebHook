using GTA;
using GTA.Math;
using GTA.UI;
using NpcHandler;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SlavysMod
{
    public static class Utils
    {
        // List of PedHashes that are typically unarmed
        public static PedHash[] MeleePedHashList = {
            PedHash.Business02AFY, PedHash.Business02AMY, PedHash.MimeSMY,
            PedHash.Fitness01AFY, PedHash.Gaffer01SMM, PedHash.Golfer01AFY
        };

        // List of PedHashes that are typically armed
        public static PedHash[] ArmedPedHashList = {
            PedHash.Cop01SFY, PedHash.Cop01SMY, PedHash.MexGang01GMY,
            PedHash.MexGoon01GMY, PedHash.MexGoon02GMY, PedHash.MexGoon03GMY,
            PedHash.ArmBoss01GMM, PedHash.ArmGoon01GMM
        };

        // List of Melee Weapon Hashes
        public static WeaponHash[] MeleeWeaponHashList = {
            WeaponHash.Knife, WeaponHash.Nightstick, WeaponHash.Hammer,
            WeaponHash.Bat, WeaponHash.Crowbar, WeaponHash.GolfClub
        };

        // List of Firearm Weapon Hashes
        public static WeaponHash[] FirearmWeaponHashList = {
            WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.Pistol50,
            WeaponHash.SNSPistol, WeaponHash.HeavyPistol, WeaponHash.VintagePistol
        };

        // Returns a random PedHash from a given array of PedHashes
        public static PedHash GetRandomPedHash(PedHash[] optionsArray)
        {
            Random random = new Random();
            return optionsArray[random.Next(optionsArray.Length)];
        }

        // Returns a random WeaponHash from a given array of WeaponHashes
        public static WeaponHash GetRandomWeaponHash(WeaponHash[] optionsArray)
        {
            Random random = new Random();
            return optionsArray[random.Next(optionsArray.Length)];
        }

        // Method to write text to screen
        public static void DrawTextOnScreen(string text, PointF position, Alignment alignment)
        {
            var textElement = new TextElement(text, position, 0.5f, Color.White, GTA.UI.Font.Pricedown, alignment)
            {
                Outline = true,
                Shadow = true
            };
            textElement.Draw();
        }

        // Handles player stats and respawn mechanics
        public static void PlayerSystem(ref int deathCount, ref TimeSpan timeAlive, ref TimeSpan bestTime, ref DateTime lastDeathTime)
        {
            timeAlive = DateTime.Now - lastDeathTime;

            if (Game.Player.Character.Health <= 100)
            {
                RespawnPlayer(ref deathCount, ref bestTime, ref lastDeathTime, timeAlive);
            }
        }

        private static void RespawnPlayer(ref int deathCount, ref TimeSpan bestTime, ref DateTime lastDeathTime, TimeSpan timeAlive)
        {
            Ped character = Game.Player.Character;
            character.MaxHealth = 10000;
            character.Health = 10000;
            character.Weapons.Give(WeaponHash.Parachute, 1, true, true);

            // Let the death animation play out in a vehicle
            if (!character.IsInVehicle())
            {
                character.Position += new Vector3(0, 0, 200);
                character.CancelRagdoll();
            }

            // Handle player stats
            deathCount++;
            lastDeathTime = DateTime.Now;
            if (timeAlive > bestTime)
                bestTime = timeAlive;
        }

        // Manages NPC attackers
        public static void AttackerSystem(List<Npc> npcList, int maxNpcLimit)
        {
            try
            {
                ManageNpcList(npcList, maxNpcLimit);
                foreach (var npc in npcList)
                {
                    npc.DrawName();
                    HandleNpcDeath(npc, npcList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to draw attacker names - {ex.Message}");
            }
        }

        // Manages vehicle NPC attackers
        public static void VehicleAttackerSystem(List<VehicleNpc> vehicleNpcList, int maxVehicleLimit)
        {
            try
            {
                ManageVehicleNpcList(vehicleNpcList, maxVehicleLimit);
                foreach (var vehicleNpc in vehicleNpcList)
                {
                    vehicleNpc.DrawName();
                    HandleVehicleNpcDeath(vehicleNpc, vehicleNpcList);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to draw vehicle attacker names - {ex.Message}");
            }
        }

        // Watches and handles effects based on their properties
        public static void EffectSystem(EffectTracker effectTracker, List<Npc> npcList)
        {
            try
            {
                foreach (var effect in effectTracker.effectList)
                {
                    if (effect.HasExpired())
                    {
                        HandleExpiredEffect(effect, npcList);
                    }
                    else if (!effect.IsActive && effect.Duration > 0)
                    {
                        StartEffect(effect, npcList);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to apply effects - {ex.Message}");
            }
        }

        private static void ManageNpcList(List<Npc> npcList, int maxNpcLimit)
        {
            if (npcList.Count > maxNpcLimit)
            {
                npcList[0].Delete();
                npcList.RemoveAt(0);
            }
        }

        private static void HandleNpcDeath(Npc npc, List<Npc> npcList)
        {
            if (npc.CurrentPed.IsDead)
            {
                if (npc.GetDeathTime() == null)
                {
                    npc.SetDeathTime();
                }
                else if (DateTime.Now.Subtract(npc.GetDeathTime().Value).TotalSeconds > 5)
                {
                    npc.Delete();
                    npcList.Remove(npc);
                }
            }
        }

        private static void ManageVehicleNpcList(List<VehicleNpc> vehicleNpcList, int maxVehicleLimit)
        {
            if (vehicleNpcList.Count > maxVehicleLimit)
            {
                vehicleNpcList[0].Delete();
                vehicleNpcList.RemoveAt(0);
            }
        }

        private static void HandleVehicleNpcDeath(VehicleNpc vehicleNpc, List<VehicleNpc> vehicleNpcList)
        {
            if (vehicleNpc.GetIsDead())
            {
                if (vehicleNpc.GetDeathTime() == null)
                {
                    vehicleNpc.SetDeathTime();
                }
                else if (DateTime.Now.Subtract(vehicleNpc.GetDeathTime().Value).TotalSeconds > 5)
                {
                    vehicleNpc.Delete();
                    vehicleNpcList.Remove(vehicleNpc);
                }
            }
        }

        private static void HandleExpiredEffect(Effect effect, List<Npc> npcList)
        {
            Logger.Log($"Effect expired: {effect.Type}");
            effect.Reset();
            switch (effect.Type)
            {
                case EffectType.Gravity:
                    World.GravityLevel = 9.8f;
                    Game.Player.Character.CancelRagdoll();
                    break;
                case EffectType.SpeedBoost:
                    Game.Player.SetRunSpeedMultThisFrame(1.0f);
                    break;
            }
        }

        private static void StartEffect(Effect effect, List<Npc> npcList)
        {
            effect.Start();
            switch (effect.Type)
            {
                case EffectType.Gravity:
                    World.GravityLevel = -4.0f;
                    Game.Player.Character.Ragdoll();
                    foreach (var npc in npcList)
                    {
                        npc.CurrentPed.ApplyForce(new Vector3(0, 0, 10));
                    }
                    break;
                case EffectType.SpeedBoost:
                    Game.Player.SetRunSpeedMultThisFrame(5.0f);
                    break;
            }
        }
    }
}
