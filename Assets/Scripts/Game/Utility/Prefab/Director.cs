using UnityEngine;

namespace Game.Utility.Prefab {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Director directorPrefab;
        public static Director DIRECTOR => instance.directorPrefab;
    }
}