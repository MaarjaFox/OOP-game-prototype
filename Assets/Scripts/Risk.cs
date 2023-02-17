using System.Collections.Generic;
using UnityEngine;

public class Risk : MonoBehaviour
{
    public List<Troop> troops;

    public void DestroyRandomTroop()
    {
        if (troops.Count > 0)
        {
            int randomIndex = Random.Range(0, troops.Count);
            Troop randomTroop = troops[randomIndex];

            if (randomTroop != null)
            {
                Destroy(randomTroop.gameObject);
                troops.RemoveAt(randomIndex);
                MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", randomTroop.TroopId);
            }
        }
    }

}
