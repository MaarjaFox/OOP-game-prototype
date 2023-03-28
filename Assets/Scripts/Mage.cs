using System.Collections;
using System.Collections.Generic;
using BestGame.Interfaces;
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
        public override bool CanRetaliate()
        {
            return false;
        }
        public override bool HasMageAbility()
        {
            return true;
        }

        public override IAbilty MageAAbility()
        {
            UnityEngine.Debug.Log("Returning " + MageAbility.Instance.ToString());
            return MageAbility.Instance;
        }

    }
}