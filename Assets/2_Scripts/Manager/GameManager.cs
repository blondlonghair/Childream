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
    [Header("Manager")] public GameState gameState = GameState.None;
    public Player hostPlayer, guestPlayer;
    public bool isHostReady, isGuestReady;
    public List<Tuple<int, int>> hostBattleList = new List<Tuple<int, int>>(); //first : 카드 ID, second : range 번호
    public List<Tuple<int, int>> guestBattleList = new List<Tuple<int, int>>();
    public bool? isPlayerWin = null;

    [Header("Timer")] [SerializeField] private float cardInvokeTimer;
    [SerializeField] private float cardInovkeInvaldTime;

    [Header("UI")] [SerializeField] private LerpSlider myHpBar;
    [SerializeField] private Slider myMpBar;
    [SerializeField] private LerpSlider EnemyHpBar;
    [SerializeField] private GameObject gameWinPanel;
    [SerializeField] private GameObject gameLosePanel;
    [SerializeField] private GameStatePanel gameStatePanel;
    [SerializeField] private MatchingDoor matchingDoor;
    [SerializeField] private TurnEndButton turnEndButton;

    public static GameManager Instance;
    private PhotonView PV;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PV = this.PV();
        matchingDoor.gameObject.SetActive(true);

        gameState = GameState.GameSetup;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        EnemyLeftRoom();
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
            myHpBar.SliderValue(hostPlayer.CurHp / hostPlayer.MaxHp);
            EnemyHpBar.SliderValue(guestPlayer.CurHp / guestPlayer.MaxHp);
            myMpBar.value =hostPlayer.CurMp / hostPlayer.MaxMp;
        }
        else
        {
            myHpBar.SliderValue(guestPlayer.CurHp / guestPlayer.MaxHp);
            EnemyHpBar.SliderValue(hostPlayer.CurHp / hostPlayer.MaxHp);
            myMpBar.value = guestPlayer.CurMp / guestPlayer.MaxMp;
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
        matchingDoor.OpenDoor();
        gameState = GameState.StartTurn;
    }

    void OnStartTurn()
    {
        cardInvokeTimer += Time.deltaTime;

        //게임 끝인지 확인
        if (hostPlayer.CurHp <= 0 || guestPlayer.CurHp <= 0)
        {
            cardInvokeTimer = 0;
            gameState = GameState.GameEnd;
        }

        //3초마다 리스트 인보크
        if (cardInvokeTimer >= cardInovkeInvaldTime)
        {
            if (hostBattleList.Count <= 0 && guestBattleList.Count <= 0)
            {
                cardInvokeTimer = 0;
                gameState = GameState.LastTurn;
            }

            //플레이어 이동 잠금은 첫번째로 발동되게  
            if (hostBattleList.Any(x => CardData.CardList[x.Item2]?.id == 7))
            {
                var hostSupCard = hostBattleList.Find(x => CardData.CardList[x.Item2]?.id == 7);
                hostBattleList.Remove(hostSupCard);
                hostBattleList.Insert(0, hostSupCard);
            }

            if (guestBattleList.Any(x => CardData.CardList[x.Item2]?.id == 7))
            {
                var guestSupCard = guestBattleList.Find(x => CardData.CardList[x.Item2]?.id == 7);
                guestBattleList.Remove(guestSupCard);
                guestBattleList.Insert(0, guestSupCard);
            }

            if (hostBattleList.Count > 0 || guestBattleList.Count > 0)
            {
                if (hostBattleList.Count > 0)
                {
                    var firstHostCard = CardData.CardList[hostBattleList[0].Item2];

                    print(firstHostCard);

                    firstHostCard?.CardSecondAbility(hostPlayer, guestPlayer, hostBattleList[0].Item1);

                    hostBattleList.RemoveAt(0);
                }

                if (guestBattleList.Count > 0)
                {
                    var firstGuestCard = CardData.CardList[guestBattleList[0].Item2];

                    print(firstGuestCard);

                    firstGuestCard?.CardSecondAbility(guestPlayer, hostPlayer, guestBattleList[0].Item1);

                    guestBattleList.RemoveAt(0);
                }

                cardInvokeTimer = 0;
            }
        }
    }

    void OnLastTurn()
    {
        gameStatePanel.ShowPanel("턴 시작");
        print("턴 시작");

        CardManager.Instance.AddCard(hostPlayer.PV().IsMine);
        CardManager.Instance.AddCard(hostPlayer.PV().IsMine);
        CardManager.Instance.AddCard(hostPlayer.PV().IsMine);
        CardManager.Instance.AddCard(guestPlayer.PV().IsMine);
        CardManager.Instance.AddCard(guestPlayer.PV().IsMine);
        CardManager.Instance.AddCard(guestPlayer.PV().IsMine);

        //플레이어 잠금효과 헤제
        hostPlayer.IsLocked = false;
        guestPlayer.IsLocked = false;

        hostPlayer.IsPlayerTurn = false;
        guestPlayer.IsPlayerTurn = false;

        hostPlayer.DefElectricity = false;
        hostPlayer.DefExplosion = false;
        hostPlayer.DefMagic = false;

        guestPlayer.DefElectricity = false;
        guestPlayer.DefExplosion = false;
        guestPlayer.DefMagic = false;
        
        hostPlayer.CurMp = hostPlayer.MaxMp;
        guestPlayer.CurMp = guestPlayer.MaxMp;

        hostPlayer.CurMoveCount = hostPlayer.MaxMoveCount;
        guestPlayer.CurMoveCount = guestPlayer.MaxMoveCount;

        hostPlayer.IsPlayerTurn = true;
        guestPlayer.IsPlayerTurn = true;

        turnEndButton.TurnStart();

        gameState = GameState.PlayerTurn;
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
        gameStatePanel.ShowPanel("결과 발표");

        gameState = GameState.StartTurn;
    }

    void OnGameEnd()
    {
        isPlayerWin = PhotonNetwork.IsMasterClient
            ? !(hostPlayer.CurHp <= 0)
            : !(guestPlayer.CurHp <= 0);

        if (isPlayerWin == true)
        {
            StartCoroutine(PanelAnimation(gameWinPanel));
        }
        else if (isPlayerWin == false)
        {
            StartCoroutine(PanelAnimation(gameLosePanel));
        }

        // GameObject panel = PhotonNetwork.IsMasterClient
        //     ? hostPlayer.CurHp <= 0 ? gameLosePanel : gameWinPanel
        //     : guestPlayer.CurHp <= 0 ? gameLosePanel : gameWinPanel;
        //
        // StartCoroutine(PanelAnimation(panel));
    }

    public void TurnEndButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (!isHostReady) turnEndButton.TurnEnd();
            PV.RPC(nameof(_HostReady), RpcTarget.AllBuffered);
        }

        else
        {
            if (!isGuestReady) turnEndButton.TurnEnd();
            PV.RPC(nameof(_GuestReady), RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void DestoryCard(bool isHost)
    {
        if (isHost)
        {
            var t = CardManager.Instance.hostCards[hostBattleList[0].Item1];
            CardManager.Instance.hostCards.Remove(t);
            PhotonNetwork.Destroy(t.gameObject);
            hostBattleList.RemoveAt(0);
        }
        else
        {
            var t = CardManager.Instance.guestCards[guestBattleList[0].Item1];
            CardManager.Instance.guestCards.Remove(t);
            PhotonNetwork.Destroy(t.gameObject);
            guestBattleList.RemoveAt(0);
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
        CardData.CardList[cardId].CardFirstAbility(hostPlayer, guestPlayer, SelectRange);
        hostBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
    }

    [PunRPC]
    private void _AddGuestBattleList(int SelectRange, int cardId)
    {
        CardData.CardList[cardId].CardFirstAbility(guestPlayer, hostPlayer, SelectRange);
        guestBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
    }

    [PunRPC]
    void InitPlayers()
    {
        hostPlayer = GameObject.Find("HostPlayer").GetComponent<Player>();
        guestPlayer = GameObject.Find("GuestPlayer").GetComponent<Player>();
    }

    bool Checkplayers() => GameObject.FindGameObjectsWithTag("Player").Length == 2;

    public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;

    public void ChangeScene(string scene)
    {
        // SceneManager.LoadScene(scene);
        PhotonNetwork.LoadLevel(scene);
        PhotonNetwork.LeaveRoom();
    }

    public void RematchButton()
    {
    }

    public void SurrenderButton()
    {
        PhotonNetwork.LeaveRoom();

        StartCoroutine(PanelAnimation(gameLosePanel));
    }

    private void EnemyLeftRoom()
    {
        if (isPlayerWin == null)
        {
            PhotonNetwork.LeaveRoom();
            StartCoroutine(PanelAnimation(gameWinPanel));
        }
    }

    IEnumerator PanelAnimation(GameObject panel)
    {
        panel.SetActive(true);
        yield return null;

        IsAnimationOver anim = panel.GetComponent<IsAnimationOver>();

        while (true)
        {
            if (anim.isAnimationOver)
            {
                panel.transform.GetChild(0).gameObject.SetActive(true);
                yield break;
            }

            yield return null;
        }
    }
}