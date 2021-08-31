using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Utils;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        None,
        GameSetup,
        GameStart,

        // 게임 루프
        OnTurn,
        ActCard,
        PlayerMove,
        PlayerAtk,
        TurnEnd,

        // 게임 루프
        GameEnd
    }

    GameState gameState = GameState.None;
    GameObject myPlayer, therePlayer;
    private PhotonView PV;

    public static GameManager Instance;

    void Awake() => Instance = this;

    void Start()
    {
        PV = this.PV();
    }

    public override void OnJoinedRoom()
    {
        gameState = GameState.GameSetup;
    }

    public bool AllPlayerIn()
    {
        return PhotonNetwork.PlayerList.Length == 2;
    }

    void Update()
    {
        //if (AllPlayerIn())
        {
            switch (gameState)
            {
                case GameState.GameSetup:
                    OnGameSetup();
                    break;
                case GameState.GameStart:
                    OnGameStart();
                    break;
                case GameState.OnTurn:
                    OnTurn();
                    break;
                case GameState.ActCard:
                    OnActCard();
                    break;
                case GameState.PlayerMove:
                    OnPlayerMove();
                    break;
                case GameState.PlayerAtk:
                    OnPlayerAtk();
                    break;
                case GameState.TurnEnd:
                    OnTurnEnd();
                    break;
                case GameState.GameEnd:
                    OnGameEnd();
                    break;
            }
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
            else
            {
                therePlayer = item;
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
        gameState = GameState.PlayerAtk;
    }

    void OnPlayerAtk()
    {
        gameState = GameState.TurnEnd;
    }

    void OnTurnEnd()
    {
        gameState = GameState.OnTurn;
    }

    void OnGameEnd()
    {
        PhotonNetwork.LoadLevel("MainScene");
    }
}