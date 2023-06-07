using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using MyGame.DTOs;
using NetworkLib.Common;
using NetworkLib.Common.DTOs;
using NetworkLib.GameClient;
using UnityEngine;

namespace MyGame.NetworkSetup
{
    public class ClientInit : MonoBehaviour
    {
        public string IP = "127.0.0.1";
        public int Port = 8052;
        public Client Client;

        public static ClientInit Instance;
        public MessageFactory MsgFactory;

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
            MsgFactory = new MessageFactory(new UnityLogger("MessageFactory::"), new BinaryFormatter());
        }

        public void Send(MessageType msgType, string v)
        {
            Debug.Log("Sending message: " + v);
            Client.Send(MsgFactory.CreateMessage(msgType, v));
        }

        protected void FixedUpdate()
        {
            TryStarting();
        }

        private void TryStarting()
        {
            try
            {
                if (Client == null)
                {
                    Client = new Client(new UnityLogger("Client::"), new TcpClient(IP, Port), MsgFactory);
                    Client.StartReceiving();
                    Debug.Log("Client started");
                    StartCoroutine(SendContinousPlayerData());
                }
            }
            catch (SocketException e)
            {
                Debug.LogWarning("Socket exception: " + e.Message);
                Client = null;
            }
        }

        private IEnumerator SendContinousPlayerData()
        {
            while (true)
            {
                yield return new WaitForSeconds(CONSTANTS.ServerSpeed / 2);
                SendPlayerPositionAndRotation();
            }
        }

        private void SendPlayerPositionAndRotation()
        {
            if (Client?.IsConnected() == true && GameController.Instance.LocalPlayer?.InGame == true)
            {
                try
                {
                    Player p = GameController.Instance.LocalPlayer;
                    Vector3 pos = p.transform.position;
                    Client.Log.LogWarning("Sending player position: " + pos);
                    PlayerPosition playerPos = new PlayerPosition(p.GetUser().Username, pos.x, pos.y, pos.z);
                    _ = Client.SendAsync(new Message(MessageType.PlayerPosition, Client.MsgFactory.Serialize(playerPos)));
                    _ = Client.SendAsync(new Message(MessageType.PlayerRotation, Client.MsgFactory.Serialize(new PlayerRotation(p.GetUser().Username, p.transform.rotation.eulerAngles.y))));
                }
                catch (Exception e)
                {
                    Client.Log.LogError("Error sending player position: " + e.Message);
                }
            }
        }

        public void OnDestroy()
        {
            Client?.Disconnect();
        }

        public void SendLogin(string username, string password)
        {
            Client.Send(new Message(MessageType.Login, Client.MsgFactory.Serialize(new Authentication(username, password))));
        }

        public void SendQueue()
        {
            Client.Send(new Message(MessageType.JoinQueue));
        }
    }
}