using BestGame.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BestGame.Interfaces
{
    public interface IAbilty
    {
        // Start is called before the first frame update
        public void Apply(Troop dealer, Troop other);
        public bool CanBeApplied(Troop dealer, Troop other);
    }

    public interface IAreaAbilty
    {
        // Start is called before the first frame update
        public List<int> AbilityRange(Hex epicenter);
        public void Apply(Troop caster, List<Troop> targets);
        public int GetDamage(Troop troop);
    }
}