using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetPlayer : NetworkBehaviour
{

    [SerializeField] private float _deathTime = 5;

    private bool _isDead;
    private float _timer;
    private void Start() {
        if (!IsOwner) return;
        UIManager.Instance.UIPlayerName.gameObject.SetActive(true);
        UIManager.Instance.UIPlayerName.OnPlayerValidateName += SubmitePlayerName;
    }

    private void Update()
    {
        ManageDeathCountDown();
    }

    private void ManageDeathCountDown() {
        if (!_isDead) return;
        _timer += Time.deltaTime;
        UIManager.Instance.UIDeathCountDown.SetNewText(Mathf.RoundToInt(_deathTime-_timer).ToString());
        if (_timer >= _deathTime) {
            _isDead = false;
            UIManager.Instance.UIDeathCountDown.gameObject.SetActive(false);
            AskForRespawnServerRpc();
        }
    }


    public void SubmitePlayerName(object sender, FixedString32Bytes playername) {
        AddNetPlayerServerRpc(OwnerClientId);
        SubmitePlayerNameServerRpc(OwnerClientId, playername);
    }
    
    [ServerRpc]
    public void SubmitePlayerNameServerRpc(ulong id , FixedString32Bytes playername) {
            if(IsServer) NetWorkManagerPlayerData.Instance.SubmiteServerPlayerNameServerRpc(id, playername);
    }
    [ServerRpc]
    public void AddNetPlayerServerRpc(ulong id ) {
        if(IsServer) NetWorkManagerPlayerData.Instance.AddNetPlayer(id, this);
    }

    public void SetInDeathMode() {
        if(!IsOwner)return;
        _isDead = true;
        _timer = 0;
    }

    [ServerRpc]
    private void AskForRespawnServerRpc() {
        UIManager.Instance.UIDeathCountDown.gameObject.SetActive(true);
        NetWorkManagerPlayerData.Instance.SpawnPlayerController(OwnerClientId);
    }
}