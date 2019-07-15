using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class player_manager : MonoBehaviour
{
    //ゲームマネージャー
    GameManager gm;
    //ステージ
    public GameObject stage_manager;
    Vector3[,] masu_real_point;
    int masu_x_number;
    int masu_y_number;
    //ゴール
    public GameObject goal_flug;
    //プレイヤ
    public GameObject player_prefab;
    Dictionary<string, GameObject> players;
    //ターン
    int turn_number;

    //virtual
    int player_number;
    //サーバから位置情報を受け取る
    string receive_id;
    Vector3 receive_position;
    Quaternion receive_rotation;
    //これのindex順でターンが進行するので、ランダムな順番にできるかも？
    string[] players_id = {"a","b","c","d"};
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ObstacleSettingComp)
            .Subscribe(_ => set_player());
    }

    // Update is called once per frame
    void Update()
    {
        //*サーバから位置と向きを受け取ったら更新する
        GameObject player = players[receive_id];
        player_function player_component = player.GetComponent<player_function>();
        player_component.set_pos_rot(receive_position, receive_rotation);

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.ChangeTurn)
            .Subscribe(_ => change_turn());
    }

    public void set_player()
    {
        //playerの初期配置
        //ステージ
        getStagePos stage_component = stage_manager.GetComponent<getStagePos>();
        masu_real_point = stage_component.get_data();
        masu_x_number = stage_component.get_x_length();
        masu_y_number = stage_component.get_y_length();
        //今回のゲームのプレイヤー数を調べる
        player_number = players_id.Length;
        //誰からスタートするかの抽選
        turn_number = Random.Range(0, player_number);
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
            player.transform.LookAt(goal_flug.transform);
            if(turn_number == i)
            {
                player.GetComponent<player_function>().set_my_turn(true);
            }
            players.Add(players_id[i], player);
        }

        //なんかアニメーションをやる？どういう順番で進行していくかとかのUIができたらいいよね。
        gm._GameState.Value = GameState.ReadyEventStart;
    }

    void change_turn()
    {
        turn_number += 1;
        turn_number = turn_number % player_number;
        GameObject player = players[players_id[turn_number]];
        player.GetComponent<player_function>().set_my_turn(true);
        gm._GameState.Value = GameState.GameStart;
    }
}
