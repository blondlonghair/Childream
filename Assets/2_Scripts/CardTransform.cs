using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CardTransform : MonoBehaviour 
{
    private void Start() 
    {
        if ((gameObject.name.Contains("Host") && PhotonNetwork.IsMasterClient) || (gameObject.name.Contains("Guest") && !PhotonNetwork.IsMasterClient))
            transform.Translate(0, 1, 0);
    }
}