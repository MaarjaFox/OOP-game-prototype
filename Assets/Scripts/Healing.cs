using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healing : MonoBehaviour
{
    public GameObject[] playerUnitPrefabs;
    public int healthToGive = 10;
    public Button healingButton;
    private int roundCount = 0;

    public int GetCurrentTurnCount()
    {
        return roundCount;
    }
    private void Start()
    {
        healingButton.onClick.AddListener(ApplyHealing);
        healingButton.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isTroopsTurn() && _areTroopsActive())
        {
            healingButton.gameObject.SetActive(true);
        }
        else
        {
            healingButton.gameObject.SetActive(false);
        }
    }

    private void ApplyHealing()
    {
        for (int i = 0; i < playerUnitPrefabs.Length; i++)
        {
            Troop troop = playerUnitPrefabs[i].GetComponent<Troop>();
            if (troop != null)
            {
                int newHealth = troop.HealthLeft + healthToGive;
                troop.HealthLeft = newHealth;

                Debug.Log("Unit " + i + " now has " + newHealth + " health.");
            }
        }
        roundCount++;
    }

    private bool _isTroopsTurn()
    {
        // Check if it's the troop's turn by comparing the current turn counter
        // with the number of turns between each troop turn
        int turnsBetweenTroopTurns = 3;
        int currentTurn = GetCurrentTurnCount();
        return currentTurn % turnsBetweenTroopTurns == 0;
    }

    private bool _areTroopsActive()
    {
        // Check if any of the troops is active
        for (int i = 0; i < playerUnitPrefabs.Length; i++)
        {
            Troop troop = playerUnitPrefabs[i].GetComponent<Troop>();
            if (troop != null && troop.gameObject.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
    }
}