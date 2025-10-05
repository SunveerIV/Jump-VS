using UnityEngine;
using Game.Settings;
using Game.Prefabs;
using Game.Behaviours.Managers;


namespace Game.Behaviours.Players {
    public class PlayerSingleplayer : PlayerBase {
        public static PlayerSingleplayer Create(Vector3 position, Quaternion rotation, Level level) {
            PlayerSingleplayer playerSingleplayer = Instantiate(PrefabContainer.PLAYER_SINGLEPLAYER, position, rotation);
            playerSingleplayer.Initialize(level);
            return playerSingleplayer;
        }
    }
}