using GTA;
using GTA.Math;
using GTA.UI;
using NpcHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlavysMod
{
    public class Utils
    {
        // List of PedHashes that makes more sense being unarmed
        static public PedHash[] MeleePedHashList = new PedHash[] {
                PedHash.Business02AFY, PedHash.Business02AMY, PedHash.MimeSMY,
                PedHash.Fitness01AFY, PedHash.Gaffer01SMM, PedHash.Golfer01AFY
        };

        // List of PedHashes that makes more sense being armed
        static public PedHash[] ArmedPedHashList = new PedHash[] {
                PedHash.Cop01SFY, PedHash.Cop01SMY, PedHash.MexGang01GMY,
                PedHash.MexGoon01GMY, PedHash.MexGoon02GMY, PedHash.MexGoon03GMY,
                PedHash.ArmBoss01GMM, PedHash.ArmGoon01GMM
        };
        
        // List of Melee Weapon Hashes
        static public WeaponHash[] MeleeWeaponHashList = new WeaponHash[] {
                WeaponHash.Knife, WeaponHash.Nightstick, WeaponHash.Hammer,
                WeaponHash.Bat, WeaponHash.Crowbar, WeaponHash.GolfClub
        };

        // List of Firearm Weapon Hashes
        static public WeaponHash[] FirearmWeaponHashList = new WeaponHash[] {
                WeaponHash.Pistol, WeaponHash.CombatPistol, WeaponHash.Pistol50,
                WeaponHash.SNSPistol, WeaponHash.HeavyPistol, WeaponHash.VintagePistol
        };

        // Returns a random PedHash from a passed array of PedHashes
        static public PedHash GetRandomPedHash(PedHash[] optionsArray)
        {
            Random random = new Random();
            PedHash randomPedHash = optionsArray[random.Next(optionsArray.Length)];
            return randomPedHash;
        }

        // Returns a random WeaponHash from a passed array of WeaponHashes
        static public WeaponHash GetRandomWeaponHash(WeaponHash[] optionsArray)
        {
            Random random = new Random();
            WeaponHash randomWeaponHash = optionsArray[random.Next(optionsArray.Length)];
            return randomWeaponHash;
        }

        static public void DrawTextOnScreen(string text, PointF position, Alignment alignment)
        {
            TextElement textElement = new TextElement(text, position, 0.5f, Color.White, GTA.UI.Font.Pricedown, alignment);
            textElement.Outline = true;
            textElement.Shadow = true;
            textElement.Draw();
        }

        static public void AttackerSystem(List<Npc> npcList, int maxNpcLimit)
        {
            try
            {
                // Prevent NPC list from exceeding the limit
                if (npcList.Count > maxNpcLimit)
                {
                    npcList[0].CurrentPed.Delete(); // Deletes the actual ped from the game
                    npcList.RemoveAt(0);
                }

                // Loop each NPC in our list 
                foreach (Npc npc in npcList)
                {
                    // Draw the NPC's name 
                    npc.DrawName();

                    // NPC Death Checker 
                    if (npc.CurrentPed.IsDead)
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
                                npc.CurrentPed.Delete();
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

        // Same as AttackerSystem but for Vehicle NPCs (DRY principle was ignored here, sorry)
        static public void VehicleAttackerSystem(List<VehicleNpc> vehicleNpcList, int maxVehicleLimit)
        {
            try
            {
                // Prevent Vehicle NPC list from exceeding the limit
                if (vehicleNpcList.Count > maxVehicleLimit)
                {
                    vehicleNpcList[0].Delete(); // Deletes the actual Vehicle
                    vehicleNpcList.RemoveAt(0);
                }

                // Loop each Vehicle NPC in our list 
                foreach (VehicleNpc vehicleNpc in vehicleNpcList)
                {
                    // Draw the Vehicle NPC's name 
                    vehicleNpc.DrawName();

                    // Check if the vehicle is destroyed to clean up 
                    if (vehicleNpc.GetIsDead())
                    {
                        if (vehicleNpc.GetDeathTime() == null)
                        {
                            vehicleNpc.SetDeathTime();
                        }
                        else
                        {
                            // If dead longer than 5 seconds, delete the Vehicle NPC
                            if (DateTime.Now.Subtract(vehicleNpc.GetDeathTime().Value).TotalSeconds > 5)
                            {
                                vehicleNpc.Delete();
                                vehicleNpcList.Remove(vehicleNpc);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to draw vehicle attacker names - " + ex.Message);
            }
        }

        // EffectsSystem watches the effects and handles them based on their properties
        static public void EffectSystem(EffectTracker effectTracker)
        {
            // TODO: Finish this method
            try
            {
                // Handle each effect in the list
                foreach (Effect effect in effectTracker.effectList) {
                    if (effect.HasExpired())
                    {
                        Logger.Log("Effect expired: " + effect.Type);
                        effect.Reset();
                        switch (effect.Type)
                        {
                            case EffectType.Gravity:
                                World.GravityLevel = 9.8f;
                                break;
                            case EffectType.SpeedBoost:
                                Game.Player.SetRunSpeedMultThisFrame(1.0f);
                                break;
                        }
                    }
                    else
                    {
                        // Start effect if it is not active and has a duration
                        if (!effect.IsActive && effect.Duration > 0)
                        {
                            effect.Start();
                            //switch (effect.Type)
                            //{
                            //    case EffectType.Gravity:
                            //        World.GravityLevel = -5.0f;
                            //        break;
                            //    case EffectType.SpeedBoost:
                            //        Game.Player.SetRunSpeedMultThisFrame(8.0f);
                            //        break;
                            //}
                        }
                    }

                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to apply effects - " + ex.Message);
            }
        }
    }
}
