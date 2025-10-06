using UnityEngine;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {

        private static PrefabContainer instance;

        private void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}