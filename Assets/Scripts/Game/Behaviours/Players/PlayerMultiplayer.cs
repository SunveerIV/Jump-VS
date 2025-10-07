using UnityEngine;
using Unity.Netcode;

namespace Game.Behaviours.Players {
    public class PlayerMultiplayer : NetworkBehaviour {

        public static PlayerMultiplayer Create(PlayerMultiplayer prefab, Vector3 position, ulong clientID) {
            PlayerMultiplayer player = Instantiate(prefab, position, Quaternion.identity);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, true);
            return player;
        }
        
    }
}