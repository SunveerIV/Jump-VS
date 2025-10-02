using UnityEngine;

namespace Game.Utility.Prefab {
    public partial class PrefabContainer : MonoBehaviour {

        private static PrefabContainer instance;

        private void Awake() {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}