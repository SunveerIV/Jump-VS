using Game.Behaviours.Managers;
using UnityEngine;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private LevelSingleplayer level;
        public static LevelSingleplayer LEVEL_SINGLEPLAYER => instance.level;
    }
}