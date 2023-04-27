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
        public Client Client;
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
            if (Client == null && GameController.Instance.Players.Count == 1)
            {

                LocalPlayer = GameController.Instance.Players[GameController.Instance.Players.Keys.First()];
                Debug.Log("Local player found " + LocalPlayer.name);

                if (LocalPlayer != null && LocalPlayer.GetComponent<Player>() != null && LocalPlayer.GetComponent<Player>().UserInfo != null)
                {
                    Client = new Client(IP, Port);
                    Debug.Log("Client started");
                    Client.Send(LocalPlayer.GetComponent<Player>().UserInfo);
                }
                else
                {
                    Debug.Log("Local player not found");
                }
            }
            else if (Client != null)
            {
                Client.Send(new Position(LocalPlayer.transform.position));
            }
            else
            {
                Debug.Log("Client not connected");
            }
        }
    }
}