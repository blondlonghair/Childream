using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Utils;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        None,
        GameSetup,
        GameStart,
        StartTurn, //턴이 시작될때
        LastTurn, //마지막 턴의 카드들의 효과 발동
        PlayerTurn, //플레이어 행동 (이동, 카드 사용)
        TurnEnd, //턴이 끝날때
        GameEnd
    }

    public GameState gameState = GameState.None;
    public Player myPlayer, therePlayer;
    public PhotonView PV;

    public List<Tuple<int, int>> HostBattleList = new List<Tuple<int, int>>();
    public List<Tuple<int, int>> GuestBattleList = new List<Tuple<int, int>>();
    
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
        if (HostBattleList.Count != 0)
        {
            Debug.Log(HostBattleList[0]);
        }

        if (GuestBattleList.Count != 0)
            print(GuestBattleList[0]);
        
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
                case GameState.StartTurn:
                    OnStartTurn();
                    break;
                case GameState.LastTurn:
                    OnActCard();
                    break;
                case GameState.PlayerTurn:
                    OnPlayerTurn();
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
                myPlayer = item.GetComponent<Player>();
            }
            else
            {
                therePlayer = item.GetComponent<Player>();
            }
        }

        gameState = GameState.GameStart;
    }

    void OnGameStart()
    {
        CardManager.Instance.AddCard(PV.IsMine);
        CardManager.Instance.AddCard(PV.IsMine);
        CardManager.Instance.AddCard(PV.IsMine);

        gameState = GameState.StartTurn;
    }

    void OnStartTurn()
    {
        gameState = GameState.LastTurn;
    }

    void OnActCard()
    {
        gameState = GameState.PlayerTurn;
    }

    void OnPlayerTurn()
    {
        gameState = GameState.PlayerTurn;
    }

    void OnTurnEnd()
    {
        gameState = GameState.StartTurn;
    }

    void OnGameEnd()
    {
        PhotonNetwork.LoadLevel("MainScene");
    }

    public void TurnEndButton()
    {
        
    }

    public void AddBattleList(int SelectRange, int id, bool isHost)
    {
        if (isHost)
        {
            PV.RPC(nameof(AddHostBattleList), RpcTarget.AllBuffered, SelectRange, id);
        }
        else
        {
            PV.RPC(nameof(AddGuestBattleList), RpcTarget.AllBuffered, SelectRange, id);
        }
    }
    
    [PunRPC]
    private void AddHostBattleList(int SelectRange, int cardId)
    {
            HostBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
    }
    
    [PunRPC]
    private void AddGuestBattleList(int SelectRange, int cardId)
    {
        GuestBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
    }
}