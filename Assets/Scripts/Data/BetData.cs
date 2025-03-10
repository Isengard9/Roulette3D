using System;
using NCGames.Enums;
using UnityEngine;

namespace NCGames.Data
{
    [Serializable]
    public class BetData
    {
        [SerializeField] public BetType betType;
        [SerializeField] public int[] numbersOfBet;
        
    }
}