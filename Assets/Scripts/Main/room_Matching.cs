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
    GameManager gm;

    [SerializeField] GameObject content, pref;

    public JSONObject roomState;

    public Room myRoom;

    //List<Room> room = new List<Room>();

    // Use this for initialization
    void Start()
    {
        so = SocketObject.Instance;
        gm = GameManager.Instance;
        StartCoroutine("RequestRoomData");

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.Menu)
            .Subscribe(_ => roomState=null);

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.GetRoomData)
            .Subscribe(_ => RoomDataIndicate());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.CreateRoom)
            .Subscribe(_ => CreateNewRoom(so.trgRoom));

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.WaitingOtherPlayer)
            .Subscribe(_ => WaitingOtherPlayer());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.JoinRoom)
            .Subscribe(_ => JoinRoom(so.trgRoom));

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
        //Debug.Log(roomState);
        var state = roomState;

        var roomindex = state.keys
            .Select((key, index) => index)
            .Where(index => !state.list[index]["sockets"].ToDictionary().First().Key.Equals(state.keys[index]))
            .Where(index => int.Parse(state.list[index]["length"].ToString()) < 4 && state.list[index]["playing"].ToString().Equals("false"));
        //Debug.Log(roomindex.Count());

        GetComponent<ui_Manager>().Menu03.GetComponent<room_Indicater>().Clear();
        
        foreach (int i in roomindex)
        {
            Debug.Log("ルーム表示");
            string name = state.keys[i];
            int cnt = int.Parse(state.list[i]["length"].ToString());
            GetComponent<ui_Manager>().Menu03.GetComponent<room_Indicater>().SetNode(name, cnt);
        }
    }

    void JoinRoom(string roomName)
    {
        if (roomName == null)
            Debug.Log("ルーム名が取得できません");

        var state = roomState;

        var roomindex = state.keys
            .Select((key, index) => index)
            .Where(index => !state.list[index]["sockets"].ToDictionary().First().Key.Equals(state.keys[index]))
            .Where(index => int.Parse(state.list[index]["length"].ToString()) < 4 && state.list[index]["playing"].ToString().Equals("false")).First();
        if (state.keys[roomindex] != null)
        {
            var data = new Dictionary<string, string>();
            data["to"] = "JOIN";
            data["room"] = state.keys[roomindex];
            data["name"] = so.name;
            data["max"] = "4";
            so.EmitMessage("RoomMatching", data);
        }
        else
        {
            Debug.Log("ルームがいっぱいでした。");
        }
        gm._GameState.Value = GameState.WaitingOtherPlayer;
    }

    void CreateNewRoom(String roomName)
    {
        if (roomName == null || roomName == "")
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
        GetComponent<ui_Manager>().PlayerClear();
        GetComponent<ui_Manager>().PlayerAdd(myRoom.member);

        Debug.Log("check");
        if (myRoom.cnt == 4)
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

    void WaitingOtherPlayer()
    {
        Debug.Log("ルームメンバー募集中...");
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
        so.trgRoom = WWW.EscapeURL(s);
    }
}

