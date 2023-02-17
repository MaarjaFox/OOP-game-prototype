using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    public static Heal Instance { get; private set; } // Define a static instance property

    public List<Troop> troops;

    public int healAmount = 20;

    private bool _healUsed = false; // Track if the heal ability has been used in this turn

    private void Awake()
    {
        Instance = this; // Set the static instance property
    }

    public void HealAllTroops()
    {
        if (!_healUsed && troops.Count > 0) // Only allow the ability to be used once per turn
        {
            foreach (Troop troop in troops)
            {
                if (troop != null)
                {
                    troop.RestoreHealth(20); // Restore 20 health to each troop in the list
                    Debug.Log($"Troop {troop.name} has been healed");
                    MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_HEALED, "", troop.TroopId);
                }
            }
            _healUsed = true; // Mark the heal ability as used
            UIManager.Instance.HideHealButton(); // Hide the heal button after the ability has been used
        }
    }

    public void ResetHealAbility() // Call this method at the start of each turn to reset the _healUsed field
    {
        _healUsed = false;
        UIManager.Instance.ShowHealButton(); // Show the heal button at the start of each turn
    }
}