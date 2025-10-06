using UnityEngine;
using Game.Interfaces;


namespace Game.Behaviours.Players {
    public class PlayerSingleplayer : PlayerBase {
        public static PlayerSingleplayer Create(PlayerSingleplayer prefab, Vector3 position, Quaternion rotation, ILevel level) {
            PlayerSingleplayer playerSingleplayer = Instantiate(prefab, position, rotation);
            playerSingleplayer.Initialize(level);
            return playerSingleplayer;
        }
    }
}