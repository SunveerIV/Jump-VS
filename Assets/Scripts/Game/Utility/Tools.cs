using System.Linq;
using UnityEngine;

namespace Game.Utility {
    public static class Tools {
        public static bool TryGetInterface<T>(GameObject obj, out T result) {
            result = obj.GetComponents<MonoBehaviour>().OfType<T>().FirstOrDefault();
            return result != null;
        }
    }
}