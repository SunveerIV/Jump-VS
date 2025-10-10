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
            #if !UNITY_EDITOR
            
            NetworkManager.Singleton.StartHost();
            
            #else
            Debug.Log("StartHost");
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
            
            int maxConnections = 2;
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            var alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(alloc, "dtls"));
            
            string lobbyCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);
            Debug.Log(lobbyCode);
            NetworkManager.Singleton.StartHost();

            lobbyCodeOutputText.text = "Lobby Code\n" + lobbyCode;
            #endif
        }

        public async void StartClient() {
            #if UNITY_EDITOR
            
            NetworkManager.Singleton.StartClient();

            #else
            
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
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(joinAlloc, "dtls"));

            NetworkManager.Singleton.StartClient();

            #endif
        }
    }
}