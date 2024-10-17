using System;
using Unity.Collections;

public struct PlayerData : IEquatable<PlayerData> {
    
    public ulong ClientId;
    public FixedString32Bytes PlayerName;

    public int Kills;
    public int Deaths;

    
    public PlayerData(ulong id) {
        ClientId = id;
        PlayerName = "Player";
        Kills = 0;
        Deaths=0;
    }
    public bool Equals(PlayerData other) {
        return ClientId == other.ClientId;
    }

    public void ChangePlayerName(FixedString32Bytes playerName) {
        PlayerName = playerName;
    }
    public void Addkill() => Kills++;
    public void AddDeath()=> Deaths++;
}