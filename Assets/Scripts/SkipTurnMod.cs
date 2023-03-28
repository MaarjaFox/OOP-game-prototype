using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BestGame.Effects;

public class SkipTurnMod : MonoBehaviour
{

    [SerializeField] public Modifier SkipRoundModifier;
   
    private static SkipTurnMod _instance;

    public static SkipTurnMod Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<SkipTurnMod>();
            }

            return _instance;
        }

    }
}

