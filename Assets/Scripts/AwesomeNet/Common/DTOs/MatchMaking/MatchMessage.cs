using System;
using NetworkLib.Common.DTOs;

namespace NetworkLib.Common.DTOs.MatchMaking
{
    [Serializable]
    public class MatchMessage : Message
    {
        public string MatchId { get; }
        public string[] Usernames { get; set; }

        public MatchMessage(string matchId, string[] usernames)
        {
            MatchId = matchId;
            Usernames = usernames;
        }
    }
}