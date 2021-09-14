using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Utils;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        None = 1,
        GameSetup = 2,
        GameStart = 4,
        StartTurn = 8, //���� ���۵ɶ�
        LastTurn, //������ ���� ī����� ȿ�� �ߵ�
        PlayerTurn, //�÷��̾� �ൿ (�̵�, ī�� ���)
        TurnEnd, //���� ������
        GameEnd
    }

    public GameState gameState = GameState.None;
    public Player HostPlayer, GuestPlayer;
    public bool isHostReady, isGuestReady;
    private PhotonView PV;

    private float CardInvokeTimer;
    [SerializeField] private float CardInovkeInvaldTime;

    // public delegate void HostInvoker(int i, int j);
    // public event HostInvoker hostInvoker;

    public List<Tuple<int, int>> HostBattleList = new List<Tuple<int, int>>();
    public List<Tuple<int, int>> GuestBattleList = new List<Tuple<int, int>>();

    public static GameManager Instance;

    void Awake() => Instance = this;

    void Start()
    {
        PV = this.PV();
        AddBattleList(1, 0, PhotonNetwork.IsMasterClient);
        AddBattleList(1, 0, !PhotonNetwork.IsMasterClient);
    }

    public override void OnJoinedRoom()
    {
        gameState = GameState.GameSetup;
    }

    void Update()
    {
        if (!AllPlayerIn())
            return;
        
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
        // print(HostPlayer);
        // print(GuestPlayer);
        gameState = GameState.StartTurn;
    }

    void OnStartTurn()
    {
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

        //3�ʸ��� ����Ʈ �κ�ũ
        if (CardInvokeTimer >= 3)
        {
            if (HostBattleList.Count > 0)
            {
                print($"ī�� ȿ�� ����");
                CardData.CardList[HostBattleList[0].Item2].CardEffective(GuestPlayer, HostBattleList[0].Item1);
                // Destroy(CardManager.Instance.hostCards[HostBattleList[0].Item1]);
                HostBattleList.RemoveAt(0);
            }

            if (GuestBattleList.Count > 0)
            {
                print($"ī�� ȿ�� ����");
                CardData.CardList[GuestBattleList[0].Item2].CardEffective(HostPlayer, GuestBattleList[0].Item1);
                // Destroy(CardManager.Instance.guestCards[GuestBattleList[0].Item1]);
                GuestBattleList.RemoveAt(0);
            }

            if (HostBattleList.Count <= 0 && GuestBattleList.Count <= 0)
            {
                gameState = GameState.PlayerTurn;
            }

            CardInvokeTimer = 0;
        }
    }

    void OnPlayerTurn()
    {
        //�÷��̾� ��� ��ư ������ ���� ������
        if (isHostReady && isGuestReady)
        {
            isHostReady = false;
            isGuestReady = false;

            gameState = GameState.TurnEnd;
        }
    }

    void OnTurnEnd()
    {
        HostPlayer.IsLocked = false;
        GuestPlayer.IsLocked = false;

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
            PV.RPC(nameof(_HostReady), RpcTarget.AllBuffered);
        }

        else
        {
            PV.RPC(nameof(_GuestReady), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void _HostReady()
    {
        isHostReady = true;
    }

    [PunRPC]
    void _GuestReady()
    {
        isGuestReady = true;
    }

    public void AddBattleList(int SelectRange, int id, bool isHost)
    {
        if (isHost)
        {
            PV.RPC(nameof(_AddHostBattleList), RpcTarget.AllBuffered, SelectRange, id);
        }
        else
        {
            PV.RPC(nameof(_AddGuestBattleList), RpcTarget.AllBuffered, SelectRange, id);
        }
    }

    [PunRPC]
    private void _AddHostBattleList(int SelectRange, int cardId)
    {
        HostBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
    }

    [PunRPC]
    private void _AddGuestBattleList(int SelectRange, int cardId)
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
    
    public bool AllPlayerIn()
    {
        return PhotonNetwork.PlayerList.Length == 2;
    }

    // public void AddPlayer(Player player, bool isMaster)
    // {
    //     PV.RPC(nameof(_AddPlayer), RpcTarget.AllBuffered, player, isMaster);
    // }
    //
    // [PunRPC]
    // void _AddPlayer(Player player, bool isMaster)
    // {
    //     if (isMaster)
    //         HostPlayer = player;
    //     else
    //         GuestPlayer = player;
    // }
}