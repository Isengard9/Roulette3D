using System;
using System.Collections.Generic;
using System.Linq;
using NCGames.Data;
using NCGames.Enums;
using NCGames.Events.Bet;
using NCGames.Managers;
using UnityEngine;

namespace NCGames.Services
{
    [Serializable]
    public class BetService
    {
        [Serializable]
        public struct Bet
        {
            [SerializeField] public BetType betType;
            [SerializeField] public int[] numbers;
            [SerializeField] public int amount;
        }

        [SerializeField] public List<Bet> currentBets;

        public BetService()
        {
            currentBets = new List<Bet>();
        }
        
        public void PlaceBet(BetType betType, int[] numbers, int amount)
        {
            Bet newBet = new Bet
            {
                betType = betType,
                numbers = numbers,
                amount = amount
            };

            currentBets.Add(newBet);
            ServiceContainer.Instance.EventPublisherService.Publish(new OnBetUpdatedEvent());
        }
        
        public Bet? GetBet(BetData currentBetData)
        {
            foreach (var bet in currentBets)
            {
                if (bet.betType == currentBetData.betType && 
                    
                    bet.numbers.SequenceEqual(currentBetData.numbersOfBet))
                {
                    return bet;
                }
            }
            return null;
        }
        
        public void UpdateBet(Bet updateBet, int newAmount)
        {
            for (int i = 0; i < currentBets.Count; i++)
            {
                if (currentBets[i].betType == updateBet.betType &&
                    currentBets[i].numbers.SequenceEqual(updateBet.numbers))
                {
                    currentBets[i] = new Bet
                    {
                        betType = updateBet.betType,
                        numbers = updateBet.numbers,
                        amount = newAmount
                    };
                    break;
                }
            }
            
            ServiceContainer.Instance.EventPublisherService.Publish(new OnBetUpdatedEvent());
        }
        
        public void RemoveBet(Bet? currentBet)
        {
            if (currentBet.HasValue)
            {
                currentBets.RemoveAll(bet => 
                    bet.betType == currentBet.Value.betType &&
                    bet.numbers.SequenceEqual(currentBet.Value.numbers));
            }
            
            ServiceContainer.Instance.EventPublisherService.Publish(new OnBetUpdatedEvent());
        }

        public float EvaluateBets(int winningNumber)
        {
            float totalPayout = 0;
            foreach (Bet bet in currentBets)
            {
                if (System.Array.Exists(bet.numbers, number => number == winningNumber))
                {
                    totalPayout += bet.amount * GetPayoutMultiplier(bet.betType);
                }
            }
            LogManager.Log("Total payout: " + totalPayout,LogManager.LogLevel.Development);
            return totalPayout;
        }

        private float GetPayoutMultiplier(BetType betType)
        {
            switch (betType)
            {
                case BetType.Straight: return 35f;
                case BetType.Split: return 17f;
                case BetType.Street: return 11f;
                case BetType.Corner: return 8f;
                case BetType.SixLine: return 5f;
                case BetType.Red:
                case BetType.Black:
                case BetType.Even:
                case BetType.Odd:
                case BetType.High:
                case BetType.Low: return 1f;
                case BetType.Dozens:
                case BetType.Columns: return 2f;
                default: return 0f;
            }
        }


        
    }
}