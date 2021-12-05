using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class Range : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    [SerializeField] private GameObject[] childs;

    [SerializeField] private Sprite[] frontRange;
    [SerializeField] private Sprite[] backRange;

    void Start()
    {
        PV = this.PV();
        
        if (PhotonNetwork.IsMasterClient)
        {
            transform.Rotate(0,0,180);
            
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, 3.5f, 0);
                gameObject.name = "MasterRange";
                childs[0].transform.localPosition = new Vector3(-3.5f, 0, 0);
                childs[2].transform.localPosition = new Vector3(3.5f, 0, 0);
                for (int i = 0; i < 3; i++)
                {
                    childs[i].GetComponent<SpriteRenderer>().sprite = frontRange[i];
                }
            }

            else
            {
                transform.position = new Vector3(0, -3.5f, 0);
                gameObject.name = "GuestRange";
                childs[0].transform.localPosition = new Vector3(-2.7f, 0, 0);
                childs[2].transform.localPosition = new Vector3(2.7f, 0, 0);
                for (int i = 0; i < 3; i++)
                {
                    BoxCollider2D collider2D = childs[i].GetComponent<BoxCollider2D>();
                    collider2D.size = new Vector2(2.7f, collider2D.size.y);
                    childs[i].GetComponent<SpriteRenderer>().sprite = backRange[i];
                }
            }
        }
        else
        {
            if (PV.IsMine)
            {
                transform.position = new Vector3(0, -3.5f, 0);
                gameObject.name = "GuestRange";
                childs[0].transform.localPosition = new Vector3(-3.5f, 0, 0);
                childs[2].transform.localPosition = new Vector3(3.5f, 0, 0);
                for (int i = 0; i < 3; i++)
                {
                    childs[i].GetComponent<SpriteRenderer>().sprite = frontRange[i];
                }
            }

            else
            {
                transform.position = new Vector3(0, 3.5f, 0);
                gameObject.name = "MasterRange";
                childs[0].transform.localPosition = new Vector3(-2.7f, 0, 0);
                childs[2].transform.localPosition = new Vector3(2.7f, 0, 0);
                for (int i = 0; i < 3; i++)
                {
                    BoxCollider2D collider2D = childs[i].GetComponent<BoxCollider2D>();
                    collider2D.size = new Vector2(2.7f, collider2D.size.y);
                    childs[i].GetComponent<SpriteRenderer>().sprite = backRange[i];
                }
            }
            //collider x size 2.7
        }
    }
}
