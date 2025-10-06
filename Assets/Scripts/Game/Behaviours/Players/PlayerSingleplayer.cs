using UnityEngine;
using Game.Prefabs;
using Game.Interfaces;


namespace Game.Behaviours.Players {
    public class PlayerSingleplayer : PlayerBase {
        public static PlayerSingleplayer Create(Vector3 position, Quaternion rotation, ILevel level) {
            PlayerSingleplayer playerSingleplayer = Instantiate(PrefabContainer.PLAYER_SINGLEPLAYER, position, rotation);
            playerSingleplayer.Initialize(level);
            return playerSingleplayer;
        }
    }
}