using System;
using System.Security.Cryptography.X509Certificates;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting.Dependencies.NCalc;

public class PlayerData : IEquatable<PlayerData> , INetworkSerializable {
    
    public ulong ClientId;
    public FixedString32Bytes PlayerName;

    public int Kills;
    public int Deaths;
    public bool IsOnLine = true;

    
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
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref ClientId);
        //serializer.SerializeValue(ref PlayerName);
        //serializer.SerializeValue(ref Kills);
        //serializer.SerializeValue(ref Deaths);
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out ClientId);
            reader.ReadValueSafe(out PlayerName);
            reader.ReadValueSafe(out Kills);
            reader.ReadValueSafe(out Deaths);
        }
        else
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(ClientId);
            writer.WriteValueSafe(PlayerName);
            writer.WriteValueSafe(Kills);
            writer.WriteValueSafe(Deaths);
        }
    }
    public SerializablePlayerData ToSerializablePlayerData() => new SerializablePlayerData(this);
}