using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using NetworkLib.GameServer;
using NetworkLib.Common;
using System.Runtime.Serialization.Formatters.Binary;

public class NetworkTesting
{
    private Client _client;
    private Client _client2;
    private Server _server;
    private readonly string _username1 = "TestUser1";
    private readonly string _username2 = "TestUser2";

    private MessageFactory _mf;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Start server
        _server = new Server(new TestLogger(), 8052, new Match());
        _server.StartAcceptingClients();
        Thread.Sleep(100);
        _mf = new MessageFactory(new TestLogger(), new BinaryFormatter());

        // Start Clients
        _client = new Client(new TestLogger(), new TcpClient("127.0.0.1", 8052), _mf);
        _client2 = new Client(new TestLogger(), new TcpClient("127.0.0.1", 8052), _mf);
        Task.Run(_client.StartReceiving);
        Task.Run(_client2.StartReceiving);
        Thread.Sleep(100);

        //Assert
        Assert.IsTrue(_server != null);
        Assert.IsTrue(_client != null);
        Assert.IsTrue(_client2 != null);
        Assert.IsTrue(_server.GetClients().Length == 2);
        Assert.IsTrue(_client.IsConnected());
        Assert.IsTrue(_client2.IsConnected());
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 0);
        Assert.IsTrue(_client2.NetworkHandler.GetQueueSize() == 0);

        // Login
        _client.NetworkHandler.Auth = new Authentication(_username1, "password");
        _client2.NetworkHandler.Auth = new Authentication(_username2, "password");
        _client.Send(new Message(MessageType.Login, _client.MsgFactory.Serialize(_client.NetworkHandler.Auth)));
        _client2.Send(new Message(MessageType.Login, _client2.MsgFactory.Serialize(_client2.NetworkHandler.Auth)));
        Thread.Sleep(200);

        //Assert
        Assert.IsTrue(FindClientAtServer(_client).NetworkHandler.GetQueueSize() == 0);
        Assert.IsTrue(FindClientAtServer(_client2).NetworkHandler.GetQueueSize() == 0);
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 1);
        Assert.IsTrue(_client2.NetworkHandler.GetQueueSize() == 1);

        //Check login response
        Message message = _client.NetworkHandler.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.LoginResponse);

        message = _client2.NetworkHandler.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.LoginResponse);
    }

    //Cleanup
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _client.Disconnect();
        _client2.Disconnect();
        _server.Shutdown();
    }

    [Test]
    public void TestBroadcastMatch()
    {
        Assert.IsTrue(_server.GetClients().Length == 2);
    }

    [Test]
    public void TestJoinQueue()
    {
        // Join queue
        _client.Send(new Message(MessageType.JoinQueue));
        _client2.Send(new Message(MessageType.JoinQueue));

        Assert.IsTrue(FindClientAtServer(_client).NetworkHandler.GetQueueSize() == 0);
        Assert.IsTrue(FindClientAtServer(_client2).NetworkHandler.GetQueueSize() == 0);
        int timeout = 0;
        while (_client.NetworkHandler.GetQueueSize() == 0 && _client2.NetworkHandler.GetQueueSize() == 0 && timeout < 10)
        {
            Thread.Sleep(100);
            timeout++;
        }
        Assert.IsTrue(timeout < 10);
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() > 0);
        Assert.IsTrue(_client2.NetworkHandler.GetQueueSize() > 0);
    }

    [Test]
    public void TestLeaveQueue()
    {

    }

    private Client FindClientAtServer(Client client)
    {
        foreach (Client c in _server.GetClients())
        {
            if (c.NetworkHandler.Auth.Username == client.NetworkHandler.Auth.Username)
            {
                return c;
            }
        }
        return null;
    }
}