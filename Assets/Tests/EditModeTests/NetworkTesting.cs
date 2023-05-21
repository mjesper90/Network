using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;
using NetworkLib.GameClient;
using NetworkLib.GameServer;
using System.Net.Sockets;
using System.Threading;
using NetworkLib;

public class NetworkTesting
{
    private Server _server;
    private Client _client;

    private Thread _serverThread;
    private Thread _clientThread;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Start server
        _serverThread = new Thread(() => _server = new Server(8052));
        _serverThread.Start();

        // Start client
        _clientThread = new Thread(() => _client = new Client(new TcpClient("127.0.0.1", 8052)));
        _clientThread.Start();
    }

    // Testing the threads are running, the server is listening and the client is connected
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

        _client.Send(new Message(MessageType.Message, _client.Serialize("Hello test"), ""));

        // Wait for the message to be sent
        Thread.Sleep(100);
        Assert.IsTrue(_server.Clients[0].NetworkHandler.MessageQueue.Count == 1);

        Message message = _server.Clients[0].NetworkHandler.MessageQueue.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_server.Clients[0].NetworkHandler.MessageQueue.Count == 0);
    }

    // Testing async client send
    [Test]
    public void ClientSendAsync()
    {
        //Send async
        _ = _client.SendAsync(new Message(MessageType.Message, _client.Serialize("Hello test"), ""));
        Thread.Sleep(100);
        Assert.IsTrue(_server.Clients[0].NetworkHandler.MessageQueue.Count == 1);

        Message message = _server.Clients[0].NetworkHandler.MessageQueue.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_server.Clients[0].NetworkHandler.MessageQueue.Count == 0);
    }

    // Testing the server can send a message to the client
    [Test]
    public void ServerSend()
    {
        _clientThread.Join();
        Assert.IsTrue(_client.IsConnected());

        _serverThread.Join();
        Assert.IsTrue(_server.Clients.Count == 1);

        _server.Clients[0].Send(new Message(MessageType.Message, _client.Serialize("Hello test"), ""));

        // Wait for the message to be sent
        Thread.Sleep(100);

        Assert.IsTrue(_client.NetworkHandler.MessageQueue.Count == 1);

        Message message = _client.NetworkHandler.MessageQueue.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_client.NetworkHandler.MessageQueue.Count == 0);
    }

    // Testing async server send
    [Test]
    public void ServerSendAsync()
    {
        //Send async
        _ = _server.Clients[0].SendAsync(new Message(MessageType.Message, _client.Serialize("Hello test"), ""));
        Thread.Sleep(100);
        Assert.IsTrue(_client.NetworkHandler.MessageQueue.Count == 1);
        Message message = _client.NetworkHandler.MessageQueue.TryDequeue(out message) ? message : null;
        Assert.IsTrue(message != null);
        Assert.IsTrue(message.MsgType == MessageType.Message);
        Assert.IsTrue(_client.Deserialize<string>(message.Data) == "Hello test");
        Assert.IsTrue(_client.NetworkHandler.MessageQueue.Count == 0);
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
