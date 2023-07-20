using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PUNConnect : MonoBehaviourPunCallbacks
{
	[SerializeField] private GameObject _waitingPrompt;

	private void Awake()
	{
	}

	private void Start()
	{
		if(PhotonNetwork.IsConnected)
			OnConnectedToMaster();
		else
			PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby()
	{
		PhotonNetwork.JoinOrCreateRoom("room1", new RoomOptions{IsOpen = true, IsVisible = true}, TypedLobby.Default);
	}

	public override void OnJoinedRoom()
	{
		SetNetworkPlayerData();
			
		if(PhotonNetwork.PlayerList.Length > 1)
			PhotonNetwork.LoadLevel((int)GameConstants.Scenes.Game);
		else
			_waitingPrompt.SetActive(true);
	}

	private void SetNetworkPlayerData()
	{
		var name = PlayerPrefs.GetString("nickname");
		PhotonNetwork.NickName = string.IsNullOrEmpty(name) ? $"Player {PhotonNetwork.LocalPlayer.ActorNumber}" : name;
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		if(PhotonNetwork.PlayerList.Length > 1)
			PhotonNetwork.LoadLevel((int)GameConstants.Scenes.Game);
	}
}
