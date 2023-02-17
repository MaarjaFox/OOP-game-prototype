using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Interfaces;
using BestGame.Effects;

public class PoisonAbility : MonoBehaviour, IAreaAbilty //implements iAreaAbility interface
{

    private static PoisonAbility instance;

    
    public static PoisonAbility Instance
    {
        get
        {
            if (instance == null) // If the instance variable is null, execute the following code block
            {
                instance = GameObject.FindObjectOfType<PoisonAbility>();
            }
            return instance;
        }
    }

    public List<int> AbilityRange(Hex epicenter) // Define a public method that takes a Hex object as an argument and returns a list of integer IDs of its neighbors
    {
        List<GameObject> _neighbors = epicenter.Neighbors;
        List<int> neighborIds = new List<int>();
        foreach (GameObject neighbor in _neighbors)
        {
            neighborIds.Add(neighbor.GetComponent<Hex>().Id);
        }
        neighborIds.Add(epicenter.Id);
        return neighborIds;


    }

    public void Apply(Troop caster, List<Troop> targets) // Define a public method that takes a Troop object as the caster and a list of Troop objects as targets
    {
        foreach (Troop troop in targets)
        {
            int dmg = troop.ApplyDealtDamage(GetDamage(caster), caster.Unit);
            UIManager.Instance.AddLogText(caster.UnitName() + " does " + dmg.ToString() + " to " + troop.UnitName());


            if (troop.GetTotalRemainingHealth() < 1) // If the current Troop object's remaining health is less than 1, destroy the current Troop and display message
            {
                //hex.occupyingTroop = null;

                Destroy(troop.gameObject);
                MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", troop.TroopId);
            }

            //if (!troop.HasEffect<Modifier>()) // Only apply the poison effect if the target is not already poisoned
           // {
           //     Modifier poisonEffect = new Modifier(2, 10, ModifierType.POISON); // Create a new damage over time effect that lasts for two rounds and deals 10 damage per round with poison damage type
           //     troop.ApplyEffect(poisonEffect); // Apply the poison effect to the target
           //     UIManager.Instance.AddLogText(caster.UnitName() + " applies poison to " + troop.UnitName());
           // }
        }
    }
   
    public int GetDamage(Troop troop)
    {
        return 10 * troop.Unit.CalculateAttack(troop.typeModifiers(Modifier.ModifierType.ATTACK));// Calculate the damage dealt by the troop
    }
    //DMG is 10.

}
