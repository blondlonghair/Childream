using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Player : MonoBehaviourPunCallbacks
{
    GameObject target;

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CastRay("Card");
            
            if (target == null)
                return;

            if (target.GetComponent<PhotonView>().IsMine)
            {
                print("down");
            }
        }

        if (Input.GetMouseButton(0))
        {
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastRay("Range");

            if (target == null)
                return;

            if (!target.GetComponentInParent<PhotonView>().IsMine)
            {
                print("up");
            }
        }
    }

    void CastRay(string _tag)
    {
        target = null;

        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

        if (hit.collider != null && hit.collider.gameObject.CompareTag(_tag))
        {
            target = hit.collider.gameObject;
        }
    }
}
