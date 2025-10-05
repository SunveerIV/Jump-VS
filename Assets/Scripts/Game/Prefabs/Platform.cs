using UnityEngine;
using Game.Behaviours.Platforms;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Platform platformPrefab;
        public static Platform PLATFORM => instance.platformPrefab;
    }
}