using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


public   class NetWorkManagerPlayerData :NetworkBehaviour
{
    public static NetWorkManagerPlayerData Instance;
    public CinemachineVirtualCamera VirtualCamera;
    
    [SerializeField]private Transform[] _spawnPoints;
    [SerializeField]private PlayerController _prefabsPlayerController;
    private Dictionary<ulong, NetPlayer> _netPlayers= new Dictionary<ulong, NetPlayer>();
    private  List<PlayerData> _playerData= new List<PlayerData>();
    

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        NetworkManager.Singleton.OnConnectionEvent += ConnectionEvent;
    }

    private void ConnectionEvent(NetworkManager arg1, ConnectionEventData arg2) {
        if( !PlayerDataAlreadyExist(arg2.ClientId)) _playerData.Add(new PlayerData(arg2.ClientId));
    }

    private bool PlayerDataAlreadyExist(ulong id)
    {
        return _playerData.Any(playerData => playerData.ClientId == id);
    }

    
    public void AddNetPlayer(ulong clientId, NetPlayer netPlayer) {
        _netPlayers.Add(clientId, netPlayer);
    }

    [ServerRpc]
    public void SubmiteServerPlayerNameServerRpc(ulong clientId , FixedString32Bytes playerName) {
        int iteration=0;
        foreach (var playerData in _playerData) {
            iteration++;
            if (playerData.ClientId == clientId) {
                playerData.ChangePlayerName(playerName);
                Debug.Log($"PlayerName: {playerName} pour clientId: {clientId} pour l'itération {iteration}");
                SpawnPlayerController(clientId);
            }
        }
    }

    public void SpawnPlayerController(ulong playerId) {
        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        PlayerController playerController = Instantiate(_prefabsPlayerController, spawnPoint.position, Quaternion.identity);
        playerController.GetComponent<NetworkObject>().Spawn();
        playerController.GetComponent<NetworkObject>().ChangeOwnership(playerId);
        
       //PlayerController player = Instantiate(_prefabsPlayerController, spawnPoint, Quaternion.identity);
    }

    public void OnPlayerKill(ulong killerId, ulong deadId) {
        _playerData.First(p=>p.ClientId == killerId).Addkill();
        _playerData.First(p=>p.ClientId == deadId).Addkill();
        
        _netPlayers[deadId].SetInDeathMode();
    }
}