using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Authentication;
using Unity.Services.Core.Environments;
using UnityEngine.Rendering.UI;

namespace Game.UI {
    public class StartNetwork : MonoBehaviour{
        
        [SerializeField] private TMP_InputField inputField;
        
        public async void StartHost() {
            #if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            
            NetworkManager.Singleton.StartHost();
            
            #else
            
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
            
            int maxConnections = 2;
            if (!AuthenticationService.Instance.IsSignedIn)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            var alloc = await RelayService.Instance.CreateAllocationAsync(maxConnections);

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(alloc, "dtls"));
            
            Debug.Log(await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId));
            NetworkManager.Singleton.StartHost();

            #endif
        }

        public async void StartClient() {
            #if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            
            NetworkManager.Singleton.StartClient();

            #else
            
            Debug.Log("Attempting to start client connection");
            await UnityServices.InitializeAsync(new InitializationOptions().SetEnvironmentName("production"));
            
            string joinCode = inputField.text;
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