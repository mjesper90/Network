using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Networker.ServerSide;

public class ClientIdentifier
{
    public string Name { get; init; }
    public uint ID { get; init; }
    public bool IsValidID { get => ID != 0; }

    public ClientIdentifier(string Name, uint ID)
    {
        this.Name = Name;
        this.ID = ID;
    }
}
