using UnityEngine;

namespace Game.Behaviours.Platforms {
    public class PlatformSingleplayer : PlatformBase {
        public static PlatformSingleplayer Create(PlatformSingleplayer prefab, Vector3 position, Quaternion rotation, int index) {
            PlatformSingleplayer platform = Instantiate(prefab, position, rotation);
            platform.Initialize(index);
            return platform;
        }
    }
}