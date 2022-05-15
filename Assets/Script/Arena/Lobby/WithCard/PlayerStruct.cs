public struct PlayerStruct
{
    public string PlayerName { get; private set; }
    public ulong ClientId { get; private set; }

    public PlayerStruct(string playerName, ulong clientId)
    {
        PlayerName = playerName;
        ClientId = clientId;
    }
}