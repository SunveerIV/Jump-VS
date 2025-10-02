using UnityEngine;

namespace Game.Utility.Prefab {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Platform platformPrefab;
        public static Platform PLATFORM => instance.platformPrefab;
    }
}