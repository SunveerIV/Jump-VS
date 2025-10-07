using Unity.Netcode;
using UnityEngine;

namespace Game.Behaviours.Platforms {
    public class PlatformMultiplayer : PlatformBase {

        public static PlatformMultiplayer Create(PlatformMultiplayer prefab, Vector3 position, int index) {
            PlatformMultiplayer platform = Instantiate(prefab, position, Quaternion.identity);
            platform.Initialize(index);
            platform.GetComponent<NetworkObject>().Spawn();
            return platform;
        }
    }
}