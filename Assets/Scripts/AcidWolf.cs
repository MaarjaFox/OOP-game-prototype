using System;

namespace BestGame.Units

{
    public class AcidWolf : BaseUnit
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

        public void PreformPreAttack(Troop troop)
        {

        }


        public void PerformPostRetaliate(Troop parentTroop, Troop enemyTroop)
        {
            parentTroop.ResetRetaliation();
        }
    }
}