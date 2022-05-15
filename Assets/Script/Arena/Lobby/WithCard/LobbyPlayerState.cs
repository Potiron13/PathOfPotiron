using System;
using Unity.Collections;
using Unity.Netcode;

    public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>
    {
        public ulong ClientId;
        public FixedString32Bytes PlayerName;
        public FixedString32Bytes CharacterName;
        public bool IsReady;

        public LobbyPlayerState(ulong clientId, FixedString32Bytes playerName, bool isReady, FixedString32Bytes characterName)
        {
            ClientId = clientId;
            PlayerName = playerName;
            IsReady = isReady;
            CharacterName = characterName;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref PlayerName);
            serializer.SerializeValue(ref IsReady);
            serializer.SerializeValue(ref CharacterName);
        }

        public bool Equals(LobbyPlayerState other)
        {
            return ClientId == other.ClientId &&
                PlayerName.Equals(other.PlayerName) &&
                IsReady == other.IsReady &&
                CharacterName == other.CharacterName;
        }
    }

