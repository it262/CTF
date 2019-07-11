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

    public JSONObject roomState;

    public Room myRoom;

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
            .Where(x => x == GameState.GetRoomData)
            .Subscribe(_ => RoomDataIndicate());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.CreateRoom)
            .Subscribe(_ => CreateNewRoom(trgRoomName));

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.WaitingOtherPlayer)
            .Subscribe(_ => Debug.Log("ルームメンバー募集中..."));

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.CheckRoomData)
            .Subscribe(_ => JoinRoom(trgRoomName));

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

    void RoomDataIndicate()
    {

        Debug.Log(roomState);
        var state = roomState;

        var roomindex = state.keys
            //.Select((key, index) => new { Key = key, Index = index, Cnt = int.Parse(state.list[index]["length"].ToString()), isPlaying = state.list[index]["playing"].ToString().Equals("false") })
            .Select((key, index) => index)
            .Where(index => !state.keys[index].Equals(so.id) && int.Parse(state.list[index]["length"].ToString()) < 4 && state.list[index]["playing"].ToString().Equals("false"));

        GetComponent<ui_Manager>().Menu03.GetComponent<room_Indicater>().Clear();
        
        foreach (int i in roomindex)
        {
            string name = state.keys[i];
            int cnt = int.Parse(state.list[i]["length"].ToString());
            var member = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> d in state.list[i]["sockets"].ToDictionary())
            {
                member.Add(d.Key, d.Value);
            }
            GetComponent<ui_Manager>().Menu03.GetComponent<room_Indicater>().SetNode(name, cnt);
        }
    }

    void JoinRoom(string roomName)
    {
        if (roomName == null)
            Debug.Log("ルーム名が取得できません");

        var state = roomState;

        var s = state.keys
            .Select((key, index) => new { Key = key, Index = index, Cnt = int.Parse(state.list[index]["length"].ToString()), isPlaying = state.list[index]["playing"].ToString().Equals("false") })
            .Where(item => !item.Key.Equals(so.id) && item.Key.Equals(roomName) && item.Cnt < dw.MAX && item.isPlaying)
            .Select(item => item.Key).First();

        if (s != null)
        {
            var data = new Dictionary<string, string>();
            data["to"] = "JOIN";
            data["room"] = s;
            data["name"] = so.name;
            data["max"] = "4";
            so.EmitMessage("RoomMatching", data);
        }
        else
        {
            Debug.Log("ルームがいっぱいでした。");
        }
        gm._GameState.Value = GameState.WaitingOtherPlayer;

        /*
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
        */

    }

    void CreateNewRoom(String roomName)
    {
        if (roomName == null)
            Debug.Log("ルーム名が取得できません");

        var data = new Dictionary<string, string>();
        data["to"] = "CREATE";
        data["room"] = roomName;
        data["name"] = so.name;
        //Debug.Log(dw.MAX);
        data["max"] = "4";
        so.EmitMessage("RoomMatching", data);
        Debug.Log("Room:"+roomName+"を作成->");
    }

    void RoomDataCheck()
    {
        Debug.Log("check");
        if (myRoom.cnt == 4)
        {
            Debug.Log("comp");
            gm._GameState.Value = GameState.RoomSettingComp;
        }
        else
        {
            Debug.Log("wait");
            gm._GameState.Value = GameState.RoomSerching;
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
                so.EmitMessage("RoomMatching", data);
            }
            yield return new WaitForSeconds(1f);
        }

    }

    public void setRoomName(string s)
    {
        trgRoomName = s;
    }
}

