using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
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
    public Player HostPlayer, GuestPlayer;
    public bool isHostReady, isGuestReady;
    public List<Tuple<int, int>> HostBattleList = new List<Tuple<int, int>>();
    public List<Tuple<int, int>> GuestBattleList = new List<Tuple<int, int>>();
  
    [Header("Timer")]
    [SerializeField] private float CardInvokeTimer;
    [SerializeField] private float CardInovkeInvaldTime;

    [Header("UI")]
    [SerializeField] private GameObject matchingPanel;
    [SerializeField] private Slider myHpBar;
    [SerializeField] private Slider myMpBar;
    [SerializeField] private Slider EnemyHpBar;
    [SerializeField] private Slider EnemyMpBar;

    public static GameManager Instance;
    private PhotonView PV;

    void Awake() => Instance = this;

    void Start()
    {
        PV = this.PV();
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
        
        if (PhotonNetwork.IsMasterClient)
        {
            myHpBar.value = HostPlayer.CurHp / HostPlayer.MaxHp;
            myMpBar.value = HostPlayer.CurMp / HostPlayer.MaxMp;
            EnemyHpBar.value = GuestPlayer.CurHp / GuestPlayer.MaxHp;
            EnemyMpBar.value = GuestPlayer.CurMp / GuestPlayer.MaxMp;
        }
        else
        {
            myHpBar.value = GuestPlayer.CurHp / GuestPlayer.MaxHp;
            myMpBar.value = GuestPlayer.CurMp / GuestPlayer.MaxMp;
            EnemyHpBar.value = HostPlayer.CurHp / HostPlayer.MaxHp;
            EnemyMpBar.value = HostPlayer.CurMp / HostPlayer.MaxMp;
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
        matchingPanel.SetActive(false);
        gameState = GameState.StartTurn;
    }

    void OnStartTurn()
    {
        CardManager.Instance.AddCard(HostPlayer.PV().IsMine);
        CardManager.Instance.AddCard(HostPlayer.PV().IsMine);
        CardManager.Instance.AddCard(HostPlayer.PV().IsMine);
        CardManager.Instance.AddCard(GuestPlayer.PV().IsMine);
        CardManager.Instance.AddCard(GuestPlayer.PV().IsMine);
        CardManager.Instance.AddCard(GuestPlayer.PV().IsMine);

        HostPlayer.CurMp = HostPlayer.MaxMp;
        GuestPlayer.CurMp = GuestPlayer.MaxMp;

        HostPlayer.CurMoveCount = HostPlayer.MaxMoveCount;
        GuestPlayer.CurMoveCount = GuestPlayer.MaxMoveCount;
        
        gameState = GameState.LastTurn;
        PV.RPC(nameof(DestoryCard), RpcTarget.AllViaServer);
    }

    void OnLastTurn()
    {
        CardInvokeTimer += Time.deltaTime;

        //3초마다 리스트 인보크
        if (CardInvokeTimer >= 3)
        {
            if (HostBattleList.Any(x => CardData.CardList[x.Item2]?.id == 7))
            {
                var hostSupCard = HostBattleList.Find(x => CardData.CardList[x.Item2]?.id == 7);
                HostBattleList.Remove(hostSupCard);
                HostBattleList.Insert(0, hostSupCard);
            }

            if (GuestBattleList.Any(x => CardData.CardList[x.Item2]?.id == 7))
            {
                var guestSupCard = GuestBattleList.Find(x => CardData.CardList[x.Item2]?.id == 7);
                GuestBattleList.Remove(guestSupCard);
                GuestBattleList.Insert(0, guestSupCard);
            }

            if (HostBattleList.Count > 0)
            {
                var firstHostCard = CardData.CardList[HostBattleList[0].Item2];
                
                print(firstHostCard);
                if (firstHostCard.targetType == TargetType.ME)
                {
                    firstHostCard?.CardEffective(HostPlayer, HostBattleList[0].Item1);
                }

                else if (firstHostCard.targetType == TargetType.ENEMY)
                {
                    firstHostCard?.CardEffective(GuestPlayer, HostBattleList[0].Item1);
                }

                HostBattleList.RemoveAt(0);
            }

            if (GuestBattleList.Count > 0)
            {
                var firstGuestCard = CardData.CardList[GuestBattleList[0].Item2];
                
                print(firstGuestCard);
                if (firstGuestCard.targetType == TargetType.ME)
                {
                    firstGuestCard?.CardEffective(GuestPlayer, GuestBattleList[0].Item1);
                }

                else if (firstGuestCard.targetType == TargetType.ENEMY)
                {
                    firstGuestCard?.CardEffective(HostPlayer, GuestBattleList[0].Item1);
                }

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
        //플레이어 모두 버튼 누르면 다음 턴으로
        if (isHostReady && isGuestReady)
        {
            isHostReady = false;
            isGuestReady = false;

            gameState = GameState.TurnEnd;
        }
    }

    void OnTurnEnd()
    {
        //플레이어 잠금효과 헤제
        HostPlayer.IsLocked = false;
        GuestPlayer.IsLocked = false;

        HostPlayer.CurMoveCount = HostPlayer.MaxMoveCount;
        GuestPlayer.CurMoveCount = GuestPlayer.MaxMoveCount;

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
    void DestoryCard(bool isHost)
    {
        if (isHost)
        {
            var t = CardManager.Instance.hostCards[HostBattleList[0].Item1];
            CardManager.Instance.hostCards.Remove(t);
            PhotonNetwork.Destroy(t.gameObject);
            HostBattleList.RemoveAt(0);
        }
        else
        {
            var t = CardManager.Instance.guestCards[GuestBattleList[0].Item1];
            CardManager.Instance.guestCards.Remove(t);
            PhotonNetwork.Destroy(t.gameObject);
            GuestBattleList.RemoveAt(0);
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

    public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;

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