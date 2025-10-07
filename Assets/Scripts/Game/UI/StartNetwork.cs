using UnityEngine;
using Unity.Netcode;

namespace Game.UI {
    public class StartNetwork : MonoBehaviour{
        public static void StartHost() {
            NetworkManager.Singleton.StartHost();
        }
    
        public static void StartServer() {
            NetworkManager.Singleton.StartServer();
        }

        public static void StartClient() {
            NetworkManager.Singleton.StartClient();
        }
    }
}