using UnityEngine;
using Game.Prefabs;
using Game.Utility;

namespace Game.Behaviours.Platforms {
    public class PlatformSingleplayer : PlatformBase {
        public static PlatformSingleplayer Create(Vector3 position, Quaternion rotation, int index) {
            PlatformSingleplayer platform = Instantiate(PrefabContainer.PLATFORM_SINGLEPLAYER, position, rotation);
            platform.Initialize(index);
            return platform;
        }
    }
}