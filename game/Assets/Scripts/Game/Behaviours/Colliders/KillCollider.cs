using UnityEngine;
using Unity.Netcode;
using Game.Utility;
using Game.Constants;
using Game.Interfaces;

namespace Game.Behaviours.Colliders {
    public class KillCollider : NetworkBehaviour, IKillCollider {
        
        public static KillCollider Create(KillCollider prefab, bool multiplayer = false) {
            var collider = Instantiate(prefab);
            if (multiplayer) collider.NetworkObject.Spawn();
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
    }
}