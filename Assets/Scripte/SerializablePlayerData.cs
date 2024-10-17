using Unity.Collections;
using Unity.Netcode;

public struct SerializablePlayerData : INetworkSerializable
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;

    public int Kills;
    public int Deaths;

    public SerializablePlayerData(ulong clientId, FixedString32Bytes playerName, int kills, int deaths) {
        ClientId = clientId;
        PlayerName = playerName;
        Kills = kills;
        Deaths = deaths;
    }
    public SerializablePlayerData(PlayerData data) {
        ClientId = data.ClientId;
        PlayerName = data.PlayerName;
        Kills = data.Kills;
        Deaths = data.Deaths;
    }
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
}