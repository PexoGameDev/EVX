﻿using System.Collections.Generic;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Attacks
{
    public static class Damage
    {
        public const float WeakMultiplier = 0.25f;
        public const float StrongMultiplier = 2f;

        private static Dictionary<DamageType, (HealthType ResistantType, HealthType VulnerableType)> _damageTriangle = new Dictionary<DamageType, (HealthType ResistantType, HealthType VulnerableType)>()
        {
            {DamageType.Ballistic, (HealthType.Armor, HealthType.EnergyShields)},
            {DamageType.Laser, (HealthType.EnergyShields, HealthType.EMF)},
            {DamageType.Plasma, (HealthType.EMF, HealthType.Armor)},
        };

        public static int CalculateDamage(int damage, DamageType type, HealthType healthType, bool fluctuate = true)
        {
            float calculatedDamage = damage;
            //Adding a little random damage fluctuation
            if (fluctuate)
                calculatedDamage *= (1f + GameController.RandomGenerator.Next(-10,10)*0.01f);

            if (type == DamageType.Default) return (int)calculatedDamage;

            if (_damageTriangle[type].ResistantType == healthType) calculatedDamage *= WeakMultiplier;
            if (_damageTriangle[type].VulnerableType == healthType) calculatedDamage *= StrongMultiplier;

            return (int) calculatedDamage;
        }
    }
}