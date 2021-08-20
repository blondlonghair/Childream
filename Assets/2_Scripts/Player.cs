using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class Player : MonoBehaviour
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
            if (!target.GetComponent<PhotonView>().IsMine)
            {
                print("down");
                return;
            }
        }

        if (Input.GetMouseButton(0))
        {
            print("button");
            //CastRay("Card");
        }

        if (Input.GetMouseButtonUp(0))
        {
            CastRay("Range");
            if (target.CompareTag("Range"))
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
