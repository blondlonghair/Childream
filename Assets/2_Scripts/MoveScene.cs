using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CurScene
{
    IntroScene,
    LoadingScene,
    LobbyScene,
    InGameScene
}

public class MoveScene : MonoBehaviourPunCallbacks
{
    public enum CurScene
    {
        IntroScene,
        LoadingScene,
        LobbyScene,
        InGameScene
    }
    
    public static MoveScene Instance;
    
    private void Start()
    {
        Instance = this;
    }

    public void fdsaScene(CurScene _curScene, bool _leaveRoom)
    {
        switch (_curScene)
        {
            case CurScene.IntroScene:
                SceneManager.LoadScene("IntroScene");
                PhotonNetwork.LeaveRoom();
                break;
            case CurScene.LobbyScene:
                break;
            case CurScene.InGameScene:
                break;
        }
    }
}
