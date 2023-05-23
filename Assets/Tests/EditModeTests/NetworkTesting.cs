using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using NetworkLib.GameClient;
using NetworkLib.GameServer;
using System.Net.Sockets;
using System.Threading;
using NetworkLib.Common.Logger;
using NetworkLib.Common.DTOs;

public class NetworkTesting
{
    private Server _server;
    private Client _client;
    private Client _client2;

    private Thread _serverThread;
    private Thread _clientThread;
    private Thread _clientThread2;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Start server
        _serverThread = new Thread(() => _server = new Server(8052));
        _serverThread.Start();

        // Wait for the server to start
        Thread.Sleep(100);

        // Start clients
        _clientThread = new Thread(() => _client = new Client(new DefaultLogger(), new TcpClient("127.0.0.1", 8052)));
        _clientThread.Start();

        //For testing multiple clients
        _clientThread2 = new Thread(() => _client2 = new Client(new DefaultLogger(), new TcpClient("127.0.0.1", 8052)));

        // Wait for the client to connect
        Thread.Sleep(100);

        //Assert
        Assert.IsTrue(_server != null);
        Assert.IsTrue(_client != null);
        Assert.IsTrue(_server.Clients.Count == 1);
        Assert.IsTrue(_client.IsConnected());
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 0);
    }

    // Testing the client is connected and the server has exactly one client
    [Test]
    public void Connectivity()
    {
        _clientThread.Join();
        Assert.IsTrue(_client.IsConnected());

        _serverThread.Join();
        Assert.IsTrue(_server.Clients.Count == 1);
    }

    // Testing the client can send a message to the server
    [Test]
    public void ClientSend()
    {
        _clientThread.Join();
        Assert.IsTrue(_client.IsConnected());

        _serverThread.Join();
        Assert.IsTrue(_server.Clients.Count == 1);

        _client.Send(new Message(MessageType.Message, _client.Serialize("Hello test")));

        // Wait for the message to be sent
        Thread.Sleep(100);

        Assert.IsTrue(_server.Clients[0].NetworkHandler.GetQueueSize() == 1);

        Message message = _server.Clients[0].NetworkHandler.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_server.Clients[0].NetworkHandler.GetQueueSize() == 0);
    }

    // Testing async client send
    [Test]
    public void ClientSendAsync()
    {
        _clientThread.Join();
        Assert.IsTrue(_client.IsConnected());

        _serverThread.Join();
        Assert.IsTrue(_server.Clients.Count == 1);

        _ = _client.SendAsync(new Message(MessageType.Message, _client.Serialize("Hello test")));

        // Wait for the message to be sent
        Thread.Sleep(100);

        Assert.IsTrue(_server.Clients[0].NetworkHandler.GetQueueSize() == 1);

        Message message = _server.Clients[0].NetworkHandler.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_server.Clients[0].NetworkHandler.GetQueueSize() == 0);
    }

    // Testing the server can send a message to the client
    [Test]
    public void ServerSend()
    {
        _server.Clients[0].Send(new Message(MessageType.Message, _client.Serialize("Hello test")));

        // Wait for the message to be sent
        Thread.Sleep(100);

        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 1);

        Message message = _client.NetworkHandler.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 0);
    }

    // Testing async server send
    [Test]
    public void ServerSendAsync()
    {
        //Send async
        _ = _server.Clients[0].SendAsync(new Message(MessageType.Message, _client.Serialize("Hello test")));
        Thread.Sleep(100);
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 1);
        Message message = _client.NetworkHandler.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_client.NetworkHandler.GetQueueSize() == 0);
    }

    // Testing multiple clients
    [Test]
    public void MultipleClients()
    {
        // Start client
        _clientThread2.Start();

        // Wait for the client to connect
        Thread.Sleep(100);

        //Assert
        Assert.IsTrue(_server.Clients.Count == 2);
        Assert.IsTrue(_client2.IsConnected());

        // Disconnect client
        _client2.Disconnect();

        // Wait for the client to disconnect
        Thread.Sleep(100);

        //Update server state
        _server.UpdateServer();

        //Assert
        Assert.IsTrue(_server.Clients.Count == 1);
        Assert.IsTrue(!_client2.IsConnected());
    }

    //Cleanup
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Shutdown();
        _serverThread.Join();
        _client.Disconnect();
        _clientThread.Join();
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
