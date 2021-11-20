namespace Enums
{
    public enum CardType
    {
        NONE,
        ATK,
        DEF,
        SUP
    }

    public enum TargetType
    {
        NONE,
        ME,
        ENEMY,
        ALL
    }

    public enum ActType
    {
        NONE,
        CHANGECARD,
        HEAL,
        LOCKGRID
    }
    
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
}
