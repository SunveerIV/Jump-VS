using System;
using System.Collections;
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

        public static void ExecuteAtEndOfFrame(this MonoBehaviour obj, Action action) {
            obj.StartCoroutine(ExecuteAtEndOfFrame(action));
        }

        private static IEnumerator ExecuteAtEndOfFrame(Action action) {
            yield return new WaitForEndOfFrame();
            action();
        }
    }
}