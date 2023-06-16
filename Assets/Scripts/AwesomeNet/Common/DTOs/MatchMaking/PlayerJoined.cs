using System;
using NetworkLib.Common.DTOs;

namespace NetworkLib.Common.DTOs.MatchMaking
{
    [Serializable]
    public class PlayerJoined : Message
    {
        public string Username { get; }
        public string MatchId { get; }

        public PlayerJoined(string username, string matchId)
        {
            Username = username;
            MatchId = matchId;
        }

        public override string ToString()
        {
            return "PlayerJoined: " + Username + ", " + MatchId;
        }
    }
}