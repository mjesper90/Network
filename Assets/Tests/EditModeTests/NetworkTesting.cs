using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using GameClient;
using GameServer;
using DTOs;
using System.Net.Sockets;

public class NetworkTesting
{
    private Server _server;
    private Client _client;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        //Start server
        _server = new Server(8052);
        //Start client
        _client = new Client(new TcpClient("127.0.0.1", 8052));
    }

    // A Test behaves as an ordinary method
    [Test]
    public void Connectivity()
    {
        Assert.IsTrue(_server.Clients.Count == 1);
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
