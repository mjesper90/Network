using System;
using System.Collections;
using System.Net.Sockets;
using MyGame.DTOs;
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
        }

        public void Send(object obj)
        {
            Client.Send(obj);
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
                    Client = new Client(new UnityLogger("Client::"), new TcpClient(IP, Port));
                    _ = Client.StartReceiving();
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
                SendPlayerPosition();
            }
        }

        private void SendPlayerPosition()
        {
            if (Client?.IsConnected() == true && GameController.Instance.LocalPlayer?.InGame == true)
            {
                Player p = GameController.Instance.LocalPlayer;
                Vector3 pos = p.transform.position;
                PlayerPosition player = new PlayerPosition(p.GetUser().Username, pos.x, pos.y, pos.z);
                _ = Client.SendAsync(new Message(MessageType.PlayerPosition, Client.Serialize(player)));
                _ = Client.SendAsync(new Message(MessageType.PlayerRotation, Client.Serialize(new PlayerRotation(p.GetUser().Username, p.transform.rotation.eulerAngles.y))));
            }
        }

        public void OnDestroy()
        {
            Client?.Disconnect();
        }

        public void SendLogin(string username, string password)
        {
            Client.Send(new Message(MessageType.Login, Client.Serialize(new Authentication(username, password))));
        }

        public void SendQueue()
        {
            Client.Send(new Message(MessageType.JoinQueue));
        }
    }
}