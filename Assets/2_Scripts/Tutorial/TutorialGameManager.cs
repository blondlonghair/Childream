using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialGameManager : SingletonMonoDestroy<TutorialGameManager>
    {
        public List<Tuple<int, int>> hostBattleList = new List<Tuple<int, int>>(); //first : 카드 ID, second : range 번호
        public List<Tuple<int, int>> guestBattleList = new List<Tuple<int, int>>();
        [SerializeField] private TurnEndButton turnEndButton;
        private bool _turnEnd;

        [SerializeField] private TutorialPlayer _player;
        [SerializeField] private GameObject _ai;

        [SerializeField] private LerpSlider AIHpBar;
        [SerializeField] private LerpSlider PlayerHpBar;

        [SerializeField] private GameObject textPanel;
        [SerializeField] private LoadingPanel _loadingPanel;

        private float theTime;
        private int textCount;
        private Coroutine _coroutine;
        private bool isSkipText, isTyping;

        enum Turn
        {
            FirstTurn,
            FirstTurnCutScene,
            FirstTurnEnd,
            SecondTurn,
            SecondTurnCutScene,
            SecondTurnEnd,
            ThirdTurn,
            ThirdTurnCutScene,
            ThirdTurnEnd,
            ForthTurn,
            ForthTurnCutScene,
            ForthTurnEnd
        }

        private Turn _turn = Turn.FirstTurn;

        public void AddBattleList(int SelectRange, int id)
        {
            // CardData.CardList[id].CardFirstAbility(guestPlayer, hostPlayer, SelectRange);
            guestBattleList.Add(new Tuple<int, int>(SelectRange, id));
        }

        public void TurnEndButton()
        {
            turnEndButton.TurnEnd();
            _turnEnd = true;
        }

        private void TextOn(string text)
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(Co_TextOn(text));
        }

        IEnumerator Co_TextOn(string text)
        {
            Text uiText = textPanel.transform.GetChild(1).GetChild(0).GetComponent<Text>();

            textPanel.SetActive(true);
            uiText.text = "";
            isTyping = true;
            yield return null;

            for (int i = 0; i < text.Length; i++)
            {
                if (isSkipText) break;

                uiText.text += text[i];
                yield return new WaitForSeconds(0.05f);
            }

            uiText.text = text;
            isTyping = false;
            isSkipText = false;
            yield return null;
        }

        private void TextOff()
        {
            textPanel.SetActive(false);
        }

        public void Update()
        {
            switch (_turn)
            {
                case Turn.FirstTurn:
                    OnFirstTurn();
                    break;
                case Turn.FirstTurnCutScene:
                    OnFirstTurnCutScene();
                    break;
                case Turn.FirstTurnEnd:
                    OnFirstTurnEnd();
                    break;
                case Turn.SecondTurn:
                    OnSecondTurn();
                    break;
                case Turn.SecondTurnCutScene:
                    OnSecondTurnCutScene();
                    break;
                case Turn.SecondTurnEnd:
                    OnSecondTurnEnd();
                    break;
                case Turn.ThirdTurn:
                    OnThirdTurn();
                    break;
                case Turn.ThirdTurnCutScene:
                    OnThirdTurnCutScene();
                    break;
                case Turn.ThirdTurnEnd:
                    OnThirdTurnEnd();
                    break;
                case Turn.ForthTurn:
                    OnForthTurn();
                    break;
                case Turn.ForthTurnCutScene:
                    OnForthTurnCutScene();
                    break;
                case Turn.ForthTurnEnd:
                    OnForthTurnEnd();
                    break;
            }
        }

        private void OnFirstTurn()
        {
            TutorialCardManager.Instance.AddCard(1);
            _turn = Turn.FirstTurnCutScene;

            textCount = 5;
            TextOn("좋아 그럼 가기전에 연습을 해볼까?");
        }

        private void OnFirstTurnCutScene()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    isSkipText = true;
                }

                else
                {
                    textCount--;

                    switch (textCount)
                    {
                        case 4:
                            TextOn("체력은 한 칸에 5이고 총 20이야! 0이 되면 패배해!마나는 한 칸에 1이고 총 5이야! 스킬을 쓰면 1씩 줄어");
                            break;
                        case 3:
                            TextOn("카드는 처음 3장을 받고 그 다음엔 4장이 될 때 까지 매 턴 뽑을 수 있어! ");
                            break;
                        case 2:
                            TextOn("카드를 드래그해서 상대 진영이나 자기 진영에 놓으면 카드를 사용할 수 있어!우선 “마법타격“ 부터 해보자!");
                            break;
                        case 1:
                            TextOn("아 내 지시대로 해준 다음엔 턴 종료 누르는 걸 잊지마!");
                            break;
                    }

                    if (textCount <= 0)
                    {
                        TextOff();
                        _turn = Turn.FirstTurnEnd;
                    }
                }
            }
        }

        private void OnFirstTurnEnd()
        {
            if (_turnEnd)
            {
                _turn = Turn.SecondTurn;
                _turnEnd = false;
                AIHpBar.SliderValue(0.75f);
                TutorialEffectManager.Instance.InitEffect(_player.gameObject, _ai.gameObject, 2, 1);
            }
        }

        private void OnSecondTurn()
        {
            theTime += Time.deltaTime;
            if (theTime < 3) return;

            TutorialCardManager.Instance.AddCard(6);
            turnEndButton.TurnStart();
            _turn = Turn.SecondTurnCutScene;
            theTime = 0;

            textCount = 4;
            TextOn("우왓, 방어해 버렸네..");
        }

        private void OnSecondTurnCutScene()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    isSkipText = true;
                }

                else
                {
                    textCount--;

                    switch (textCount)
                    {
                        case 3:
                            TextOn("그럼 나도 한번 예측해서 막아보자!");
                            break;
                        case 2:
                            TextOn("“폭발마법” 을 쓸 것 같은데…");
                            break;
                        case 1:
                            TextOn("그럼 “방폭마법“ 을 사용해보자!내가 있는 곳에 드래그해서 사용해줘!");
                            break;
                    }

                    if (textCount <= 0)
                    {
                        TextOff();
                        _turn = Turn.SecondTurnEnd;
                    }
                }
            }
        }

        private void OnSecondTurnEnd()
        {
            if (_turnEnd)
            {
                _turn = Turn.ThirdTurn;
                _turnEnd = false;
                hostBattleList.Add(new Tuple<int, int>(1, 1));
                TutorialEffectManager.Instance.InitEffect(_ai.gameObject, _player.gameObject, 5, 3);
                TutorialEffectManager.Instance.InitEffect(_ai.gameObject, _player.gameObject, 5, 6);
            }
        }

        private void OnThirdTurn()
        {
            theTime += Time.deltaTime;
            if (theTime < 3) return;

            TutorialCardManager.Instance.AddCard(8);
            turnEndButton.TurnStart();
            _turn = Turn.ThirdTurnCutScene;
            theTime = 0;

            textCount = 7;
            TextOn("이런 “마법타격“ 이였네..");
        }

        private void OnThirdTurnCutScene()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    isSkipText = true;
                }

                else
                {
                    textCount--;

                    switch (textCount)
                    {
                        case 6:
                            TextOn("각 방어카드는 발동하는 조건이 달라!");
                            break;
                        case 5:
                            TextOn("지금부터는 신중히 써보자..");
                            break;
                        case 4:
                            TextOn("일단 체력을 회복하자!“회복주문” 을 드래그해서 사용해줘!");
                            break;
                        case 3:
                            TextOn("아 그리고 말 안 했지만");
                            break;
                        case 2:
                            TextOn("왼쪽 아래의 이동버튼을 누르면위치를 바꿀 수 있어!");
                            break;
                        case 1:
                            TextOn("날 좌측으로 옮겨봐");
                            break;
                    }

                    if (textCount <= 0)
                    {
                        TextOff();
                        _turn = Turn.ThirdTurnEnd;
                    }
                }
            }
        }

        private void OnThirdTurnEnd()
        {
            if (_turnEnd)
            {
                _turn = Turn.ForthTurn;
                _turnEnd = false;
            }
        }

        private void OnForthTurn()
        {
            theTime += Time.deltaTime;
            if (theTime < 3) return;

            turnEndButton.TurnStart();
            _turn = Turn.ForthTurnCutScene;
            theTime = 0;

            textCount = 2;
            TextOn("음! 이제부턴 문제 없겠어!");
        }

        private void OnForthTurnCutScene()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (isTyping)
                {
                    isSkipText = true;
                }

                else
                {
                    textCount--;

                    switch (textCount)
                    {
                        case 1:
                            TextOn("자 그럼 본격적으로 시작해보자!");
                            break;
                    }

                    if (textCount <= 0)
                    {
                        TextOff();
                        _turn = Turn.ForthTurnEnd;
                    }
                }
            }
        }

        private void OnForthTurnEnd()
        {
            _loadingPanel.Close("IntroScene");
            // SceneManager.LoadScene("IntroScene");
        }
    }
}