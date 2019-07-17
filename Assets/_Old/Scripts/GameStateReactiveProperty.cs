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

    Menu,

    CreateRoom,

    GetRoomData,

    RoomSerching,

    JoinRoom,

    WaitingOtherPlayer,

    RoomDataUpdate,

    RoomSettingComp,

    //以下game
    //ステージの情報を取得
    StageSettingComp,
    //プレイヤーを生成
    PlayerSettingComp,
    //障害物を配置
    ReadyEventStart,
    //ゲーム開始演出
    GameStart,
    //ゲームプレイ中
    MoveObstacles,
    //障害物が動く
    ChangeTurn,
    //次の人にターンを移す
    //全員死亡、制限時間までにゴール出来なかったら=Draw。誰かがゴールにたどり着いたら=Win。
    DrawEventStart,
    WinEventStart,
    //ゲーム終了演出
    GameEnd,
    //playerとobstacleを全て破壊して、Menuへ。
    //以上game

    DefaultObstacleSetting,

    PlayerSetting,

    Playing,

    BackToMenu

}