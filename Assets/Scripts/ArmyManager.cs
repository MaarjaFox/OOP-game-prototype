using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Effects;


public class ArmyManager : MonoBehaviour, IBusSubscriber
{
    // Start is called before the first frame update
    [SerializeField]
    public List<Modifier> modifiers;

    public static int HighestId = 0;

    [SerializeField]
    private bool _isAi;

    public bool IsAi
    {
        get
        {
            return _isAi;
        }
    }

    [SerializeField] public string PlayerName;
    public int ArmyId = -1;
    private int subscriberId;

    void Start()
    {
        ArmyId = ++ArmyManager.HighestId;
        subscriberId = MessageBus.Instance.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTurnUpdated()
    {
        foreach (var modifier in modifiers)
        {
            modifier.OnTurnUpdated();
        }
    }

    public void ReceiveMessage(MessageBus.MessageTypes type, string str, int int_param)
    {

        switch (type)
        {
            case MessageBus.MessageTypes.TROOP_DESTROYED:

                break;
        }
    }

    public int BusSubscriberId()
    {
        return subscriberId;
    }
}