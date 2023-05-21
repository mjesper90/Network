//TODO: Refactor
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
    public static readonly string PlayerPrefab = "Prefabs/Player";
    public static readonly string ProjectilePrefab = "Prefabs/Projectile";
    public static readonly string WeaponPrefab = "Prefabs/Weapon";

    //Matchmaking
    public static readonly int MaxPlayersPerMatch = 2;
}