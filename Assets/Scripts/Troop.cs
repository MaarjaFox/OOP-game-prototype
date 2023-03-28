using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Units;
using BestGame.Effects;
using BestGame.Interfaces;
using UnityEngine.UI;

public class Troop : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private int size = 1; //health??




    [SerializeField] public int HealthLeft = 1;

    [SerializeField]
    private List<Modifier> modifiers; //contains objects of type modifier

    private bool canRetaliate = true;  //can attack is set to true


    public int TotalInitiative //opportunity to act or take charge before others do?
    {
        get { return ApplyInitiativeModifiers(Unit.Initiative); }
    }


    [SerializeField] private GameObject prefab;

    public BaseUnit Unit;
    public ArmyManager _army;
    public int HexId = -1;
    public int TroopId = -1;
    public static volatile int LatestId = 0;


    void Start()
    {

        Unit = prefab.GetComponent<BaseUnit>(); // Get the BaseUnit component from the prefab
        _army = this.GetComponentInParent<ArmyManager>();
        HealthLeft = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH)); // Calculate the health of the unit based on health modifiers and set the HealthLeft variable
        LatestId++;// Increment the LatestId counter and set this Troop's TroopId variable to the new value
        TroopId = LatestId;
        var thisObj = Instantiate(this.prefab, this.gameObject.transform);
        foreach (var mod in modifiers)  // Instantiate and add all modifiers to the unit as children of the new unit prefab
        {
            var modif = Instantiate(mod, this.gameObject.transform);
            modif.transform.parent = thisObj.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    public List<Modifier> typeModifiers(Modifier.ModifierType type) // A function that returns a list of all the modifiers of a specific type
    {
        List<Modifier> allModifiers = new List<Modifier>();  // Create a new list to hold the modifiers
        foreach (var mod in this.GetComponentsInChildren<Modifier>())// Get all the modifiers on this game object and its children
        {
            if (mod.type == type) // If the modifier is of the specified type, add it to the list
            {
                allModifiers.Add(mod);
            }
        }

        foreach (var mod in _army.modifiers)  // Get all the modifiers from the army manager
        {
            if (mod.type == type) // If the modifier is of the specified type, add it to the list
            {
                allModifiers.Add(mod); // Return the list of modifiers of the specified type
            }
        }

        return allModifiers;
    }


    public void AddModifier(Modifier modifier)// A function that adds a modifier to the list of modifiers on this troop????
    {
        modifiers.Add(modifier);
    }

    public void Heal(int amount)
    {
        HealthLeft += amount;
        if (HealthLeft > Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH)))
        {
            HealthLeft = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH));
        }
    }

    public void PerformAttack(Troop otherTroop)
    {
        int initial = otherTroop.size; // Get the initial HEALTH of the other troop

        Unit.PerformPreAttack(otherTroop); // Perform pre-attack actions on the attacking unit


        int damage = 0;
        for (int i = 1; i <= size; i++) // Loop through each attacker in the unit
        {
            damage += Unit.CalculateDealtDamage(otherTroop.prefab.GetComponent<BaseUnit>(),
                typeModifiers(Modifier.ModifierType.ATTACK),
                otherTroop.typeModifiers(Modifier.ModifierType.DEFENSE),
                typeModifiers(Modifier.ModifierType.DEALING_DAMAGE));
                
        //Adding healer so to get the healer modifiers of the healing unit?

    }


        int dealtDamage = otherTroop.ApplyDealtDamage(damage, Unit); // Apply the dealt damage to the defending unit

        int killed = initial - otherTroop.size; 
        Unit.PerformPostAttack(otherTroop); 
        UIManager.Instance.AddLogText(UnitName() + " attacks " + otherTroop.UnitName() + " with a total of " + dealtDamage + " damage. " + killed + " killed");

        if (otherTroop.CanRetaliate()) // Check if the defending unit can retaliate/attack
        {
            otherTroop.Retaliate(this);
        }
    }


    public bool CanRetaliate()
    {
        return canRetaliate && Unit.CanRetaliate() && size > 0; //troop can attack if its still alive
    }

    public void HasRetaliated() //cannot retaliate again until ResetRetaliation() is called.
    {
        canRetaliate = false;
    }

    public void ResetRetaliation()
    {
        canRetaliate = true;

    }

    public void PerformRangedAbility(IAbilty ability, Troop target)
    {

    }
   
    public bool hasRangedAbility()
    {
        return Unit.HasRangedAbility();
    }

    public IAbilty RangedAbility()
    {
        return Unit.RangedAbility();
    }
    public void PerformMageAbility(IAbilty ability, Troop target)
    {

    }

    public void SkipTurn(Troop otherTroop)
    {
        Modifier mod = Instantiate(SkipTurnMod.Instance.SkipRoundModifier);
        otherTroop.AddModifier(mod);
    }
    public bool hasMageAbility()
    {
        return Unit.HasMageAbility();

    }

    public IAbilty MageAAbility()
    {
        return Unit.MageAAbility();
    }
    public bool hasAreaAbility()
    {
        return Unit.HasAreaAbility();
    }

    public IAreaAbilty AreaAbility()
    {
        return Unit.AreaAbility();
    }
    public bool hasPoisonAbility()
    {
        return Unit.HasPoisonAbility();
    }
  
    public IAreaAbilty PoisonRangedAbility()
    {
        return Unit.PoisonRangedAbility();
    }
    public bool hasTankAbility()
    {
        return Unit.HasTankAbility();
    }
    public IAreaAbilty TankAllAbility()
    {
        return Unit.TankAllAbility();
    }

    public void RestoreHealth(int amount)
    {
        HealthLeft += amount;
        if (HealthLeft > Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH)))
        {
            HealthLeft = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH));
        }
    }
    void Retaliate(Troop troop)
    {
        Unit.PerformPreRetaliate(this, troop);
        int size = troop.size;
        int damage = 0;
        for (int i = 1; i <= size; i++)
        {
            damage += Unit.CalculateDealtDamage(troop.prefab.GetComponent<BaseUnit>(),
               typeModifiers(Modifier.ModifierType.ATTACK),
               troop.typeModifiers(Modifier.ModifierType.DEFENSE),
               typeModifiers(Modifier.ModifierType.DEALING_DAMAGE));
        }

        int retaliationDamage = troop.ApplyDealtDamage(damage, Unit);
        int killed = size - troop.size;
        UIManager.Instance.AddLogText(UnitName() + " retaliates with a total of " + retaliationDamage + " damage. " + killed + " killed");

        if (troop.GetTotalRemainingHealth() < 1)  // If the troop has been defeated, it is destroyed.
        {
            //hex.occupyingTroop = null;

            Destroy(troop.gameObject);
            MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", troop.TroopId);
        }
        else  // Otherwise, the unit that just retaliated performs any necessary post-retaliation actions.
        {
            Unit.PerformPostRetaliate(this, troop); //does only acidwolf use this?

        }
    }


    public int ApplyDealtDamage(int damage, BaseUnit attacker)
    {

        int damageToApply = Unit.AppliedDamage(damage, attacker, typeModifiers(Modifier.ModifierType.DEALT_DAMAGE));

        int totalDamage = damageToApply;
        // Calculate the modified health of the unit using its CalculateHealth method and the typeModifiers for health.
        int modifiedHealth = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH));
        int totalHealth = GetTotalRemainingHealth();
        int remainingTotalHealth = totalHealth - totalDamage;


        if (remainingTotalHealth > 0) // If the remaining total health is greater than 0, calculate the new health and health left of the unit.
        {
            int remainingMod = remainingTotalHealth % modifiedHealth;

            if (remainingMod == 0)
            {
                size = remainingTotalHealth / modifiedHealth;
                HealthLeft = modifiedHealth;
            }
            else// If the remaining mod is not 0, the new size is the remaining total health divided by the modified health plus 1.
            {
                size = (remainingTotalHealth / modifiedHealth) + 1;  
                HealthLeft = remainingMod; 

            }
        }
        else
        {
            size = 0;
            HealthLeft = 0;
        }

        return totalDamage;
    }

    public int GetTotalRemainingHealth()
    {
        if (size == 0)
        {
            return 0;
        }
        else
        {
            int modifiedHealth = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH));
            return (size - 1) * modifiedHealth + HealthLeft;
            // If the unit has a positive size, the function calculates how much health is left
        }//any remaining health that doesnt make up full modified health point
        //can I just reverse it to (size + 1)... for healer?
    }

    public string UnitName()
    {
        return prefab.GetComponent<BaseUnit>().name;
    }

    int ApplyInitiativeModifiers(int baseInitiative) //returns an int
    {
        int initiative = baseInitiative;

        foreach (var modifier in _army.modifiers) //Loops over each modifier object in _army.modifiers.If the modifier object's type is INITIATIVE, then it applies the modifier to initiative.
        {
            if (modifier.type == Modifier.ModifierType.INITIATIVE)
            {
                initiative = modifier.Apply(initiative);
            }
        }

        foreach (var modifier in modifiers) //loop in modifier
        {
            if (modifier.type == Modifier.ModifierType.INITIATIVE)
            {
                initiative = modifier.Apply(initiative);
            }
        }
        if (initiative <= 1)
        {
            initiative = 1;
        }
        return initiative;
       
    }

    public void InitializeAt(Hex hex)// Initialize the troop at the given hex, setting its position, moving it to the hex, and setting its ID
    {
        this.transform.position = hex.Position();// Set the troop's position to the center of the hex
        hex.MoveTroopTo(this);// Move the troop to the hex by setting its occupyingTroop
        HexId = hex.Id; // Set the troop's HexId to the hex's ID

    }
    // Move the troop to the given hex, optionally passing in a turn manager to handle the action
    public void MoveTo(Hex hex, BattleTurnManager turnManager = null)
    {

        turnManager.SetPerformingAction(); // Set the turn manager to the "performing action" state
        if (hex.occupyingTroop != null) // If the hex is already occupied by another troop
        {
            // If the occupying troop is not in the same army as this troop
            if (hex.occupyingTroop._army.GetInstanceID() != _army.GetInstanceID())
            {
                hex.manager.HighlightAttackableTiles(hex); 
                turnManager.SetPerformingAttack();
            }
            else  // If the occupying troop is in the same army as this troop
            {
                //To be continued with casting benefitial effects or support behavior
                turnManager.SetDefaultState();
            }

        }
        else // If the hex is not already occupied by another troop
        {
            hex.MoveTroopTo(this);
            HexId = hex.Id;
            StartCoroutine(LerpPosition(hex.Position(), 0.6f, turnManager)); //move speed
        }

    }

    public void MoveToAndAttack(Hex hex, Troop target, BattleTurnManager turnManager = null)
    {


        hex.MoveTroopTo(this);
        HexId = hex.Id;
        StartCoroutine(LerpPositionAndAttack(hex, 0.3f, target, turnManager));

    }


    IEnumerator LerpPosition(Vector3 targetPosition, float duration, BattleTurnManager manager = null)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        if (manager)
        {
            manager.FinishAction();
        }
    }

    IEnumerator LerpPositionAndAttack(Hex hex, float duration, Troop target, BattleTurnManager manager = null)
    {
        float time = 0;
        Vector3 startPosition = transform.position;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, hex.Position(), time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = hex.Position();
        PerformAttack(target);

        if (target.GetTotalRemainingHealth() < 1)
        {
            //hex.occupyingTroop = null;

            Destroy(target.gameObject);
            MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", target.TroopId);
        }

        if (manager)
        {
            manager.FinishAction();
        }
    }

    //public void OnTurnUpdated()
   // {

       // foreach (var modifier in this.GetComponentsInChildren<Modifier>())
       // {
       //     modifier.OnTurnUpdated();
       // }

       // canRetaliate = true;

      
    //}
    public void OnTurnUpdated()
    {
        var modifiers = GetComponents<Modifier>();
        foreach (var modifier in modifiers)
        {
            modifier.OnTurnUpdated();
        }
        canRetaliate = true;
    }

}