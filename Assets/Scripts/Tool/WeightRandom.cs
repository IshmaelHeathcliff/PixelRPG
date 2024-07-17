using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace QFramework.Tool
{
    public static class WeightRandom<T>
    {
        public static T GetRandom(Dictionary<T, int> itemWeights)
        {
            var items = itemWeights.Keys.ToList();
            var weights = itemWeights.Values.ToList();
            int totalWeight = weights.Sum();
            
            var random = new Random();
            int randomValue = random.Next(0, totalWeight);

            var i = 0;
            while (true)
            {
                if (randomValue < weights[i])
                {
                    return items[i];
                }

                randomValue -= weights[i];
                i++;

                if (i >= items.Count)
                {
                    break;
                }
            }

            return default(T);
        }
        
    }
}