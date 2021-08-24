using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    public enum GameState
    {
        GameSetup,
        GameStart,
        OnTurn,
        TurnEnd,
    }

    GameState gameState;

    void Start()
    {
        
    }

    void Update()
    {
        switch (gameState)
        {
            case GameState.GameSetup:
                gameState = GameState.GameStart;
                break;

            case GameState.GameStart:
                gameState = GameState.OnTurn;
                break;
            
            case GameState.OnTurn:
                break;
            
            case GameState.TurnEnd:
                break;
        }
    }
}
