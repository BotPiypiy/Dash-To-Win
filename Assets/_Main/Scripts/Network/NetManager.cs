using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Threading.Tasks;

public class NetManager : NetworkManager
{
    public static NetManager Instance { get; private set; }

    [Header("Other objects")]
    public NetScoreBoard ScoreBoard;
    public UIManager UIManager;
    [Header("Game parameters")]
    public GameParameters GameParameters;

    public static Action<string> GameOver;

    public static PlayerController LocalPlayer;

    public override void Awake()
    {
        base.Awake();

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void Disconnect()
    {
        LocalPlayer.Disconnect();

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }
    }
}
