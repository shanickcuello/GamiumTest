using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Spawner
{
        public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
        {
            [SerializeField] private TMP_InputField roomNameInputField;
            [SerializeField] private Button startServerOrClientButton;
            private NetworkRunner _runner;
            private void Start()
            {
                startServerOrClientButton.onClick.AddListener(()=> StartGame(roomNameInputField.text));
            }

            [SerializeField] private NetworkPrefabRef _playerPrefab;
            private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

            async void StartGame(string roomName)
            {
                _runner = gameObject.AddComponent<NetworkRunner>();
                _runner.ProvideInput = true;
                await _runner.StartGame(new StartGameArgs()
                {
                    GameMode = GameMode.AutoHostOrClient,
                    SessionName = roomName,
                    Scene = SceneManager.GetActiveScene().buildIndex,
                    SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
                });}
            
            public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
            {
                Debug.Log("Paso por aca");
                if (runner.IsServer)
                {
                    // Create a unique position for the player
                    Vector3 spawnPosition = new Vector3(0,0,0);
                    NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
                    // Keep track of the player avatars so we can remove it when they disconnect
                    _spawnedCharacters.Add(player, networkPlayerObject);
                }
            }

            public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
            {
                // Find and remove the players avatar
                if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
                {
                    runner.Despawn(networkObject);
                    _spawnedCharacters.Remove(player);
                }
            }
            
            public void OnInput(NetworkRunner runner, NetworkInput input) { }
            public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
            public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
            public void OnConnectedToServer(NetworkRunner runner) { }
            public void OnDisconnectedFromServer(NetworkRunner runner) { }
            public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
            public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
            public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
            public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
            public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
            public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
            public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
            public void OnSceneLoadDone(NetworkRunner runner) { }
            public void OnSceneLoadStart(NetworkRunner runner) { }
        }
}