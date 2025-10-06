using UnityEngine;
using Game.Behaviours.Platforms;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private PlatformSingleplayer platformPrefab;
        public static PlatformSingleplayer PLATFORM_SINGLEPLAYER => instance.platformPrefab;
    }
}