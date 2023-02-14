using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Interfaces;
using BestGame.Effects;

public class FireballAbility : MonoBehaviour, IAreaAbilty
{

    private static FireballAbility instance;


    public static FireballAbility Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<FireballAbility>();
            }
            return instance;
        }
    }

    public List<int> AbilityRange(Hex epicenter)
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

    public void Apply(Troop caster, List<Troop> targets)
    {
        foreach (Troop troop in targets)
        {
            int dmg = troop.ApplyDealtDamage(GetDamage(caster), caster.Unit);
            UIManager.Instance.AddLogText(caster.UnitName() + " does " + dmg.ToString() + " to " + troop.UnitName());


            if (troop.GetTotalRemainingHealth() < 1)
            {
                //hex.occupyingTroop = null;

                Destroy(troop.gameObject);
                MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", troop.TroopId);
            }
        }
    }

    public int GetDamage(Troop troop)
    {
        return 20 * troop.Unit.CalculateAttack(troop.typeModifiers(Modifier.ModifierType.ATTACK));
    }


}