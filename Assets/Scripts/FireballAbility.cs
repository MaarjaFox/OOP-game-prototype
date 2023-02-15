using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Interfaces;
using BestGame.Effects;

public class FireballAbility : MonoBehaviour, IAreaAbilty //implements iAreaAbility interface
{

    private static FireballAbility instance;


    public static FireballAbility Instance
    {
        get  
        {
            if (instance == null) // If the instance variable is null, execute the following code block
            {
                instance = GameObject.FindObjectOfType<FireballAbility>(); 
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
        }
    }

    public int GetDamage(Troop troop)
    {
        return 20 * troop.Unit.CalculateAttack(troop.typeModifiers(Modifier.ModifierType.ATTACK));// Calculate the damage dealt by the troop
    }
    //DMG is 20.

}