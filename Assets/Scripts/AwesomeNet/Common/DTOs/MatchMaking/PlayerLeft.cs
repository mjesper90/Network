using System;
using NetworkLib.Common.DTOs;

namespace NetworkLib.Common.DTOs.MatchMaking
{
    [Serializable]
    public class PlayerLeft : Message
    {
        public string Username { get; }
        public string MatchId { get; }

        public PlayerLeft(string username, string matchId)
        {
            Username = username;
            MatchId = matchId;
        }

        public override string ToString()
        {
            return "PlayerLeft: " + Username + ", " + MatchId;
        }
    }
}