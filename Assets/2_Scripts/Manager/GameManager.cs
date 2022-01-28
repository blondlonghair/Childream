using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.Mathematics;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace Online
{
    public class GameManager : PunSingletonMonoDestroy<GameManager>
    {
        [Header("Manager")] public GameState gameState = GameState.None;
        public OnPlayer hostOnPlayer, guestOnPlayer;
        public bool isHostReady, isGuestReady;
        public List<Tuple<int, int>> hostBattleList = new List<Tuple<int, int>>(); //first : 카드 ID, second : range 번호
        public List<Tuple<int, int>> guestBattleList = new List<Tuple<int, int>>();
        public bool? isPlayerWin = null;
        public Vector3 worldMousePos;

        [Header("Timer")] [SerializeField] private float cardInvokeTimer;
        [SerializeField] private float cardInovkeInvaldTime;

        [Header("UI")] [SerializeField] private LerpSlider myHpBar;
        [SerializeField] private Slider myMpBar;
        [SerializeField] private LerpSlider EnemyHpBar;
        [SerializeField] private GameObject gameWinPanel;
        [SerializeField] private GameObject gameLosePanel;
        [SerializeField] private GameObject gameDrawPanel;
        [SerializeField] private GameStatePanel gameStatePanel;
        [SerializeField] private MatchingDoor matchingDoor;
        [SerializeField] private TurnEndButton turnEndButton;
        [SerializeField] private LoadingPanel _loadingPanel;

        private PhotonView PV;

        private void Start()
        {
            PV = this.PV();

            gameState = GameState.GameSetup;
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            EnemyLeftRoom();
        }

        void Update()
        {
            Vector3 mousePos = Input.mousePosition;
            worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
            worldMousePos.z = 0;

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
                myHpBar.SliderValue(hostOnPlayer.CurHp / hostOnPlayer.MaxHp);
                EnemyHpBar.SliderValue(guestOnPlayer.CurHp / guestOnPlayer.MaxHp);
                myMpBar.value = hostOnPlayer.CurMp / hostOnPlayer.MaxMp;
            }
            else
            {
                myHpBar.SliderValue(guestOnPlayer.CurHp / guestOnPlayer.MaxHp);
                EnemyHpBar.SliderValue(hostOnPlayer.CurHp / hostOnPlayer.MaxHp);
                myMpBar.value = guestOnPlayer.CurMp / guestOnPlayer.MaxMp;
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

            CardManager.Instance.AddCard(hostOnPlayer.PV().IsMine);
            CardManager.Instance.AddCard(hostOnPlayer.PV().IsMine);
            CardManager.Instance.AddCard(guestOnPlayer.PV().IsMine);
            CardManager.Instance.AddCard(guestOnPlayer.PV().IsMine);

            gameState = GameState.StartTurn;
        }

        void OnStartTurn()
        {
            cardInvokeTimer += Time.deltaTime;

            //게임 끝인지 확인
            if (hostOnPlayer.CurHp <= 0 || guestOnPlayer.CurHp <= 0)
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
                if (hostBattleList.Any(x => OnCardData.CardList[x.Item2]?.id == 7))
                {
                    var hostSupCard = hostBattleList.Find(x => OnCardData.CardList[x.Item2]?.id == 7);
                    hostBattleList.Remove(hostSupCard);
                    hostBattleList.Insert(0, hostSupCard);
                }

                if (guestBattleList.Any(x => OnCardData.CardList[x.Item2]?.id == 7))
                {
                    var guestSupCard = guestBattleList.Find(x => OnCardData.CardList[x.Item2]?.id == 7);
                    guestBattleList.Remove(guestSupCard);
                    guestBattleList.Insert(0, guestSupCard);
                }

                if (hostBattleList.Count > 0 || guestBattleList.Count > 0)
                {
                    if (hostBattleList.Count > 0)
                    {
                        var firstHostCard = OnCardData.CardList[hostBattleList[0].Item2];

                        print(firstHostCard);

                        firstHostCard?.CardSecondAbility(hostOnPlayer, guestOnPlayer, hostBattleList[0].Item1);

                        hostBattleList.RemoveAt(0);
                    }

                    if (guestBattleList.Count > 0)
                    {
                        var firstGuestCard = OnCardData.CardList[guestBattleList[0].Item2];

                        print(firstGuestCard);

                        firstGuestCard?.CardSecondAbility(guestOnPlayer, hostOnPlayer, guestBattleList[0].Item1);

                        guestBattleList.RemoveAt(0);
                    }

                    cardInvokeTimer = 0;
                }
            }
        }

        void OnLastTurn()
        {
            gameStatePanel.ShowPanel(PanelState.TurnStart);

            CardManager.Instance.AddCard(hostOnPlayer.PV().IsMine);
            CardManager.Instance.AddCard(hostOnPlayer.PV().IsMine);
            CardManager.Instance.AddCard(guestOnPlayer.PV().IsMine);
            CardManager.Instance.AddCard(guestOnPlayer.PV().IsMine);

            //플레이어 잠금효과 헤제
            hostOnPlayer.IsLocked = false;
            guestOnPlayer.IsLocked = false;

            hostOnPlayer.DefElectricity = false;
            hostOnPlayer.DefExplosion = false;
            hostOnPlayer.DefMagic = false;
            guestOnPlayer.DefElectricity = false;
            guestOnPlayer.DefExplosion = false;
            guestOnPlayer.DefMagic = false;

            hostOnPlayer.CurMp = hostOnPlayer.MaxMp;
            guestOnPlayer.CurMp = guestOnPlayer.MaxMp;

            hostOnPlayer.isPlayerTurn = true;
            guestOnPlayer.isPlayerTurn = true;

            // hostPlayer.CurState = hostPlayer.nextRange;
            // guestPlayer.CurState = guestPlayer.nextRange;

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
            gameStatePanel.ShowPanel(PanelState.Result);

            hostOnPlayer.isPlayerTurn = false;
            guestOnPlayer.isPlayerTurn = false;

            if (PhotonNetwork.IsMasterClient
                    ? (hostOnPlayer.CurState != hostOnPlayer.nextRange)
                    : (guestOnPlayer.CurState != guestOnPlayer.nextRange))
            {
                print(PhotonNetwork.IsMasterClient ? hostOnPlayer.nextRange : guestOnPlayer.nextRange);
                // AddBattleList(PhotonNetwork.IsMasterClient ? hostPlayer.nextRange : guestPlayer.nextRange, 10, PhotonNetwork.IsMasterClient);
                if ((PhotonNetwork.IsMasterClient ? hostOnPlayer : guestOnPlayer).CurState !=
                    (PhotonNetwork.IsMasterClient ? hostOnPlayer : guestOnPlayer).nextRange - 3)
                {
                    AddBattleList(PhotonNetwork.IsMasterClient ? hostOnPlayer.nextRange : guestOnPlayer.nextRange, 10,
                        PhotonNetwork.IsMasterClient);
                }
            }

            gameState = GameState.StartTurn;
        }

        void OnGameEnd()
        {
            isPlayerWin = PhotonNetwork.IsMasterClient
                ? !(hostOnPlayer.CurHp <= 0)
                : !(guestOnPlayer.CurHp <= 0);

            if (isPlayerWin == true)
            {
                StartCoroutine(PanelAnimation(gameWinPanel));
            }
            else if (isPlayerWin == false)
            {
                StartCoroutine(PanelAnimation(gameLosePanel));
            }
        }

        public void TurnEndButton()
        {
            if (gameState != GameState.PlayerTurn) return;

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
            hostOnPlayer.isPlayerTurn = false;
        }

        [PunRPC]
        void _GuestReady()
        {
            isGuestReady = true;
            guestOnPlayer.isPlayerTurn = false;
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
            OnCardData.CardList[cardId].CardFirstAbility(hostOnPlayer, guestOnPlayer, SelectRange);
            hostBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
        }

        [PunRPC]
        private void _AddGuestBattleList(int SelectRange, int cardId)
        {
            OnCardData.CardList[cardId].CardFirstAbility(guestOnPlayer, hostOnPlayer, SelectRange);
            guestBattleList.Add(new Tuple<int, int>(SelectRange, cardId));
        }

        [PunRPC]
        void InitPlayers()
        {
            hostOnPlayer = GameObject.Find("HostPlayer").GetComponent<OnPlayer>();
            guestOnPlayer = GameObject.Find("GuestPlayer").GetComponent<OnPlayer>();
        }

        bool Checkplayers() => GameObject.FindGameObjectsWithTag("Player").Length == 2;

        public bool AllPlayerIn() => PhotonNetwork.PlayerList.Length == 2;

        public void ChangeScene(string scene)
        {
            // SceneManager.LoadScene(scene);
            PhotonNetwork.LeaveRoom();
            _loadingPanel.Close(scene);
            // PhotonNetwork.LoadLevel(scene);
        }

        public void RematchButton()
        {
        }

        public void SurrenderButton()
        {
            StartCoroutine(PanelAnimation(gameLosePanel));
        }

        private void EnemyLeftRoom()
        {
            if (isPlayerWin == null)
            {
                StartCoroutine(PanelAnimation(gameWinPanel));
            }
        }

        IEnumerator PanelAnimation(GameObject panel)
        {
            panel.SetActive(true);
            yield return null;

            if (panel.TryGetComponent(out IsAnimationOver anim))
            {
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

        public (GameObject, int) CastRayRange()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(worldMousePos, Vector2.zero, 0f);
            int range = 0;

            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.tag.Contains("Range") && !hit.collider.gameObject.CompareTag("EffectRange"))
                {
                    range = int.Parse(hit.collider.gameObject.tag.Replace("Range", ""));

                    if (hit.collider.gameObject.GetPhotonView().IsMine)
                    {
                        range += 3;
                    }

                    return (hit.collider.gameObject, range);
                }
            }

            return (null, range);
        }

        public bool CheckCastRay(string tag)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return false;

            RaycastHit2D[] hits = Physics2D.RaycastAll(worldMousePos, Vector2.zero, 0f);

            foreach (var hit2D in hits)
            {
                if (hit2D.collider != null && hit2D.collider.gameObject.CompareTag(tag))
                {
                    return true;
                }
            }

            return false;
        }
    }
}