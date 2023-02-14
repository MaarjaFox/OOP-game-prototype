using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using BestGame.Interfaces;

public class AiBehavior : MonoBehaviour
{
    private static AiBehavior instance;


    public static AiBehavior Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AiBehavior>();
            }
            return instance;
        }
    }

    public Troop GetStrongestOpponent(ArmyManager army)
    {

        List<Tuple<int, Troop>> opponents = new List<Tuple<int, Troop>>();
        foreach (Troop opposingTroop in army.GetComponentsInChildren<Troop>())
        {
            opponents.Add(Tuple.Create(opposingTroop.GetTotalRemainingHealth(), opposingTroop));
        }

        opponents.Sort(delegate (Tuple<int, Troop> first, Tuple<int, Troop> second) {
            return first.Item1 - second.Item1;
        });


        Troop target = opponents.ToArray()[0].Item2;
        return target;

    }

    public ArmyManager GetOpposingArmy(Troop troop, BattleTurnManager manager)
    {
        ArmyManager opposingArmy = null;
        foreach (ArmyManager army in manager._armies)
        {
            if (army.ArmyId != troop._army.ArmyId)
            {
                opposingArmy = army;
            }
        }
        return opposingArmy;
    }


    public void AttackStrongestOpponent(Troop troop, BattleTurnManager battleTurnManager, MapManager mapManager)
    {

        List<Tuple<int, AStarSearch, Troop>> candidates = new List<Tuple<int, AStarSearch, Troop>>();
        //Iterate through all armies
        Troop target = GetStrongestOpponent(GetOpposingArmy(troop, battleTurnManager));

        foreach (var neighbor in mapManager.getHexById(target.HexId).GetComponent<Hex>().FreeNeighbors)
        {

            //Calculate A*
            var aStar = new AStarSearch(
                mapManager.getHexById(troop.HexId).GetComponent<Hex>(),
                neighbor

               );
            //Add to candidates
            candidates.Add(Tuple.Create(neighbor.Id, aStar, target));
            Debug.Log(" Dist from " + neighbor.Id + " " + aStar.costSoFar[neighbor.Id]);
        }
        //Find nearest hex to troop

        candidates.Sort(
            delegate (Tuple<int, AStarSearch, Troop> first, Tuple<int, AStarSearch, Troop> second)
            {
                return first.Item2.costSoFar[first.Item1].CompareTo(second.Item2.costSoFar[second.Item1]);

            });
        Tuple<int, AStarSearch, Troop> first = candidates.ToArray()[0];


        Debug.Log(" Nearest attackable " + first.Item1);
        int distToNearest = (int)first.Item2.costSoFar[first.Item1];
        int targetHex = first.Item1;
        //IF the nearest square is in range
        if (troop.TotalInitiative >= distToNearest)
        {
            //Attack the unit
            UIManager.Instance.AddLogText("AI chooses to attack " + first.Item3.UnitName());
            troop.MoveToAndAttack(mapManager.getHexById(targetHex).GetComponent<Hex>(), first.Item3, battleTurnManager);
        }
        else
        {
            //Else - step back using A*-s Came from, until in range
            while (distToNearest > troop.TotalInitiative)
            {
                distToNearest--;
                targetHex = first.Item2.cameFrom[targetHex];
            }
            UIManager.Instance.AddLogText("AI chooses to move to " + targetHex);
            troop.MoveTo(mapManager.getHexById(targetHex).GetComponent<Hex>(), battleTurnManager);
        }

    }



    public void BestTroopMove(Troop troop, BattleTurnManager battleTurnManager, MapManager mapManager)
    {
        if (troop.hasRangedAbility())
        {
            //Use it on the highest threat
            UIManager.Instance.AddLogText("AI decides to use ranged with " + troop.UnitName());
            AttackWithRange(troop, battleTurnManager, mapManager);
        }
        else if (troop.hasAreaAbility())
        {
            //Use it on the highest threat
            AttackWithArea(troop, battleTurnManager, mapManager);
        }
        else
        {
            AttackStrongestOpponent(troop, battleTurnManager, mapManager);
        }
    }

    public void AttackWithRange(Troop troop, BattleTurnManager battleTurnManager, MapManager mapManager)
    {
        ArmyManager army = GetOpposingArmy(troop, battleTurnManager);
        Troop highestThreat = GetStrongestOpponent(army);
        IAbilty abilty = troop.RangedAbility();
        if (abilty.CanBeApplied(troop, highestThreat))
        {
            abilty.Apply(troop, highestThreat);
        }
        battleTurnManager.FinishAction();

    }

    public void AttackWithArea(Troop troop, BattleTurnManager battleTurnManager, MapManager mapManager)
    {
        ArmyManager army = GetOpposingArmy(troop, battleTurnManager);
        Dictionary<int, int> troopLocations = new Dictionary<int, int>();
        foreach (Troop t in army.GetComponentsInChildren<Troop>())
        {
            troopLocations.Add(t.HexId, t.TroopId);
        }

        IAreaAbilty abilty = troop.AreaAbility();
        List<Tuple<int, Hex>> candidates = new List<Tuple<int, Hex>>();

        foreach (Hex hex in mapManager.Hexes)
        {
            List<int> targets = abilty.AbilityRange(hex);
            int enemies = 0;
            foreach (int target in targets)
            {
                if (troopLocations.ContainsKey(target))
                {
                    enemies++;
                }
            }
            candidates.Add(Tuple.Create(enemies, hex));
        }

        candidates.Sort(delegate (Tuple<int, Hex> first, Tuple<int, Hex> second)
        {
            return second.Item1 - first.Item1;
        });

        Tuple<int, Hex> selectedTarget = candidates.ToArray()[0];

        Debug.Log("AI chooses to cast area ability on " + selectedTarget.Item2.Id + " hitting a total of " + selectedTarget.Item1 + " enemies");

        List<Troop> targetEnemies = new List<Troop>();

        foreach (int hexId in abilty.AbilityRange(selectedTarget.Item2))
        {
            Troop targetEnemy = mapManager.getHexById(hexId).GetComponent<Hex>().occupyingTroop;
            if (targetEnemy != null)
            {
                Debug.Log("Adding " + targetEnemy.TroopId + " to targets");
                targetEnemies.Add(targetEnemy);
            }

        }

        abilty.Apply(troop, targetEnemies);

        battleTurnManager.FinishAction();
    }




    private class PriorityQueue<TElement, TPriority>
    {
        private List<Tuple<TElement, TPriority>> elements = new List<Tuple<TElement, TPriority>>();

        public int Count
        {
            get
            {
                return elements.Count;
            }
        }

        public void Enqueue(TElement item, TPriority priority)
        {
            elements.Add(Tuple.Create(item, priority));
        }

        public TElement Dequeue()
        {
            Comparer<TPriority> comparer = Comparer<TPriority>.Default;
            int bestIndex = 0;
            for (int i = 0; i < elements.Count; i++)
            {
                if (comparer.Compare(elements[i].Item2, elements[bestIndex].Item2) < 0)
                {
                    bestIndex = i;
                }
            }

            TElement bestItem = elements[bestIndex].Item1;
            elements.RemoveAt(bestIndex);
            return bestItem;
        }
    }

    private class AStarSearch
    {

        public Dictionary<int, int> cameFrom = new Dictionary<int, int>();

        public Dictionary<int, double> costSoFar = new Dictionary<int, double>();

        static public double Heuristic(Hex a, Hex b)
        {
            return (double)((
                    Math.Abs(a.Q - b.Q)
                    + Math.Abs(a.Q + a.R - b.Q - b.R)
                    + Math.Abs(a.R - a.R)
                ) / 2f);
        }

        public AStarSearch(Hex start, Hex goal)
        {
            var frontier = new PriorityQueue<Hex, double>();
            frontier.Enqueue(start, 0);
            cameFrom[start.Id] = start.Id;
            costSoFar[start.Id] = 0;
            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
                if (current.Id.Equals(goal.Id))
                {
                    break;
                }

                foreach (var next in current.FreeNeighbors)
                {

                    double newCost = costSoFar[current.Id] + 1;
                    if (!costSoFar.ContainsKey(next.Id) || newCost < costSoFar[next.Id])
                    {
                        costSoFar[next.Id] = newCost;
                        double priority = newCost + Heuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        cameFrom[next.Id] = current.Id;
                    }

                }
            }

        }




    }
}