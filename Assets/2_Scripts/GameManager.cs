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
        StartTurn, //���� ���۵ɶ�
        LastTurn, //������ ���� ī����� ȿ�� �ߵ�
        PlayerTurn, //�÷��̾� �ൿ (�̵�, ī�� ���)
        TurnEnd, //���� ������
        GameEnd
    }

    public GameState gameState = GameState.None;
    public PhotonView PV;
    public Player HostPlayer, GuestPlayer;
    public bool isHostReady, isGuestReady;

    private float CardInvokeTimer;
    [SerializeField] private float CardInovkeInvaldTime;

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

        if (AllPlayerIn())
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
                    OnLastTurn();
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

        gameState = GameState.GameStart;
    }

    void OnGameStart()
    {
        if (!Checkplayers())
            return;

        PV.RPC(nameof(InitPlayers), RpcTarget.AllBuffered);
        print(HostPlayer);
        print(GuestPlayer);
        gameState = GameState.StartTurn;
    }

    void OnStartTurn()
    {
        print("AddCard" + (PhotonNetwork.IsMasterClient ? " Host" : " Guest"));

        CardManager.Instance.AddCard(PV.IsMine);
        CardManager.Instance.AddCard(PV.IsMine);
        CardManager.Instance.AddCard(PV.IsMine);

        HostPlayer.CurMp = HostPlayer.MaxMp;
        GuestPlayer.CurMp = GuestPlayer.MaxMp;

        HostPlayer.CurMoveCount = HostPlayer.MaxMoveCount;
        GuestPlayer.CurMoveCount = GuestPlayer.MaxMoveCount;

        gameState = GameState.LastTurn;
    }

    void OnLastTurn()
    {
        CardInvokeTimer += Time.deltaTime;

        if (CardInvokeTimer >= 0.5)
        {
            // HostBattleList[0];
            // GuestBattleList[0]; //���⼭ ��Ʋ����Ʈ�� �κ�ũ

            HostBattleList.RemoveAt(0);
            GuestBattleList.RemoveAt(0);
        }

        gameState = GameState.PlayerTurn;
    }

    void OnPlayerTurn()
    {
        if (isHostReady && isGuestReady)
        {
            gameState = GameState.TurnEnd;
            isHostReady = false;
            isGuestReady = false;
        }
    }

    void OnTurnEnd()
    {
        gameState = GameState.StartTurn;
    }

    void OnGameEnd()
    {
        // PhotonNetwork.LoadLevel("MainScene");
    }

    public void TurnEndButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC(nameof(HostReady), RpcTarget.AllBuffered);
        }

        else
        {
            PV.RPC(nameof(GuestReady), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void HostReady()
    {
        isHostReady = true;
    }

    [PunRPC]
    void GuestReady()
    {
        isGuestReady = true;
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

    [PunRPC]
    void InitPlayers()
    {
        HostPlayer = GameObject.Find("HostPlayer").GetComponent<Player>();
        GuestPlayer = GameObject.Find("GuestPlayer").GetComponent<Player>();
    }

    bool Checkplayers() => GameObject.FindGameObjectsWithTag("Player").Length == 2;

    public void AddPlayer(Player player, bool isMaster)
    {
        PV.RPC(nameof(_AddPlayer), RpcTarget.AllBuffered, player, isMaster);
    }

    [PunRPC]
    void _AddPlayer(Player player, bool isMaster)
    {
        if (isMaster)
            HostPlayer = player;
        else
            GuestPlayer = player;
    }
}