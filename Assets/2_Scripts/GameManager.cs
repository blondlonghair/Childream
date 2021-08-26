using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        None,
        GameSetup,
        GameStart,
        OnTurn,
        ActCard,
        PlayerMove,
        PlayerAtk,
        TurnEnd,
    }

    GameState gameState = GameState.None;
    PhotonView PV;
    GameObject myPlayer;

    public static GameManager Instance;

    void Awake() => Instance = this;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void OnJoinedRoom()
    {
        gameState = GameState.GameSetup;
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.GameSetup: OnGameSetup(); break;
            case GameState.GameStart: OnGameStart(); break;
            case GameState.OnTurn: OnTurn(); break;
            case GameState.ActCard: OnActCard(); break;
            case GameState.PlayerMove: OnPlayerMove(); break;
            case GameState.PlayerAtk: OnPlayerAtk(); break;
            case GameState.TurnEnd: OnTurnEnd(); break;
        }
    }

    void OnGameSetup()
    {
        PhotonNetwork.Instantiate("Prefab/Player", Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("Prefab/Ranges", Vector3.zero, Quaternion.identity);

        GameObject[] tPlayer;
        tPlayer = GameObject.FindGameObjectsWithTag("Player");

        foreach (var item in tPlayer)
        {
            if (item.GetComponent<PhotonView>().IsMine)
            {
                myPlayer = item;
            }
        }

        gameState = GameState.GameStart;
    }

    void OnGameStart()
    {
        CardManager.Instance.AddCard(PV.IsMine);
        CardManager.Instance.AddCard(PV.IsMine);
        CardManager.Instance.AddCard(PV.IsMine);

        gameState = GameState.OnTurn;
    }

    void OnTurn()
    {
        gameState = GameState.ActCard;
    }

    void OnActCard()
    {
        gameState = GameState.PlayerMove;
    }

    void OnPlayerMove()
    {

    }

    void OnPlayerAtk()
    {

    }

    void OnTurnEnd()
    {

    }
}