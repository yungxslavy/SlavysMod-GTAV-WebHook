using GTA;
using System;
using System.Collections.Generic;
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
    }
}
