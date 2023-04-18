using System.Linq;
using System.Net.Sockets;
using DTOs;
using UnityEngine;

namespace GameClient
{
    public class ClientInit : MonoBehaviour
    {
        public string IP = "127.0.0.1";
        public int Port = 8052;
        private Client _client;
        public GameObject LocalPlayer;

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


        protected void FixedUpdate()
        {
            if (_client == null && GameController.Instance.Players.Count == 1)
            {
                LocalPlayer = GameController.Instance.Players.First().Value;
                _client = new Client(IP, Port);
                Debug.Log("Client started");
                _client.Send(LocalPlayer.GetComponent<Player>().GetUser());
            }
            else if (_client != null && LocalPlayer != null)
            {
                _client.Send(new Position(LocalPlayer.transform.position));
            }
            else
            {
                Debug.Log("Client not connected");
            }
        }
    }
}