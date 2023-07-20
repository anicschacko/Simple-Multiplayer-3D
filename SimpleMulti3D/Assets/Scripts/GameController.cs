using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public static GameConfig GameConfig;

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private ObjectPooling _pooler;
    [SerializeField] private ScoreBoard _scoreBoard;
    [SerializeField] private Text _timerText;
    [SerializeField] private Text _popUPText;
    
    private Dictionary<string, int> _playerScores;
    private WaitForSeconds OneSecond = new WaitForSeconds(1f);
    
    private void Awake()
    {
        GameConfig = Resources.Load<GameConfig>("GameConfig");
        GameEvents.UpdateScore += UpdateScores;
        _playerScores = new Dictionary<string, int>();
    }

    private void UpdateScores(string playerName)
    {
        if (_playerScores.ContainsKey(playerName))
            _playerScores[playerName] += 1;
        else
            _playerScores.Add(playerName, 1);

        var sortDict = from entry in _playerScores orderby entry.Value descending select entry;
        _playerScores = sortDict.ToDictionary(x => x.Key, x => x.Value);

        if (_scoreBoard == null)
            _scoreBoard = FindObjectOfType<ScoreBoard>();
        _scoreBoard.PopulateScores(_playerScores);
    }
    
    private IEnumerator Start()
    {
        yield return OneSecond;
        PhotonNetwork.Instantiate(_playerPrefab.name, GameConfig.RandomVec3InsideGrid(0.01f), Quaternion.identity);
        
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(nameof(RoomTimerCoroutine));
            StartCoroutine(nameof(StartPickupSpawn));
        }
    }

    private bool isTimerRunning = false;

    [PunRPC]
    private IEnumerator RoomTimerCoroutine()
    {
        yield return null;
        isTimerRunning = true;
        float startTime = Time.time;
        float roomDuration = GameConfig.gameTimer;

        // Store the room's expiration time in the room's properties using Photon Custom Property
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add("RoomExpirationTime", startTime + roomDuration);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

        while (isTimerRunning && PhotonNetwork.IsMasterClient)
        {
            float elapsed = Time.time - startTime;
            float remainingTime = Mathf.Max(0f, roomDuration - elapsed);

            // Format the remaining time as minutes and seconds (e.g., "03:45")
            int minutes = Mathf.FloorToInt(remainingTime / 60f);
            int seconds = Mathf.FloorToInt(remainingTime % 60f);
            string timerString = $"{minutes:00}:{seconds:00}";

            photonView.RPC(nameof(UpdateTimerUI), RpcTarget.AllBuffered, timerString);
            
            if (elapsed >= roomDuration)
            {
                photonView.RPC("GameOver", RpcTarget.AllBuffered);
                break;
            }

            yield return null;
        }
    }

    [PunRPC]
    private void UpdateTimerUI(string s)
    {
        if (_timerText != null)
            _timerText.text = s;
    }

    [PunRPC]
    void GameOver()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        string jsonString = JsonConvert.SerializeObject(_playerScores);
        PlayerPrefs.SetString(GameConstants.LeaderboardData, jsonString);
        _playerScores.Clear();
        PhotonNetwork.LoadLevel((int)GameConstants.Scenes.MainMenu);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _popUPText.text = $"{otherPlayer.NickName} left Room\nReturning to MainScene";
        WaitForSeconds(3f, () =>
        {
            if(PhotonNetwork.PlayerList.Length < 2)
                photonView.RPC("GameOver", RpcTarget.AllBuffered);
        });
    }

    public override void OnLeftLobby()
    {
        Debug.LogError($"{PhotonNetwork.LocalPlayer.NickName} left lobby");
    }

    private IEnumerator StartPickupSpawn()
    {
        yield return null;

        while (true)
        {
            photonView.RPC("InstantiatePickup", RpcTarget.AllBuffered, GameConfig.RandomVec3InsideGrid(0.2f));
            yield return new WaitForSeconds(GameConfig.pickUpItemLifeSpan + 1f);
        }
    }

    [PunRPC]
    void InstantiatePickup(Vector3 position)
    {
        GameObject go = _pooler.GetItemFromPool();
        go.GetComponent<PhotonView>().ViewID = UnityEngine.Random.Range(50, 999);
        go.transform.position = position;
    }

    void WaitForSeconds(float seconds, System.Action callback)
    {
        StartCoroutine(Wait(seconds, callback));

        IEnumerator Wait(float s, System.Action callback)
        {
            yield return new WaitForSeconds(s);
            callback?.Invoke();
        }
            
    }
}
