using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Collections.Specialized;

public class player_manager : MonoBehaviour
{
    SocketObject so;
    //ゲームマネージャー
    GameManager gm;
    //ステージ
    public GameObject stage_manager;
    Vector3[,] masu_real_point;
    int masu_x_number;
    int masu_y_number;
    //プレイヤ
    public GameObject player_prefab;
    public OrderedDictionary members = new OrderedDictionary();　//id情報と名前*
    public Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();//idとプレイヤーの対応辞書*
    Dictionary<string, bool> is_players_dead = new Dictionary<string, bool>(); //idと死んだかどうかの対応表
    int player_number;//今回の参加プレイヤー数
    //ターン
    public string[] players_id_turn_info;//ターンの情報*
    public int turn_number;//現在のターンの場所*
    //サーバから位置情報を受け取る
    public Dictionary<string,Vector3> receive_position;
    public Dictionary<string, Vector3> receive_rotation;

    [SerializeField] GameObject TurnIndicater;
    GameObject TurnIndicater_Inscance;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        so = SocketObject.Instance;

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.StageSettingComp)
            .Subscribe(_ => set_player());

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ChangeTurn)
            .Subscribe(_ => change_turn());

        receive_position = new Dictionary<string, Vector3>();
        receive_rotation = new Dictionary<string, Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (receive_position.Count > 0)
        {
            foreach (KeyValuePair<string, Vector3> pos in receive_position)
            {
                string receive_id = pos.Key;
                Vector3 rot = receive_rotation[receive_id];
                if (players.ContainsKey(receive_id) && !receive_id.Equals(so.id))
                {
                    Debug.Log(receive_id + ":" + pos.Value);
                    //*サーバから位置と向きを受け取ったら更新する
                    GameObject player = players[receive_id];
                    player_function player_component = player.GetComponent<player_function>();
                    player_component.set_pos_rot(pos.Value, rot);
                }
            }
            receive_position.Clear();
            receive_rotation.Clear();
        }
    }

    public void set_player()
    {
        members = GetComponent<room_Matching>().myRoom.member;
        players_id_turn_info = new string[members.Count];
        int count = 0;
        foreach (DictionaryEntry pair in members)
        {
            players_id_turn_info[count] = pair.Key.ToString();
            is_players_dead[pair.Key.ToString()] = false;
            count++;
        }
        players = new Dictionary<string, GameObject>();
        //playerの初期配置
        //ステージ
        getStagePos stage_component = stage_manager.GetComponent<getStagePos>();
        masu_real_point = stage_component.get_data();
        masu_x_number = stage_component.get_x_length()-1;
        masu_y_number = stage_component.get_y_length()-1;
        //今回のゲームのプレイヤー数を調べる
        player_number = players_id_turn_info.Length;
        //誰からスタートするかの抽選
        turn_number = 0;// Random.Range(0, player_number);
        for (int i = 0; i < player_number; i++)
        {
            int spawn_masu_x = 0, spawn_masu_y = 0;
            if(i == 0)
            {
                spawn_masu_x = 0;
                spawn_masu_y = 0;
            }else if(i == 1)
            {
                spawn_masu_x = masu_x_number;
                spawn_masu_y = 0;
            }else if(i == 2)
            {
                spawn_masu_x = masu_x_number;
                spawn_masu_y = masu_y_number;
            }else if(i == 3)
            {
                spawn_masu_x = 0;
                spawn_masu_y = masu_y_number;
            }
            GameObject player = (GameObject)Instantiate(player_prefab, masu_real_point[spawn_masu_x, spawn_masu_y], Quaternion.identity);
            if(turn_number == i)
            {
                player.GetComponent<player_function>().set_my_turn(true);
                TurnIndicater_Inscance = (GameObject)Instantiate(TurnIndicater, player.transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
            if(players_id_turn_info[i] == so.id)
            {
                player.GetComponent<player_function>().set_is_enemy(false);
            }
            players.Add(players_id_turn_info[i], player);
        }

        //なんかアニメーションをやる？どういう順番で進行していくかとかのUIができたらいいよね。
        gm._GameState.Value = GameState.PlayerSettingComp;
    }

    public void change_turn()
    {
        turn_number += 1;
        turn_number = turn_number % player_number;
        GameObject player = players[players_id_turn_info[turn_number]];
        TurnIndicater_Inscance.transform.position = player.transform.position + new Vector3(0, 1, 0);
        player.GetComponent<player_function>().set_my_turn(true);
        gm._GameState.Value = GameState.GameStart;
        Debug.Log("TURNCHANGE:"+players_id_turn_info[turn_number]);
    }

    public void set_is_dead_player(string id)
    {
        is_players_dead[id] = true;
        players[id].GetComponent<player_function>().set_is_dead(true);

        //もし全てのプレイヤーが死んでしまったら
        if (!is_players_dead.ContainsValue(false))
        {
            //draw演出？
            gm._GameState.Value = GameState.DrawEventStart;
        }
    }
}
