using System;
using NetworkLib.Common.DTOs;

namespace NetworkLib.Common.DTOs.MatchMaking
{
    [Serializable]
    public class MatchMessage : Message
    {
        public string MatchId { get; }

        public MatchMessage(string matchId)
        {
            MatchId = matchId;
        }
    }
}