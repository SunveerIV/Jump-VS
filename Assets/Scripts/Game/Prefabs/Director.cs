using UnityEngine;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Director directorPrefab;
        public static Director DIRECTOR => instance.directorPrefab;
    }
}