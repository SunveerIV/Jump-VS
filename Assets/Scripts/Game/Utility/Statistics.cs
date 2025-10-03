using UnityEngine;

namespace Game.Utility {
    public static class Statistics {
        /// <summary>
        /// Has the probability of returning "true" p = probability of the time
        /// </summary>
        /// <param name="probability">Probability from 0 to 1</param>
        /// <returns></returns>
        public static bool Probability(float probability) => Random.value < probability;
    }
}