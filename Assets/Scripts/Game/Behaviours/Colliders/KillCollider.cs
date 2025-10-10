using UnityEngine;
using Game.Utility;
using Game.Interfaces;

namespace Game.Behaviours.Colliders {
    public class KillCollider : MonoBehaviour {
        
        public static KillCollider Create(KillCollider prefab) {
            var collider = Instantiate(prefab, Camera.main.transform);
            return collider;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (Tools.TryGetInterface(other.gameObject, out IPlayer player)) {
                player.RequestDespawn();
            }
        }
    }
}