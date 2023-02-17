using UnityEngine;
using System;
using System.Collections.Generic;
using BestGame.Effects;
using BestGame.Interfaces;

namespace BestGame.Units

{
    public abstract class BaseUnit : MonoBehaviour
    {
        [SerializeField]
        protected int baseInitiative = 1;
        [SerializeField]
        protected int minDamage = 1;
        [SerializeField]
        protected int maxDamage = 1;
        [SerializeField]
        protected int baseAttack = 1;
        [SerializeField]
        protected int baseDefense = 1;
        [SerializeField]
        protected int baseHealth = 1;

        protected bool canRetaliate = true;

        protected System.Random random;

        public System.Random RandomGen
        {
            get
            {
                if (random == null)
                {
                    random = new System.Random();
                }

                return random;
            }
        }

        public virtual bool CanRetaliate()
        {
            return canRetaliate;
        }
        public virtual bool HasMageAbility()
        {
            return false;
        }
        public virtual bool HasHealingAbility()
        {
            return false;
        }
        public virtual IAbilty MageAbility()
        {
            return null;
        }
        public virtual bool HasRangedAbility()
        {
            return false;
        }
        public virtual bool HasPoisonAbility()
        {
            return false;
        }
        public virtual bool HasTankAbility()
        {
            return false;
        }
        public virtual IAreaAbilty TankAllAbility()
        {
            return null;
        }
        public virtual IAreaAbilty PoisonRangedAbility()
        {
            return null;
        }
        public virtual IAbilty RangedAbility()
        {
            return null;
        }

        public virtual bool HasAreaAbility()
        {
            return false;
        }

        public virtual IAreaAbilty AreaAbility()
        {
            return null;
        }

        public void Start()
        {
            random = new System.Random();
        }

        public abstract int CalculateBaseDamage(BaseUnit other);
        public abstract int CalculateBaseDefense();
        public abstract int CalculateBaseAttack();

        public abstract int CalculateBaseHealth();
        public int CalculateDefense(List<Modifier> modifiers)
        {
            return ApplyModifiers(CalculateBaseDefense(), modifiers);
        }

        public int CalculateHealth(List<Modifier> modifiers)
        {
            return ApplyModifiers(CalculateBaseHealth(), modifiers);
        }
        public int CalculateAttack(List<Modifier> modifiers)
        {
            return ApplyModifiers(CalculateBaseAttack(), modifiers);
        }

        public virtual int CalculateDealtDamage(BaseUnit other,
                                                List<Modifier> attackModifiers,
                                                List<Modifier> defenseModifiers,
                                                List<Modifier> dealingDamageModifiers)
        {

            int atk = CalculateAttack(attackModifiers);
            int def = other.CalculateDefense(defenseModifiers);
            int damage = Math.Max(CalculateBaseDamage(other) - (def - atk), 1);
            damage = ApplyModifiers(damage, dealingDamageModifiers);
            return damage;
        }

        public virtual int AppliedDamage(int dealtDamage, BaseUnit damageDealer, List<Modifier> modifiers)
        {
            return ApplyModifiers(dealtDamage, modifiers);
        }

        public int Initiative
        {
            get { return baseInitiative; }
        }


        public virtual void PerformPreAttack(Troop troop)
        {

        }

        public virtual void PerformPostAttack(Troop troop)
        {

        }
        public virtual void PerformPreRetaliate(Troop parentTroop, Troop enemyTroop)
        {
            parentTroop.HasRetaliated();
        }
        public virtual void PerformPostRetaliate(Troop parentTroop, Troop enemyTroop)
        {

        }


        private int ApplyModifiers(int baseValue, List<Modifier> modifiers)
        {
            int total = baseValue;

            foreach (var mod in modifiers)
            {
                if (mod.IsApplicable(this))
                {
                    total = mod.Apply(total);
                }
            }

            return total;
        }

    }


}