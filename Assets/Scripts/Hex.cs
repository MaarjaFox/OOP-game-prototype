using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hex : MonoBehaviour, IBusSubscriber
{
    // Base class for cubic hex coords
    // https://www.redblobgames.com/grids/hexagons/
    //Q + R + S = 0





    public int Q;
    public int R;
    public int S;
    public int Id;
    public MapManager manager;
    private int _busSubscriberId;


    public static Tuple<int, int>[] neighborMap =
    {
        new Tuple<int,int> ( 1, 0),
        new Tuple<int,int> ( 1,-1),
        new Tuple<int,int> ( 0,-1),
        new Tuple<int,int> (-1, 0),
        new Tuple<int,int> (-1, 1),
        new Tuple<int,int> ( 0, 1),
    };


    private bool positionCached = false;
    private Vector3 _position;


    public Troop occupyingTroop;
    private List<GameObject> _neighbors = null;


    public void TroopDestroyed(int hexId)
    {
        if (hexId == Id)
        {
            occupyingTroop = null;
        }
    }

    public List<GameObject> Neighbors {
        get
        {
            if (_neighbors == null)
            {
                _neighbors = calculateNeighbors();
            }

            return _neighbors;
        }
    }

    public List<Hex> FreeNeighbors {
       get {
            List<Hex> list = new List<Hex>();
            Hex hex;
            foreach (GameObject go in Neighbors) {
                hex = go.GetComponent<Hex>();
                if (hex.occupyingTroop == null) {
                    list.Add(hex);
                }
            }
            return list;
       }
    
    }

    static readonly float WIDTH_CONST = Mathf.Sqrt(3) / 2 ;
    
    public void PositionHex(int q, int r, int Id, MapManager manager)
    {
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
        this.Id = Id;
        this.manager = manager;
        this.gameObject.transform.position = Position();
        MessageBus.Instance.Subscribe(this);

    }
  
    //World-space position
    public Vector3 Position()
    {

        if (!positionCached)
        {
            float radius = 1f;
            float height = radius * 2;
            float width =  WIDTH_CONST * height;

            float horizontal_spacing = width;
            float vertical_spacing = height * 0.75f;
        
            _position = new Vector3(
                horizontal_spacing  * (this.Q + this.R/2f),
                0f,
                vertical_spacing  *this.R
            );    
        }

        return _position;

    }

    public void MoveTroopTo(Troop troop)
    {
        occupyingTroop = troop;
        if (troop.HexId > -1)
        {
            manager.getHexById(troop.HexId).GetComponent<Hex>().occupyingTroop = null;
        }

    }

    private List<GameObject> calculateNeighbors()
    {
        List<GameObject> neighbors = new List<GameObject>();
       
        foreach (var tuple in neighborMap)
        {
            try
            {
                neighbors.Add(manager.getHexByCoords(this.Q + tuple.Item1, this.R + tuple.Item2));
            }
             catch (KeyNotFoundException ex)
            {

            }
            
        }


        return neighbors;
    }


    public void ReceiveMessage(MessageBus.MessageTypes type, string str, int int_param)
    {
        switch (type)
        {
            case MessageBus.MessageTypes.TROOP_DESTROYED:
                if (occupyingTroop != null)
                {
                    if (occupyingTroop.TroopId == int_param)
                    {
                        occupyingTroop = null;
                    }
                }

                break;
        }
    }

    public int BusSubscriberId()
    {
        return _busSubscriberId;
    }
}
