using Game.Behaviours.Managers;
using UnityEngine;

namespace Game.Prefabs {
    public partial class PrefabContainer : MonoBehaviour {
        [SerializeField] private Level level;
        public static Level LEVEL => instance.level;
    }
}