using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UniRx;

public class room_Matching : MonoBehaviour
{
    SocketObject so;
    DataWorker dw;
    GameManager gm;

    [SerializeField] GameObject content, pref;

    public string trgRoomName;

    //List<Room> room = new List<Room>();

    // Use this for initialization
    void Start()
    {
        so = SocketObject.Instance;
        dw = DataWorker.Instance;
        gm = GameManager.Instance;
        StartCoroutine("RequestRoomData");

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.WaitingOtherPlayer)
            .Subscribe(_ => Debug.Log("ルームメンバー募集中..."));

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.CheckRoomData)
            .Subscribe(_ => JoinRoom());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomDataUpdate)
            .Subscribe(_ => RoomDataCheck());
    }

    // Update is called once per frame
    void Update()
    {
        //QuickStart ();
    }

    void JoinRoom()
    {
        var state = dw.roomState;
        dw.roomState = null;

        //入室可能ルームが見つかったかどうか
        bool hit = false;
        for (int i = 0; i < state.list.Count; i++)
        {
            if (!so.id.Equals(state.keys[i]))
            {
                string roomName = state.keys[i].ToString();
                int cnt = int.Parse(state.list[i]["length"].ToString());
                //if (roomName.Contains("[ROOM]"))
                if (roomName.Equals(trgRoomName))
                {
                    Debug.Log(roomName);
                    if (dw.MAX > cnt)
                    {
                        if (state.list[i]["playing"].ToString().Equals("false"))
                        {
                            Debug.Log("Room:[" + roomName + "] " + cnt + "/" + dw.MAX);
                            //ルーム入室リクエスト送信（未確定）
                            var data = new Dictionary<string, string>();
                            data["to"] = roomName;
                            data["name"] = so.name;
                            data["max"] = "4";
                            so.EmitMessage("Quick", data);
                            hit = true;
                            break;
                        }
                    }
                }
            }
        }
        //もし入室可能な部屋が見つからなかったら新しい部屋を作る
        if (!hit)
        {
            Debug.Log("ルームがいっぱいでした。");
        }

        gm._GameState.Value = GameState.WaitingOtherPlayer;

    }

    void CreateNewRoom(String roomNane)
    {
        var data = new Dictionary<string, string>();
        data["to"] = "JOIN";
        data["name"] = so.name;
        //Debug.Log(dw.MAX);
        data["max"] = "4";
        so.EmitMessage("Quick", data);
    }

    void RoomDataCheck()
    {
        Debug.Log("check");
        if (dw.myRoom.cnt == 4)
        {
            Debug.Log("comp");
            gm._GameState.Value = GameState.RoomSettingComp;
        }
        else
        {
            Debug.Log("wait");
            gm._GameState.Value = GameState.WaitingOtherPlayer;
        }
    }

    IEnumerator RequestRoomData()
    {
        while (true)
        {
            if (gm._GameState.Value == GameState.RoomSerching)
            {
                Debug.Log("ルーム検索中...");
                var data = new Dictionary<string, string>();
                data["to"] = "";
                so.EmitMessage("Quick", data);
            }
            yield return new WaitForSeconds(1f);
        }

    }
}

