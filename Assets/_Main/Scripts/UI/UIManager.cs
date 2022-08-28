using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreBoard;
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private GameObject _enterScreen;
    [SerializeField] private InputField _nicknameIF;

    private void Start()
    {
        NetManager.GameOver += GameOverHandler;
    }

    private async void GameOverHandler(string name)
    {
        _gameOverScreen.SetActive(true);
        _gameOverText.text = name + " is winner!";
        await Task.Delay((int)(NetManager.Instance.GameParameters.GameOverDelay * 1000));
        _gameOverScreen.SetActive(false);
    }

    public void UpdtaeScoreBoard(Dictionary<string, int> scores)
    {
        _scoreBoard.text = string.Empty;
        foreach (var item in scores)
        {
            _scoreBoard.text += item.Key + ": " + item.Value + "\n\n";
        }
    }

    public void SwitchEnterScreen()
    {
        _enterScreen.SetActive(!_enterScreen.activeInHierarchy);
    }

    public string GetNickname()
    {
        return _nicknameIF.text;
    }
}
