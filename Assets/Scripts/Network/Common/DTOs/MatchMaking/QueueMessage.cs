using System;
using NetworkLib.Common.DTOs;

namespace NetworkLib.Common.DTOs.MatchMaking
{
    [Serializable]
    public class QueueMessage : Message
    {
        public string MatchId { get; }

        public QueueMessage(string matchId)
        {
            MatchId = matchId;
        }

        public QueueMessage()
        {

        }

        public override string ToString()
        {
            return "QueueMessage: " + MatchId;
        }
    }
}