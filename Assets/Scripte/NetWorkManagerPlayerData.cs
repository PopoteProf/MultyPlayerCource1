using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;


public class NetWorkManagerPlayerData :NetworkBehaviour
{
    public static NetWorkManagerPlayerData Instance;
    public CinemachineVirtualCamera VirtualCamera;
    
    [SerializeField]private Transform[] _spawnPoints;
    [SerializeField]private PlayerController _prefabsPlayerController;
    private Dictionary<ulong, NetPlayer> _netPlayers= new Dictionary<ulong, NetPlayer>();
    public  List<PlayerData> _playerData = new List<PlayerData>();
    

    private void Awake() {
        Instance = this;
        //playerData= new NetworkList<PlayerData>();
    }

    private void Start() {
        NetworkManager.Singleton.OnConnectionEvent += ConnectionEvent;
        NetworkManager.Singleton.OnClientDisconnectCallback+= SingletonOnOnClientDisconnectCallback;
    }

    private void SingletonOnOnClientDisconnectCallback(ulong id) {
        _netPlayers.Remove(id);
        _playerData.FirstOrDefault(d => d.ClientId == id).IsOnLine=false;
        UpdatePlayerData();
    }

    private void ConnectionEvent(NetworkManager arg1, ConnectionEventData arg2) {
        Debug.Log("Do Connection Event"+ arg2.ClientId);
        if(IsServer) if( !PlayerDataAlreadyExist(arg2.ClientId)) _playerData.Add(new PlayerData(arg2.ClientId));
    }

    private bool PlayerDataAlreadyExist(ulong id)
    {
        foreach (var playerData in _playerData)
        {
            if (playerData.ClientId == id) return true;
        }

        return false;
        //return _playerData.Any(playerData => playerData.ClientId == id);
    }

    
    public void AddNetPlayer(ulong clientId, NetPlayer netPlayer) {
        _netPlayers.Add(clientId, netPlayer);
    }

    [ServerRpc]
    public void SubmiteServerPlayerNameServerRpc(ulong clientId , FixedString32Bytes playerName) {
        int iteration=0;
        foreach (var playerData in _playerData) {
            iteration++;
            if (playerData.ClientId == clientId)
            {
                playerData.PlayerName = playerName;
                UpdatePlayerData();
                //PlayerData data = playerData;
                //data.PlayerName = playerName.Value;
                //playerData.ChangePlayerName(playerName);
                //playerData.PlayerName = playerName.Value;
                Debug.Log($"PlayerName: {playerName} pour clientId: {clientId} pour l'itération {iteration}");
                DebugPlayerData();
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
        foreach (var playerData in _playerData) if (playerData.ClientId == killerId) playerData.Addkill();
        foreach (var playerData in _playerData) if (playerData.ClientId == deadId) playerData.AddDeath();
        
        //_playerData.First(p=>p.ClientId == killerId).Addkill();
        //_playerData.First(p=>p.ClientId == deadId).Addkill();

        UpdatePlayerData();

        DebugPlayerData();
        _netPlayers[deadId].SetInDeathModeClientRpc();
    }

    public void UpdatePlayerData()
    {
        List<SerializablePlayerData> datas = new List<SerializablePlayerData>();
        foreach (var data in _playerData)
        {
            if (!data.IsOnLine) continue;
            datas.Add(data.ToSerializablePlayerData());
        }
        //for (int i = 0; i < _playerData.Count; i++) {
        //     datas[i] = _playerData[i].ToSerializablePlayerData();
        //}

        foreach (var netPlayer in _netPlayers.Values) {
            netPlayer.UpdatePlayerDataClientRpc(datas.ToArray());
        }
        
    }

    private void DebugPlayerData()
    {
        Debug.Log("Player data ");
        foreach (var playerdata in _playerData) {
            Debug.Log( playerdata.ClientId+ "   "+playerdata.PlayerName+"     "+playerdata.Kills+ "    "+playerdata.Deaths);
        }
    }
}