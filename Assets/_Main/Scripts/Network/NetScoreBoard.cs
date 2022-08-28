using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class NetScoreBoard : NetworkBehaviour
{
    private static Dictionary<string, int> _scores = new Dictionary<string, int>();

    private void Awake()
    {
        CmdRequireScores();
    }

    private void Start()
    {
        NetManager.GameOver += GameOverHandler;
    }

    private void OnDestroy()
    {
        NetManager.GameOver -= GameOverHandler;
    }

    private void GameOverHandler(string name)
    {
        if (isServer)
        {
            _scores.Clear();
            UpdateScores();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdRequireScores()
    {
        string[] keys = new string[_scores.Count];
        _scores.Keys.CopyTo(keys, 0);
        int[] values = new int[_scores.Count];
        _scores.Values.CopyTo(values, 0);

        TRpcRequireScores(keys, values);
    }

    [TargetRpc]
    private void TRpcRequireScores(string[] keys, int[] values)
    {
        _scores.Clear();
        for (int i = 0; i < values.Length; i++)
        {
            _scores.Add(keys[i], values[i]);
        }

        NetManager.Instance.UIManager.UpdtaeScoreBoard(_scores);
    }

    [Command(requiresAuthority = false)]
    public void CmdAddPlayer(string name)
    {
        if (_scores.ContainsKey(name))
        {
            _scores[name] = 0;
        }
        else
        {
            _scores.Add(name, 0);
        }

        UpdateScores();

        NetManager.Instance.UIManager.UpdtaeScoreBoard(_scores);
    }

    //TO-DO: need to find reaseon, why calling on client, but not executing on server
    [Command(requiresAuthority = false)]
    public void CmdDeletePlayer(string name)
    {
        Debug.Log("Try delete");
        if (_scores.Remove(name))
        {
            Debug.Log("Delete");
            UpdateScores();

            NetManager.Instance.UIManager.UpdtaeScoreBoard(_scores);
        }
    }

    public void IncScore(string name)
    {
        if (_scores.ContainsKey(name))
        {
            _scores[name]++;

            UpdateScores();

            NetManager.Instance.UIManager.UpdtaeScoreBoard(_scores);
        }
    }

    private void UpdateScores()
    {
        string[] keys = new string[_scores.Count];
        _scores.Keys.CopyTo(keys, 0);
        int[] values = new int[_scores.Count];
        _scores.Values.CopyTo(values, 0);

        CmdUpdateScores(keys, values);
    }

    [Command(requiresAuthority = false)]
    private void CmdUpdateScores(string[] keys, int[] values)
    {
        RpcUpdateScores(keys, values);

        _scores.Clear();
        for (int i = 0; i < values.Length; i++)
        {
            _scores.Add(keys[i], values[i]);
            if (values[i] >= NetManager.Instance.GameParameters.ScoreForWin)
            {
                NetManager.GameOver.Invoke(keys[i]);
            }
        }
    }

    [ClientRpc(includeOwner = false)]
    private void RpcUpdateScores(string[] keys, int[] values)
    {
        _scores.Clear();
        for (int i = 0; i < values.Length; i++)
        {
            _scores.Add(keys[i], values[i]);
            if (values[i] >= NetManager.Instance.GameParameters.ScoreForWin)
            {
                NetManager.GameOver.Invoke(keys[i]);
            }
        }

        NetManager.Instance.UIManager.UpdtaeScoreBoard(_scores);
    }
}
