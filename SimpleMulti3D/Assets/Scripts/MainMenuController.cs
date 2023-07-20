using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private InputField _nickname;
    [SerializeField] private Button _startButton;
    [SerializeField] private ScoreBoard _scoreBoard;

    private Dictionary<string, int> _leaderboardData;

    private void Awake()
    {
        _startButton.onClick.AddListener(OnClickStartButton);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        
        PopulateLeaderboard();
    }

    private void OnClickStartButton()
    {
        PlayerPrefs.SetString("nickname", _nickname.text);
        SceneManager.LoadScene((int)GameConstants.Scenes.Loading);
    }

    void PopulateLeaderboard()
    {
        string jsonString = PlayerPrefs.GetString(GameConstants.LeaderboardData);
        if (string.IsNullOrEmpty(jsonString)) return;
        _leaderboardData = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonString);
        _scoreBoard.PopulateScores(_leaderboardData);
    }
}
