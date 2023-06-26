using System;
using System.Collections;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using AmongUs.DTOs;
using AmongUs.GameControl;
using AmongUs.Logging;
using NetworkLib.Common;
using NetworkLib.Common.DTOs;
using NetworkLib.Common.DTOs.MatchMaking;
using NetworkLib.GameClient;
using UnityEngine;

namespace AmongUs.NetworkSetup
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

        public void ConnectClient()
        {
            try
            {
                if (Client == null)
                {
                    Client = new Client(new UnityLogger("Client::"), new TcpClient(IP, Port), MsgFactory);
                    Client.StartReceiving();
                    SendLogin(GameController.Instance.LocalPlayer.Username, "password");
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
            if (Client?.IsConnected() == true && GameController.Instance.LocalPlayer.MatchId != "")
            {
                try
                {
                    Player p = GameController.Instance.LocalPlayer;
                    Vector3 pos = p.transform.position;
                    _ = Client.SendAsync(new PositionAndYRotation(p.Username, pos.x, pos.y, pos.z, p.transform.rotation.eulerAngles.y));
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
            _ = Client.SendAsync(new Authentication(username, password));
        }

        public void SendQueue()
        {
            _ = Client.SendAsync(new QueueMessage());
        }
    }
}