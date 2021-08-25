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

    public static GameManager Instance;

    void Awake() => Instance = this;

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
        gameState = GameState.GameStart;
        PhotonNetwork.Instantiate("Prefab/Player", Vector3.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("Prefab/Ranges", Vector3.zero, Quaternion.identity);
    }

    void OnGameStart()
    {
        CardManager.Instance.AddCard(true);
        CardManager.Instance.AddCard(true);
        CardManager.Instance.AddCard(true);

        gameState = GameState.OnTurn;
    }

    void OnTurn()
    {

    }

    void OnActCard()
    {

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
