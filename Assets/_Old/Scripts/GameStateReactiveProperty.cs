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

    DefaultObstacleSetting,

    PlayerSetting,

    Playing,

    BackToMenu

}