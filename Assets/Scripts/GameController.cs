using System;
using System.Collections.Generic;
using DTOs;
using GameClient;
using GameServer;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    private Camera _cam;

    public Dictionary<string, GameObject> Players = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> Projectiles = new Dictionary<string, GameObject>();
    private Server _server;
    private Client _client;
    public Player LocalPlayer;

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

    public void Start()
    {
        GameObject go = Instantiate(Resources.Load(CONSTANTS.PlayerPrefab), new Vector3(0, 0.5f, 0), Quaternion.identity) as GameObject;
        _cam = Camera.main;

        //Add camera to player with offset
        _cam.transform.SetParent(go.transform);
        _cam.transform.localPosition = CONSTANTS.CameraOffset;

        LocalPlayer = go.GetComponent<Player>();
        Debug.Log(LocalPlayer.GetUser().Username + " spawned");
        Players.Add(LocalPlayer.GetUser().Username, go);
        LocalPlayer.IsLocal = true;
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

    public void Update()
    {
        while (_client.NetworkHandler.ActionQueue.Count > 0)
        {
            _client.NetworkHandler.ActionQueue.TryDequeue(out Batch batch);
            BatchUpdate(batch);
        }
    }

    public void BatchUpdate(Batch batch)
    {
        try
        {
            Debug.Log("Batch received " + batch.Users.Length);
            string localplayername = LocalPlayer.GetUser().Username;
            foreach (User user in batch.Users)
            {
                Debug.Log("User " + user.Username + " updated");
                //Update non-local players already spawned
                if (Players.ContainsKey(user.Username))
                {
                    Player p = Players[user.Username].GetComponent<Player>();
                    p.GetUser().Health = user.Health;
                    if (!p.IsLocal)
                    {
                        p.LerpMovement(new Vector3(user.Pos.X, user.Pos.Y, user.Pos.Z));
                    }
                }
                //Spawn new players
                else
                {
                    GameObject res = Resources.Load<GameObject>(CONSTANTS.PlayerPrefab);
                    GameObject player = Instantiate(res, new Vector3(user.Pos.X, user.Pos.Y, user.Pos.Z), Quaternion.identity);
                    player.GetComponent<Player>().GetUser().Username = user.Username;
                    Debug.Log("Player " + user.Username + " spawned");
                    foreach (GameObject otherPlayer in Players.Values)
                    {
                        //Physics.IgnoreCollision(player.GetComponent<Collider>(), otherPlayer.GetComponent<Collider>());
                    }
                    Players.Add(user.Username, player);
                }
            }

            //Destroy players that are not in the batch, except for the local player
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
                    if (projectile.Owner.Username != localplayername)
                        Projectiles[projectile.ID].GetComponent<MonoProjectile>().LerpMovement(new Vector3(projectile.Current.X, projectile.Current.Y, projectile.Current.Z));
                }
                else
                {
                    GameObject res = Resources.Load<GameObject>(CONSTANTS.ProjectilePrefab);
                    GameObject proj = Instantiate(res, new Vector3(projectile.Start.X, projectile.Start.Y, projectile.Start.Z), Quaternion.identity);
                    proj.GetComponent<MonoProjectile>().Launch(projectile);
                    Projectiles.Add(projectile.ID, proj);
                }
            }

            toRemove = new List<string>();
            foreach (string key in Projectiles.Keys)
            {
                bool found = false;
                foreach (Projectile projectile in batch.Projectiles)
                {
                    if (projectile.ID == key)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    toRemove.Add(key);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
