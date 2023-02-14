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
    private int size = 1;




    [SerializeField] public int HealthLeft = 1;

    [SerializeField]
    private List<Modifier> modifiers;

    private bool canRetaliate = true;

    public int TotalInitiative
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

        Unit = prefab.GetComponent<BaseUnit>();
        _army = this.GetComponentInParent<ArmyManager>();
        HealthLeft = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH));
        LatestId++;
        TroopId = LatestId;
        var thisObj = Instantiate(this.prefab, this.gameObject.transform);
        foreach (var mod in modifiers)
        {
            var modif = Instantiate(mod, this.gameObject.transform);
            modif.transform.parent = thisObj.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }


    public List<Modifier> typeModifiers(Modifier.ModifierType type)
    {
        List<Modifier> allModifiers = new List<Modifier>();
        foreach (var mod in this.GetComponentsInChildren<Modifier>())
        {
            if (mod.type == type)
            {
                allModifiers.Add(mod);
            }
        }

        foreach (var mod in _army.modifiers)
        {
            if (mod.type == type)
            {
                allModifiers.Add(mod);
            }
        }

        return allModifiers;
    }


    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
    }


    public void PerformAttack(Troop otherTroop)
    {
        int initial = otherTroop.size;

        Unit.PerformPreAttack(otherTroop);


        int damage = 0;
        for (int i = 1; i <= size; i++)
        {
            damage += Unit.CalculateDealtDamage(otherTroop.prefab.GetComponent<BaseUnit>(),
                typeModifiers(Modifier.ModifierType.ATTACK),
                otherTroop.typeModifiers(Modifier.ModifierType.DEFENSE),
                typeModifiers(Modifier.ModifierType.DEALING_DAMAGE));

        }


        int dealtDamage = otherTroop.ApplyDealtDamage(damage, Unit);

        int killed = initial - otherTroop.size;
        Unit.PerformPostAttack(otherTroop);
        UIManager.Instance.AddLogText(UnitName() + " attacks " + otherTroop.UnitName() + " with a total of " + dealtDamage + " damage. " + killed + " killed");

        if (otherTroop.CanRetaliate())
        {
            otherTroop.Retaliate(this);
        }
    }


    public bool CanRetaliate()
    {
        return canRetaliate && Unit.CanRetaliate() && size > 0;
    }

    public void HasRetaliated()
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

    public bool hasAreaAbility()
    {
        return Unit.HasAreaAbility();
    }

    public IAreaAbilty AreaAbility()
    {
        return Unit.AreaAbility();
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

        if (troop.GetTotalRemainingHealth() < 1)
        {
            //hex.occupyingTroop = null;

            Destroy(troop.gameObject);
            MessageBus.Instance.Broadcast(MessageBus.MessageTypes.TROOP_DESTROYED, "", troop.TroopId);
        }
        else
        {
            Unit.PerformPostRetaliate(this, troop);

        }
    }


    public int ApplyDealtDamage(int damage, BaseUnit attacker)
    {

        int damageToApply = Unit.AppliedDamage(damage, attacker, typeModifiers(Modifier.ModifierType.DEALT_DAMAGE));

        int totalDamage = damageToApply;

        int modifiedHealth = Unit.CalculateHealth(typeModifiers(Modifier.ModifierType.HEALTH));
        int totalHealth = GetTotalRemainingHealth();
        int remainingTotalHealth = totalHealth - totalDamage;


        if (remainingTotalHealth > 0)
        {
            int remainingMod = remainingTotalHealth % modifiedHealth;

            if (remainingMod == 0)
            {
                size = remainingTotalHealth / modifiedHealth;
                HealthLeft = modifiedHealth;
            }
            else
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
        }
    }

    public string UnitName()
    {
        return prefab.GetComponent<BaseUnit>().name;
    }

    int ApplyInitiativeModifiers(int baseInitiative)
    {
        int initiative = baseInitiative;

        foreach (var modifier in _army.modifiers)
        {
            if (modifier.type == Modifier.ModifierType.INITIATIVE)
            {
                initiative = modifier.Apply(initiative);
            }
        }

        foreach (var modifier in modifiers)
        {
            if (modifier.type == Modifier.ModifierType.INITIATIVE)
            {
                initiative = modifier.Apply(initiative);
            }
        }

        return initiative;
    }

    public void InitializeAt(Hex hex)
    {
        this.transform.position = hex.Position();
        hex.MoveTroopTo(this);
        HexId = hex.Id;

    }

    public void MoveTo(Hex hex, BattleTurnManager turnManager = null)
    {

        turnManager.SetPerformingAction();
        if (hex.occupyingTroop != null)
        {

            if (hex.occupyingTroop._army.GetInstanceID() != _army.GetInstanceID())
            {
                hex.manager.HighlightAttackableTiles(hex);
                turnManager.SetPerformingAttack();
            }
            else
            {
                //To be continued with casting benefitial effects or support behavior
                turnManager.SetDefaultState();
            }

        }
        else
        {
            hex.MoveTroopTo(this);
            HexId = hex.Id;
            StartCoroutine(LerpPosition(hex.Position(), 0.3f, turnManager));
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

    public void OnTurnUpdated()
    {
        foreach (var modifier in this.GetComponentsInChildren<Modifier>())
        {
            modifier.OnTurnUpdated();
        }

        canRetaliate = true;
    }


}