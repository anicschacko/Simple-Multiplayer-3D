using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviourPunCallbacks
{
	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private Transform raycastOrigin;
	[SerializeField] private LayerMask wallLayer;
	

	private Vector3 moveDirection;
	private int score = 0;
	
    private void Awake()
    {
	    InputController.UpdateYRotation += OnUpdateYRotation;
    }
    
    private void OnUpdateYRotation(bool shouldUpdate, float obj)
    {
	    if (!shouldUpdate) return;
	    var offset = transform.localRotation.eulerAngles.y + obj;
	    transform.localRotation = Quaternion.Euler(0f, offset, 0f);
    }

    void Start()
    {
	    moveDirection = Vector3.forward;
	    score = 0;
    }
    
    void Update()
    {
	    transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

	    // Check for wall collisions using raycasting
	    RaycastHit hit;
	    if (Physics.Raycast(raycastOrigin.position, transform.forward, out hit, 1f, wallLayer))
	    {
		    Vector3 reflection = Vector3.Reflect(transform.forward, hit.normal);
		    transform.rotation = Quaternion.LookRotation(reflection);
	    }
    }

    private void OnTriggerEnter(Collider other)
    {
	    if (!photonView.IsMine) return;
	    if (!other.gameObject.name.Contains("Pickup")) return;

		string nickname = PhotonNetwork.LocalPlayer.NickName;
		other.gameObject.SetActive(false);
		photonView.RPC("IncrementScore", RpcTarget.AllBuffered, nickname);
    }

    [PunRPC]
    private void IncrementScore(string nickname)
    {
	    GameEvents.UpdateScore?.Invoke(nickname);
    }

    private void OnDestroy()
    {
	    InputController.UpdateYRotation -= OnUpdateYRotation;
    }
}
