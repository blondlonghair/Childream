using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
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

    [Header("Manager")] public GameState gameState = GameState.None;
    public Player HostPlayer, GuestPlayer;
    public bool isHostReady, isGuestReady;
    public List<Tuple<int, int>> HostBattleList = new List<Tuple<int, int>>(); //first : 카드 ID, second : range 번호
    public List<Tuple<int, int>> GuestBattleList = new List<Tuple<int, int>>();

    [Header("Timer")] [SerializeField] private float CardInvokeTimer;
    [SerializeField] private float CardInovkeInvaldTime;

    [Header("UI")] [SerializeField] private Slider myHpBar;
    [SerializeField] private Slider myMpBar;
    [SerializeField] private Slider EnemyHpBar;
    [SerializeField] private Slider EnemyMpBar;
    [SerializeField] private GameObject GameEndPanel;

    public static GameManager Instance;
    private PhotonView PV;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PV = this.PV();
        gameState = GameState.GameSetup;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        GameEndPanel.SetActive(true);
        GameEndPanel.transform.Find("GameEndText").GetComponent<Text>().text = "플레이어 나감";
        PhotonNetwork.LeaveRoom();
    }

    void Update()
    {
        if (!AllPlayerIn()) return;

        //스위치 분기 나누기
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

        if (!Checkplayers()) return;

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
        gameState = GameState.StartTurn;
    }

    void OnStartTurn()
    {
        // foreach (var card in CardManager.Instance.guestCards)
        // {
        //     CardManager.Instance.DestroyCard(card.gameObject, PV.IsMine);
        //     print("host card 삭제");
        // }
        //
        // foreach (var card in CardManager.Instance.hostCards)
        // {
        //     CardManager.Instance.DestroyCard(card.gameObject, PV.IsMine);
        //     print("guest card 삭제");
        // }

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
    }

    void OnLastTurn()
    {
        CardInvokeTimer += Time.deltaTime;
        
        //게임 끝인지 확인
        if (HostPlayer.CurHp <= 0 || GuestPlayer.CurHp <= 0)
        {
            gameState = GameState.GameEnd;
        }

        //3초마다 리스트 인보크
        if (CardInvokeTimer >= 3)
        {
            //플레이어 이동 잠금은 첫번째로 발동되게 
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
                
                firstHostCard?.CardSecondAbility(HostPlayer, GuestPlayer, HostBattleList[0].Item1);

                HostBattleList.RemoveAt(0);
            }

            if (GuestBattleList.Count > 0)
            {
                var firstGuestCard = CardData.CardList[GuestBattleList[0].Item2];

                print(firstGuestCard);
                
                firstGuestCard?.CardSecondAbility(GuestPlayer, HostPlayer, GuestBattleList[0].Item1);

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
        if (PhotonNetwork.IsMasterClient)
        {
            if (HostPlayer.CurHp <= 0)
            {
                GameEndPanel.transform.Find("GameEndText").GetComponent<Text>().text = "패배";
            }

            else if (GuestPlayer.CurHp <= 0)
            {
                GameEndPanel.transform.Find("GameEndText").GetComponent<Text>().text = "승리";
            }
        }

        else
        {
            if (HostPlayer.CurHp <= 0)
            {
                GameEndPanel.transform.Find("GameEndText").GetComponent<Text>().text = "승리";
            }

            else if (GuestPlayer.CurHp <= 0)
            {
                GameEndPanel.transform.Find("GameEndText").GetComponent<Text>().text = "패배";
            }
        }
        
        GameEndPanel.SetActive(true);
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
        CardData.CardList[cardId].CardFirstAbility(HostPlayer, GuestPlayer, SelectRange);
        HostBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
    }

    [PunRPC]
    private void _AddGuestBattleList(int SelectRange, int cardId)
    {
        CardData.CardList[cardId].CardFirstAbility(GuestPlayer, HostPlayer, SelectRange);
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

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    
    public void SurrenderButton()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
}