using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BestGame.Units;
using BestGame.Interfaces;
using UnityEngine;




public class BattleTurnManager : MonoBehaviour, IBusSubscriber
{
    // Start is called before the first frame update
    [SerializeField]
    private MapManager _map;
    [SerializeField] public ArmyManager[] _armies;



    private BattleTurnManager.STATES state;

    public BattleTurnManager.STATES State
    {
        get { return state; }
        set
        {
            state = value;
            UIManager.Instance.SetStateText(state.ToString());
        }
    }


    private List<Troop> _troops = new List<Troop>();
    private List<Troop> _unactedTroops = new List<Troop>();
    private int subscriberId;
    [SerializeField] private Troop _actingTroop = null;
    [SerializeField] private Troop _targetedTroop = null;
    [SerializeField] private int _actingArmyId;
    private IAbilty _selectedAbilty = null;
    private IAreaAbilty _selectedAreaAbilty = null;
    public enum STATES
    {
        STATE_INITIALIZING,
        STATE_LEFT_TURN,
        STATE_RIGHT_TURN,
        STATE_PERFORMING_ACTION,
        STATE_PERFORMING_RANGED_ABILITY,
        STATE_PERFORMING_HEAL_ABILITY,
        STATE_PERFORMING_AREA_ABILITY,
        STATE_DEFAULT,
        STATE_PERFORMING_ATTACK,
        STATE_ENDED
    }
    void Start()
    {
        _map.GenerateMap();
        _map.AttachListener(this);
        State = STATES.STATE_DEFAULT;
        _positionArmies();
        subscriberId = MessageBus.Instance.Subscribe(this);
        Debug.Log("Armies " + _armies.Length);


    }

    // Update is called once per frame
    void Update()
    {
        if (State != STATES.STATE_DEFAULT)
        {
            return;
        }

        if (_unactedTroops.Count == 0)
        {
            foreach (var troop in _troops)
            {
                troop.OnTurnUpdated();
                _unactedTroops.Add(troop);
            }

            OnTurnUpdated();
        }

        if (_actingTroop == null)
        {
            foreach (var army in _armies)
            {
                int count = army.GetComponentsInChildren<Troop>().Length;
                if (count == 0)
                {
                    UIManager.Instance.DisplayEndText(army.PlayerName);
                    State = STATES.STATE_ENDED;
                }
            }

            _getNextActor();
            if (_actingTroop._army.IsAi)
            {

                AiBehavior.Instance.BestTroopMove(_actingTroop, this, _map);


            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _actingTroop = null;
        }

    }

    public void SetPerformingAttack()
    {
        this.State = STATES.STATE_PERFORMING_ATTACK;
    }

    public void SetDefaultState()
    {
        this.State = STATES.STATE_DEFAULT;

    }
    public void SetPerformingRangedAbility()
    {
        this.State = STATES.STATE_PERFORMING_RANGED_ABILITY;
        UIManager.Instance.HideRangedAttackButton();
        _highlightTargetableTroops();
    }
    public void SetPerformingMageAbility()
    {

        this.State = STATES.STATE_PERFORMING_RANGED_ABILITY; //////MAGE ABILITY IS THIS
       UIManager.Instance.HideMageButton();
        UIManager.Instance.HideHealButton();
        _highlightTargetableTroops();

    }

    public void SetPerformingHealAbility()
    {
        this.State = STATES.STATE_PERFORMING_HEAL_ABILITY;
        UIManager.Instance.HideMageButton();
        UIManager.Instance.HideHealButton();
        
        // _highlightTargetableTroops();
    }
    public void SetPerformingPoisonAbility()
    {
        this.State = STATES.STATE_PERFORMING_AREA_ABILITY;
        UIManager.Instance.HidePoisonButton();
        _highlightTargetableTroops();
    }
    public void SetPerformingTankAbility()
    {
        this.State = STATES.STATE_PERFORMING_AREA_ABILITY;
        UIManager.Instance.HideTankButton();
        UIManager.Instance.HideRiskButton(); ////please work
        _highlightTargetableTroops();
    }
    public void SetPerformingAreaAbility()
    {
        this.State = STATES.STATE_PERFORMING_AREA_ABILITY;
        UIManager.Instance.HideAreaButton();
    }

    private void _highlightTargetableTroops()
    {
        List<int> _tilesToHighlight = new List<int>();
        foreach (Troop troop in _troops)
        {
            if (_selectedAbilty.CanBeApplied(_actingTroop, troop))
            {
                _tilesToHighlight.Add(troop.HexId);
            }
        }
        _map.HighlightListOfTiles(_tilesToHighlight, MapManager.HexHighight.ATTACKABLE_HEX);
    }

    public void SetPerformingAction()
    {
        this.State = STATES.STATE_PERFORMING_ACTION;
    }



    public void CancelAction()
    {
        this.State = STATES.STATE_DEFAULT;
    }

    public void FinishAction()
    {
        _actingTroop = null;
        _targetedTroop = null;
        this.State = STATES.STATE_DEFAULT;

    }

    public void SetTarget(Troop troop)
    {
        this._targetedTroop = troop;
    }


    public void TileHoverEntered(Hex tile)
    {
        if (_actingTroop != null)
        {

            switch (state)
            {
                case STATES.STATE_PERFORMING_AREA_ABILITY:
                    List<int> ids = _selectedAreaAbilty.AbilityRange(tile);
                    _map.HighlightListOfTiles(ids, MapManager.HexHighight.ATTACKABLE_HEX);
                    break;
            }
        }
    }
    public void TileHoverExited(Hex tile)
    {

    }



    public void HexClicked(Hex hex)
    {

        //switch possible actions here
        if (_actingTroop != null)
        {

            switch (state)
            {
                case STATES.STATE_DEFAULT:
                    if (_map.HexInCurrentRange(hex) && hex.Id != _actingTroop.HexId)
                    {

                        _map.getHexById(_actingTroop.HexId).GetComponent<Hex>().occupyingTroop = null;
                        _actingTroop.MoveTo(hex, this);

                    }


                    break;
                case STATES.STATE_PERFORMING_RANGED_ABILITY:
                    //If there are valid targets
                    Debug.Log(hex.occupyingTroop);
                    Debug.Log(hex.Id);
                    Debug.Log(_selectedAbilty);
                    if (_selectedAbilty.CanBeApplied(_actingTroop, hex.occupyingTroop))
                    {


                        _selectedAbilty.Apply(_actingTroop, hex.occupyingTroop);
                    }
                    FinishAction(); 

                    break;

                case STATES.STATE_PERFORMING_HEAL_ABILITY:
                    
                    if (_selectedAbilty.CanBeApplied(_actingTroop, null))
                    {
                        _selectedAbilty.Apply(_actingTroop, null);
                    }

                    FinishAction();
                    Debug.Log("FinishAction() called"); //why is it not being called?
                    break;

                case STATES.STATE_PERFORMING_AREA_ABILITY:

                    //If there are valid targets
                    List<int> hexIds = _selectedAreaAbilty.AbilityRange(hex);
                    List<Troop> troops = new List<Troop>();
                    foreach (int hexId in hexIds)
                    {

                        Troop other = _map.getHexById(hexId).GetComponent<Hex>().occupyingTroop;
                        if (other != null)
                        {
                            troops.Add(other);
                        }
                    }
                    _selectedAreaAbilty.Apply(_actingTroop, troops);
                    FinishAction();

                    break;
                    ////////////////
               // case STATES.STATE_PERFORMING_POISON_ABILITY:
                 //   Debug.Log(hex.occupyingTroop);
                 //   Debug.Log(hex.Id);
                  //  Debug.Log(_selectedAbilty);
                  //  if (_selectedAbilty.CanBeApplied(_actingTroop, hex.occupyingTroop))
                    //{


                  //      _selectedAbilty.Apply(_actingTroop, hex.occupyingTroop);
                  //  }

                   // FinishAction();
                    ////////////////////
                  //  break;
                case STATES.STATE_PERFORMING_ATTACK:
                    if (_map.HexInCurrentRange(hex))
                    {

                        _map.getHexById(_actingTroop.HexId).GetComponent<Hex>().occupyingTroop = null;
                        _actingTroop.MoveToAndAttack(hex, _targetedTroop, this);


                    }
                    else
                    {
                        State = STATES.STATE_DEFAULT;
                        _targetedTroop = null;
                    }

                    break;
            }


        }

    }

    private void _getNextActor()
    {
        _unactedTroops.Sort(
            delegate (Troop troop, Troop troop1)
            {
                return troop1.TotalInitiative.CompareTo(troop.TotalInitiative);

            });

        _actingTroop = _unactedTroops.First();
        _actingArmyId = _actingTroop._army.ArmyId;
        checkTroopAbilities();

        Hex troopHex = _map.getHexById(_actingTroop.HexId).GetComponent<Hex>();
        _map.HighlightRange(troopHex, _actingTroop.TotalInitiative);
        _map.HiglightHex(troopHex, MapManager.HexHighight.ACTIVE_UNIT);
        _unactedTroops.Remove(_actingTroop);

    }

    private void checkTroopAbilities()
    {
        _selectedAbilty = null;
        _selectedAreaAbilty = null;

        if (_actingTroop.hasRangedAbility())
        {
            UIManager.Instance.ShowRangedAttackButton();
            _selectedAbilty = _actingTroop.RangedAbility();
        }
        else
        {

            UIManager.Instance.HideRangedAttackButton();
        }

        if (_actingTroop.hasMageAbility())
        {
            UIManager.Instance.ShowMageButton();
            UIManager.Instance.ShowHealButton();
            _selectedAbilty = _actingTroop.MageAAbility();
           // Heal.Instance.HealAllTroops();
            Heal.Instance.ResetHealAbility();
        }
        else
        {
            UIManager.Instance.HideHealButton();
            UIManager.Instance.HideMageButton();
            
        }
        //////////////
        if (_actingTroop.hasPoisonAbility())
        {
            UIManager.Instance.ShowPoisonButton();
            _selectedAreaAbilty = _actingTroop.PoisonRangedAbility();
        }
        else

        //////////////////
        {

            UIManager.Instance.HidePoisonButton();
        }
        if (_actingTroop.hasTankAbility())
        {
            UIManager.Instance.ShowTankButton();
            UIManager.Instance.ShowRiskButton();
            _selectedAreaAbilty = _actingTroop.TankAllAbility();
            //Risk.Instance.DestroyRandomTroop();
            Risk.Instance.ResetRiskAbility();
        }
        else
        {

            UIManager.Instance.HideTankButton();
            UIManager.Instance.HideRiskButton();
        }
        if (_actingTroop.hasAreaAbility())
        {
            UIManager.Instance.ShowAreaButton();
            _selectedAreaAbilty = _actingTroop.AreaAbility();
        }
        else
        {
            UIManager.Instance.HideAreaButton();
        }
    }


    private void _positionArmies()
    {
        int armyIdx = 0;
        foreach (var army in _armies)
        {
            _positionArmy(armyIdx, army);
            armyIdx++;
        }
        Debug.Log("Troops " + _troops.Count);
    }

    private void _positionArmy(int armyIdx, ArmyManager army)
    {
        int troopIdx = 0;
        foreach (var troop in army.GetComponentsInChildren<Troop>())
        {
            _positionTroop(armyIdx, troopIdx, troop);
            troopIdx++;
        }
    }

    private void _positionTroop(int armyIdx, int troopIdx, Troop troop)
    {

        Debug.Log("Adding troop " + troop.UnitName());
        _troops.Add(troop);
        Tuple<int, int> coords = MapManager.StartingCoords[armyIdx][troopIdx];
        troop.InitializeAt(_map.getHexByCoords(coords.Item1, coords.Item2).GetComponent<Hex>());
    }

    public void OnTurnUpdated()
    {
        foreach (var army in _armies)
        {
            army.OnTurnUpdated();
        }

    }
    public void ReceiveMessage(MessageBus.MessageTypes type, string str, int int_param)
    {

        switch (type)
        {
            case MessageBus.MessageTypes.TROOP_DESTROYED:
                CleanupDestroyedTroop(int_param);

                break;

        }
    }

    public int BusSubscriberId()
    {
        return subscriberId;
    }

    void CleanupDestroyedTroop(int TroopId)
    {
        var foundTroop = _troops.SingleOrDefault(t => t.TroopId == TroopId);
        if (foundTroop != null)
        {
            _troops.Remove(foundTroop);

        }

        var unactedTroop = _unactedTroops.SingleOrDefault(t => t.TroopId == TroopId);
        if (unactedTroop != null)
        {
            _unactedTroops.Remove(unactedTroop);

        }



        foreach (var army in _armies)
        {
            int count = army.GetComponentsInChildren<Troop>().Length;
            if (count == 0)
            {
                UIManager.Instance.DisplayEndText(army.PlayerName);
                State = STATES.STATE_ENDED;
            }
        }
    }

}