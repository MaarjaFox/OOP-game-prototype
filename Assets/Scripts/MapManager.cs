using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.ShortcutManagement;
using UnityEngine;


public class MapManager : MonoBehaviour
{
    public enum HexHighight
    {
        ACTIVE_UNIT,
        MOVABLE_RANGE,
        ATTACKABLE_HEX,
        NEIGHBOR_COLOR,
        DEFAULT
    }
    public GameObject _hexPrefab;

    private static int incrementingId = 0;

    [SerializeField]
    private Dictionary<int, GameObject> _tiles = new Dictionary<int, GameObject>();

    private Dictionary<Tuple<int, int>, int> _coordinatesToId = new Dictionary<Tuple<int, int>, int>();

    public int rowCount;
    public int columnCount;

    [SerializeField]
    public Color clickedColor;
    [SerializeField]
    public Color rangeColor;
    [SerializeField]
    public Color defaultColor;
    [SerializeField]
    public Color neighborColor;
    [SerializeField]
    public Color attackableColor;
    private BattleTurnManager _battleManager;

    public TMPro.TMP_Text _selectedGridElement;

    public List<Hex> Hexes = new List<Hex>();


    public Hex clickedTile = null;

    private List<int> _selectedRange = new List<int>();

    [CanBeNull]
    [SerializeField]
    public static Tuple<int, int>[][] StartingCoords =
    {
        new[]{
            new Tuple<int, int>(-4, 8),
            new Tuple<int, int>(-3, 6),
            new Tuple<int, int>(-2, 4),
            new Tuple<int, int>(-1, 2),
        },
        new[]{
            new Tuple<int, int>(13, 9),
            new Tuple<int, int>(14, 7),
            new Tuple<int, int>(15, 5),
            new Tuple<int, int>(16, 3),
        }
    };




    [SerializeField]
    private int[][] player2StartingCoords;




    public GameObject getHexById(int id)
    {

        if (_tiles.ContainsKey(id))
        {
            return _tiles[id];
        }

        return null;
    }

    public GameObject getHexByCoords(int col, int row)
    {
        if (_coordinatesToId.ContainsKey(new Tuple<int, int>(col, row)))
        {
            return _tiles[_coordinatesToId[new Tuple<int, int>(col, row)]];
        }
        else
        {
            throw new KeyNotFoundException();
        }
    }

    public void HighlightAttackableTiles(Hex hex)
    {
        List<int> attackableIds = new List<int>();
        List<GameObject> neighbors = hex.Neighbors;
        Hex checkingHex;
        foreach (var neighbor in neighbors)
        {
            checkingHex = neighbor.GetComponent<Hex>();
            if (_selectedRange.Contains(checkingHex.Id) && checkingHex.occupyingTroop == null)
            {
                attackableIds.Add(checkingHex.Id);
                HiglightHex(getHexById(checkingHex.Id).GetComponent<Hex>(), HexHighight.ATTACKABLE_HEX);
            }
        }

        if (attackableIds.Count > 0)
        {
            //  UnhighlightSelectedRange();
            UnhighlightSelectedRange();
            _selectedRange = attackableIds;
            foreach (var id in attackableIds)
            {
                HiglightHex(getHexById(id).GetComponent<Hex>(), HexHighight.ATTACKABLE_HEX);
            }
            _battleManager.SetTarget(hex.occupyingTroop);

        }


    }


    public bool HexInCurrentRange(Hex hex)
    {
        return _selectedRange.Contains(hex.Id);
    }

    public void GenerateMap()
    {

        for (int col = 0; col < columnCount; col++)
        {
            for (int row = 0; row < rowCount; row++)
            {

                // Hex h= new Hex(col - row / 2, row);
                incrementingId++;
                var hexTile = Instantiate(_hexPrefab,
                    new Vector3(0, 0, 0),
                    Quaternion.identity,
                    this.transform);

                hexTile.gameObject.AddComponent<Hex>();
                hexTile.gameObject.GetComponent<Hex>().PositionHex(col - row / 2, row, incrementingId, this);
                hexTile.gameObject.GetComponentInChildren<TileClickBehavior>()._manager = this;
                hexTile.gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", defaultColor);

                _tiles.Add(incrementingId, hexTile);
                Hexes.Add(hexTile.GetComponent<Hex>());
                _coordinatesToId.Add(new Tuple<int, int>(col - row / 2, row), incrementingId);

            }
        }

    }

    public void TileClicked(Hex newTile)
    {
        _battleManager.HexClicked(newTile);
    }

    public void TileHoverEntered(Hex newTile)
    {
        _battleManager.TileHoverEntered(newTile);
    }
    public void TileHoverExited(Hex newTile)
    {
        _battleManager.TileHoverExited(newTile);
    }

    public void AttachListener(BattleTurnManager manager)
    {
        this._battleManager = manager;
    }


    public void HiglightHex(Hex hex, HexHighight highlight)
    {
        switch (highlight)
        {
            case HexHighight.DEFAULT:
                hex.GetComponentInChildren<Renderer>().material.SetColor("_Color", defaultColor);
                break;
            case HexHighight.ACTIVE_UNIT:
                hex.GetComponentInChildren<Renderer>().material.SetColor("_Color", clickedColor);
                break;
            case HexHighight.MOVABLE_RANGE:
                hex.GetComponentInChildren<Renderer>().material.SetColor("_Color", rangeColor);
                break;
            case HexHighight.ATTACKABLE_HEX:
                hex.GetComponentInChildren<Renderer>().material.SetColor("_Color", attackableColor);
                break;
        }
    }


    public void HighlightNeighbors(Hex hex)
    {
        foreach (var element in hex.Neighbors)
        {
            element.GetComponentInChildren<Renderer>().material.SetColor("_Color", neighborColor);
        }
    }


    public void UnHighlightNeighbors(Hex hex)
    {
        foreach (var element in hex.Neighbors)
        {
            element.GetComponentInChildren<Renderer>().material.SetColor("_Color", defaultColor);
        }
    }

    private void UnhighlightSelectedRange()
    {
        foreach (int id in _selectedRange)
        {

            HiglightHex(_tiles[id].GetComponent<Hex>(), HexHighight.DEFAULT);
        }
    }


    public void HighlightRange(Hex hex, int range, HexHighight highlightColor = HexHighight.MOVABLE_RANGE)
    {

        UnhighlightSelectedRange();

        _selectedRange = GetTilesInRange(hex, range);

        HighlightListOfTiles(_selectedRange, highlightColor);
    }

    public void HighlightListOfTiles(List<int> ids, HexHighight highlightColor = HexHighight.MOVABLE_RANGE)
    {
        UnhighlightSelectedRange();
        _selectedRange = ids;
        foreach (int id in ids)
        {
            HiglightHex(_tiles[id].GetComponent<Hex>(), highlightColor);
        }

    }
    public List<int> GetTilesInRange(Hex hex, int range)
    {
        List<int> ids = new List<int>();
        for (int i = -range; i <= range; i++)
        {
            for (int j = Math.Max(-range, -i - range); j <= Math.Min(range, -i + range); j++)
            {
                try
                {
                    ids.Add(_coordinatesToId[new Tuple<int, int>(hex.Q + i, hex.R + j)]);
                }
                catch (KeyNotFoundException ex)
                {

                }

            }
        }

        return ids;
    }

}