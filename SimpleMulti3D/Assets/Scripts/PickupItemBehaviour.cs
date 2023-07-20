using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PickupItemBehaviour : MonoBehaviourPunCallbacks
{
	private void OnEnable()
	{
		StartCoroutine(nameof(LifeSpanCountDown));
	}

	IEnumerator LifeSpanCountDown()
	{
		yield return new WaitForSeconds(GameController.GameConfig.pickUpItemLifeSpan);
		this.gameObject.SetActive(false);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.name.Contains("Player"))
		{
			photonView.RPC(nameof(DestroyPickup), RpcTarget.AllBuffered);
		}
	}

	[PunRPC]
	public void DestroyPickup()
	{
		gameObject.SetActive(false);
	}
}