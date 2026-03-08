using System.Linq;
using UnityEngine;

namespace Game.Utility {
    public static class Tools {
        public static bool TryGetInterface<T>(this GameObject obj, out T result) {
            result = obj.GetComponents<MonoBehaviour>().OfType<T>().FirstOrDefault();
            return result != null;
        }

        public static bool TryGetInterface<T>(this Collision2D obj, out T result) {
            return obj.gameObject.TryGetInterface(out result);
        }
    }
}