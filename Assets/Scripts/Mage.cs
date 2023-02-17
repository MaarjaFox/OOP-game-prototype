using BestGame.Interfaces;
using System;
using System.Diagnostics;
using UnityEngine;
namespace BestGame.Units

{
    public class Mage : BaseUnit
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

        public override bool HasMageAbility()
        {
            return true;
        }
        public override bool HasHealingAbility()
        {
            return true;
        }

        public override IAbilty MageAbility()
        {
            UnityEngine.Debug.Log("Returning " + MageAttackAbility.Instance.ToString());
            return MageAttackAbility.Instance;
        }

    }
}