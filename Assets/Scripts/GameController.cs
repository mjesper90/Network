using System;
using System.Collections.Generic;
using DTOs;
using GameClient;
using GameServer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private Server _server;
    private Client _client;
    public Dictionary<string, GameObject> Players = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();

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

    public void SetServer(Server server)
    {
        _server = server;
    }

    public void SetClient(Client client)
    {
        _client = client;
    }

    public Server GetServer()
    {
        if (_server == null)
        {
            Debug.Log("Server is null");
            return null;
        }
        return _server;
    }

    public Client GetClient()
    {
        if (_client == null)
        {
            Debug.Log("Client is null");
            return null;
        }
        return _client;
    }

    public void BatchUpdate(Batch batch)
    {
        string localplayername = ClientInit.Instance.LocalPlayer.GetComponent<Player>().GetUser().Username;
        foreach (User user in batch.Users)
        {
            //Debug.Log("User: " + user.Username + " " + user.Pos.X + " " + user.Pos.Y + " " + user.Pos.Z + " " + user.Health);
            if (Players.ContainsKey(user.Username))
            {
                Player p = Players[user.Username].GetComponent<Player>();
                p.GetUser().Health = user.Health;
                if (!p.IsLocal)
                {
                    p.LerpMovement(new Vector3(user.Pos.X, user.Pos.Y, user.Pos.Z));
                }
            }
            else
            {
                GameObject res = Resources.Load<GameObject>("Prefabs/Player");
                GameObject player = Instantiate(res, new Vector3(user.Pos.X, user.Pos.Y, user.Pos.Z), Quaternion.identity) as GameObject;
                player.GetComponent<Player>().GetUser().Username = user.Username;
                Debug.Log("Player " + user.Username + " spawned");
                Players.Add(user.Username, player);
            }
        }

        //Destroy players that are not in the batch
        List<string> toRemove = new List<string>();
        foreach (string key in Players.Keys)
        {
            bool found = false;
            foreach (User user in batch.Users)
            {
                if (user.Username == key)
                {
                    found = true;
                    break;
                }
            }
            if (!found && key != localplayername)
            {
                toRemove.Add(key);
            }
        }
        foreach (string key in toRemove)
        {
            Debug.Log("Player " + key + " removed");
            Destroy(Players[key]);
            Players.Remove(key);
        }

        foreach (Projectile projectile in batch.Projectiles)
        {
            if (Projectiles.ContainsKey(projectile.ID))
            {
                Projectiles[projectile.ID].transform.position = new Vector3(projectile.End.X, projectile.End.Y, projectile.End.Z);
            }
            else
            {
                GameObject proj = Instantiate(Resources.Load("Prefabs/Projectile"), new Vector3(projectile.Start.X, projectile.Start.Y, projectile.Start.Z), Quaternion.identity) as GameObject;
                Projectiles.Add(projectile.ID, proj);
            }
        }
    }
}
