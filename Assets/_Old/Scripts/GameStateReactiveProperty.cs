using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UniRx;

[System.Serializable]
public class GameStateReactiveProperty : ReactiveProperty<GameState>
{

    public GameStateReactiveProperty() { }

    public GameStateReactiveProperty(GameState initialValue) : base(initialValue) { }

}

public enum GameState
{
    None,

    RoomSerching,

    CheckRoomData,

    WaitingOtherPlayer,

    RoomDataUpdate,

    RoomSettingComp,

    //仮
    Menu,

    //以下game
    StageSettingComp,

    ObstacleSettingComp,

    ReadyEventStart,

    TurnEventComp,

    GameStart,

    MoveObstacles,

    ChangeTurn,

    GameEnd,

    ResultEventComp,

    //以上game

    DefaultObstacleSetting,

    PlayerSetting,

    Playing,

    BackToMenu

}