using UnityEngine;

namespace OffLine
{
    public class OffIngameManager : MonoBehaviour
    {
        enum GameState
        {
            PlayerTurn,
            EnemyTurn,
            GameEnd
        }

        private int _curStage;
        private int _curTurn;
    }
}