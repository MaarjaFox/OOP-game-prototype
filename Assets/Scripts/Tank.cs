using BestGame.Interfaces;
using System;
using System.Diagnostics;
using UnityEngine;
namespace BestGame.Units

{
    public class Tank : BaseUnit
    {
        public override int CalculateBaseDamage(BaseUnit other)
        {

            return RandomGen.Next(minDamage, maxDamage);
        }

        public override int CalculateBaseDefense()
        {
            return baseDefense;
        }

        public override int CalculateBaseAttack()
        {
            return baseAttack;
        }
        public override int CalculateBaseHealth()
        {
            return baseHealth;
        }

        public override bool HasTankAbility() //from baseunit
        {
            return true;
        }
        
        public override IAreaAbilty TankAllAbility()
        {
            return TankAbility.Instance;
        }
    }
}