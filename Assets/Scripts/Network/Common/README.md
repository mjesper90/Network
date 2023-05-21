# The Common folder & DTOs namespace
The Common folder consists of different convient serializeable Data Transfer Objects (DTO)
that both the Server and the Client can translate from binary into Object instances.

It also contains the CONSTANTS.cs file, which contains agreed upon values, like how fast the updates are transfered between Server and Client.

Extend with your own classes, just make sure the are marked as [Serializable]
Make use of the Message and MessageType to store the serialized DTO.