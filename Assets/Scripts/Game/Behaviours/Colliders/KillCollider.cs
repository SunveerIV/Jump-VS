using UnityEngine;
using Unity.Netcode;
using Game.Utility;
using Game.Constants;
using Game.Interfaces;

namespace Game.Behaviours.Colliders {
    public class KillCollider : NetworkBehaviour {
        
        public static KillCollider Create(KillCollider prefab) {
            var collider = Instantiate(prefab);
            return collider;
        }

        private void Update() {
            Raise();
        }

        private void Raise() {
            Vector3 position = transform.position;
            position.y += Time.deltaTime * Level.RAISE_SPEED;
            transform.position = position;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (Tools.TryGetInterface(other.gameObject, out IPlayer player)) {
                player.RequestDespawn();
            }
        }
    }
}