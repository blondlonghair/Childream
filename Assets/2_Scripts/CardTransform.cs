using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CardTransform : MonoBehaviour 
{
    private void Start() 
    {
        transform.position += gameObject.name.Contains("Host") && PhotonNetwork.IsMasterClient || gameObject.name.Contains("Guest") && !PhotonNetwork.IsMasterClient ? 
        PhotonNetwork.IsMasterClient ? new Vector3(0, -2, 0) : new Vector3(0, 2, 0) : 
        PhotonNetwork.IsMasterClient ? new Vector3(0, -1, 0) : new Vector3(0, 1, 0);
    }
}