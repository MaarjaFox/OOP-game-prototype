using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Interfaces;

public class RangedAttackAbility : MonoBehaviour, IAbilty
{
    private static RangedAttackAbility instance;


    public static RangedAttackAbility Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<RangedAttackAbility>();
            }
            return instance;
        }
    }


    public void Apply(Troop dealer, Troop other)
    {
        other.HasRetaliated();
        dealer.PerformAttack(other);

        if (other.GetTotalRemainingHealth() < 1)
        {
            //hex.occupyingTroop = null;

            Destroy(other.gameObject);
            MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", other.TroopId);
        }

    }

    public bool CanBeApplied(Troop dealer, Troop other)
    {
        if (dealer._army.BusSubscriberId() == other._army.BusSubscriberId())
        {
            return false;
        }
        return true;
    }
}