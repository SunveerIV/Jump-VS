using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Core.Environments;

namespace Game.UI {
    public class StartNetwork : MonoBehaviour{
        
        [SerializeField] private TMP_InputField lobbyCodeInputField;
        [SerializeField] private TMP_Text lobbyCodeOutputText;
        
        public async void StartHost() {
            NetworkManager networkManager = NetworkManager.Singleton;
            UnityTransport unityTransport = networkManager.GetComponent<UnityTransport>();
            
            if (unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport) {
                networkManager.StartHost();
                return;
            }
            
            Debug.Log("StartHost");
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
            
            int maxConnections = 2;
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            var alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            unityTransport.SetRelayServerData(AllocationUtils.ToRelayServerData(alloc, "dtls"));
            
            string lobbyCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
            Debug.Log(lobbyCode);
            networkManager.StartHost();

            lobbyCodeOutputText.text = "Lobby Code\n" + lobbyCode;
        }

        public async void StartClient() {
            NetworkManager networkManager = NetworkManager.Singleton;
            UnityTransport unityTransport = networkManager.GetComponent<UnityTransport>();
            
            if (unityTransport.Protocol == UnityTransport.ProtocolType.UnityTransport) {
                networkManager.StartClient();
                return;
            }
            
            Debug.Log("Attempting to start client connection");
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
            
            string joinCode = lobbyCodeInputField.text;
            Debug.Log("Joining code: " + joinCode);
            Debug.Log("Async initialized");
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in");
            var joinAlloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Joined code");
            unityTransport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAlloc, "dtls"));

            networkManager.StartClient();
        }
    }
}