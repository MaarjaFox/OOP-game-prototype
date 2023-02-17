using System.Collections.Generic;
using UnityEngine;

public class Risk : MonoBehaviour
{
    public static Risk Instance { get; private set; } // Define a static instance property

    public List<Troop> troops;

    private bool _riskUsed = false; // Track if the risk ability has been used in this turn

    private void Awake()
    {
        Instance = this; // Set the static instance property
    }

    public void DestroyRandomTroop()
    {
        if (!_riskUsed && troops.Count > 0) // Only allow the ability to be used once per turn
        {
            int randomIndex = Random.Range(0, troops.Count);
            Troop randomTroop = troops[randomIndex];

            



            if (randomTroop != null)
            {
                Destroy(randomTroop.gameObject);
                troops.RemoveAt(randomIndex);
                MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", randomTroop.TroopId);
                _riskUsed = true; // Mark the risk ability as used
                UIManager.Instance.HideRiskButton(); // Hide the risk button after the ability has been used
            }
        }
    }

    public void ResetRiskAbility() // Call this method at the start of each turn to reset the _riskUsed field
    {
        _riskUsed = false;
        UIManager.Instance.ShowRiskButton(); // Show the risk button at the start of each turn
    }
}
