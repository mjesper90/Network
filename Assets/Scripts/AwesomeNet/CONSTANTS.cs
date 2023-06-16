//TODO: Refactor to be config files
using UnityEngine;

public static class CONSTANTS
{
    //Camera
    public static readonly Vector3 CameraOffset = new Vector3(0, 2.5f, -5f);

    //Game
    public static readonly float Gravity = 9.82f;
    public static readonly float ProjectileSpeed = 100f;
    public static readonly float PlayerSpeed = 5f;
    public static readonly float JumpForce = 1000f;

    //Networking
    public static readonly float ServerSpeed = 0.1f;
    public static readonly int Port = 8052;
    public static readonly string ServerIP = "127.0.0.1";
    public static readonly int BufferSize = 4096;

    //Prefabs
    public static readonly string PlayerPrefab = "MyShooter/Prefabs/Player";
    public static readonly string ProjectilePrefab = "MyShooter/Prefabs/Projectile";
    public static readonly string WeaponPrefab = "MyShooter/Prefabs/Weapon";
    public static readonly string ClientPrefab = "Network/Prefabs/Client";
    public static readonly string ServerPrefab = "Network/Prefabs/Server";

    //Matchmaking
    public static readonly int MaxPlayersPerMatch = 999;
}