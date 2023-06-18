using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Networker.ServerSide;

public class ClientIdentifier
{
    public string Name { get; init; }
    public Guid ID { get; init; }
    public bool IsValidID { get => ID != Guid.Empty; }

    public ClientIdentifier(string Name, Guid ID)
    {
        this.Name = Name;
        this.ID = ID;
    }
}
