﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlavysMod
{
    // Enum for the different types of effects
    public enum EffectType
    {
        Gravity,
        SpeedBoost
    }

    public struct Effect
    {
        public bool IsActive { get; set; }
        public int Duration { get; set; }
        public DateTime? StartTime { get; set; }
        public EffectType Type { get; set; }

        public void Reset()
        {
            IsActive = false;
            Duration = 0;
            StartTime = null;
        }

        public void Start()
        {
            IsActive = true;
            StartTime = DateTime.Now;
        }

        public int GetRemainingDuration()
        {
            if (!StartTime.HasValue)
                return Duration;

            return Duration - (DateTime.Now - StartTime.Value).Seconds;
        }

        // Method to check if the effect has expired
        public bool HasExpired()
        {
            return IsActive && StartTime.HasValue && DateTime.Now > StartTime.Value + TimeSpan.FromSeconds(Duration);
        }
    }

    public class EffectTracker
    {
        public Effect gravityEffect;
        private Effect speedBoostEffect;
        public readonly List<Effect> effectList;

        public Effect GravityEffect => gravityEffect;
        public Effect SpeedBoostEffect => speedBoostEffect;

        public EffectTracker()
        {
            gravityEffect = new Effect { IsActive = false, Duration = 0, StartTime = null, Type = EffectType.Gravity };
            speedBoostEffect = new Effect { IsActive = false, Duration = 0, StartTime = null, Type = EffectType.SpeedBoost };
            effectList = new List<Effect> { gravityEffect, speedBoostEffect };
        }

        public void AddEffectDuration(EffectType effectType, int duration)
        {
            switch (effectType)
            {
                case EffectType.Gravity:
                    gravityEffect.Duration += duration;
                    break;
                case EffectType.SpeedBoost:
                    speedBoostEffect.Duration += duration;
                    break;
            }
        }
    }
}