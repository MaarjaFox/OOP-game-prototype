using BestGame.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BestGame.Units {
    public class WereWolf : BaseUnit
    {
        public override int CalculateBaseAttack()
        {
            return baseAttack;
        }

        public override int CalculateBaseDamage(BaseUnit other)
        {
           return RandomGen.Next(minDamage, maxDamage);
        }

        public override int CalculateBaseDefense()
        {
            return baseDefense;
        }

        public override int CalculateBaseHealth()
        {
            return baseHealth;
        }

        public override bool HasAreaAbility()
        {
            return true;
        }

        public override IAreaAbilty AreaAbility()
        {
            return FireballAbility.Instance;
        }

        // Start is called before the first frame update

    }
}


