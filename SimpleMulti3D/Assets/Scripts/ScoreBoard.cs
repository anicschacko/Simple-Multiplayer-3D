using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreBoard : MonoBehaviourPunCallbacks
{
    private List<Text> _scoreCards;

    [ContextMenu("Populate List")]
    private void Populate()
    {
        _scoreCards = transform.GetComponentsInChildren<Text>(true).ToList()
            .FindAll(x => x.gameObject.name.Contains("Score"));
    }

    private Dictionary<string, int> players;

    public void PopulateScores(Dictionary<string, int> players)
    {
        this.players = players;
        Populate();
        string scoreString = "";
        for (int i = 0; i < players.Count; i++)
        {
            scoreString = $"{i + 1}.\t{players.Keys.ElementAt(i)}\t-\t{players.Values.ElementAt(i)}";
            _scoreCards[i].text = scoreString;
            _scoreCards[i].gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        _scoreCards = null;
    }
}
