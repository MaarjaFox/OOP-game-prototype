using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBus : MonoBehaviour
{
    // Start is called before the first frame update

    public enum MessageTypes
    {
        TROOP_DESTROYED,
        TROOP_HEALED
    }
    private static MessageBus _instance;
    private static int latestSubscriberId = 0;
    
    
    private Dictionary<int, IBusSubscriber> _subscribers = new Dictionary<int, IBusSubscriber>();
    
    
    
    public static MessageBus Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MessageBus>();
            }
            return _instance;
        }     
    }

    public int Subscribe(IBusSubscriber subscriber)
    {
        latestSubscriberId++;
        _subscribers.Add(latestSubscriberId,    subscriber);
        return latestSubscriberId;
    }


    public void Broadcast(MessageBus.MessageTypes type, string str, int int_param)
    {
        foreach (var subscriber in _subscribers.Values)
        {
            subscriber.ReceiveMessage(type, str, int_param);
        }
    }
    
    public void Send(int RecipientId, MessageBus.MessageTypes type, string str, int int_param)
    {
        _subscribers[RecipientId].ReceiveMessage(type, str, int_param);
    }
    
}


public interface IBusSubscriber
{
    void ReceiveMessage(MessageBus.MessageTypes type, string str, int int_param);
    int BusSubscriberId();
}