using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class event_manager : MonoBehaviour
{
    GameManager gm;
    float time;
    public float remit_time;
    string winner_id;
    public Text info_text;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        this.info_text.text = "";
    }

    // Update is called once per frame
    void Update()
    { 

        //ready_event_startを受け取ったらゲームスタート直前のイベントを開始する。
        if(gm._GameState.Value == GameState.ReadyEventStart)
        {
            time += Time.deltaTime;
            if (time < 8.0f)
            {
                this.ready_event();
            }
            else
            {
                time = 0;

                Debug.Log("Go!");
                this.info_text.text = "Go!";
                gm._GameState.Value = GameState.GameStart;
                this.info_text.text = "";
            }
        }

        if (gm._GameState.Value == GameState.GameStart)
        {
            time += Time.deltaTime;
            if(time > remit_time)
            {
                time = 0;
                gm._GameState.Value = GameState.DrawEventStart;
            }
        }


        if(gm._GameState.Value == GameState.DrawEventStart)
        {
            time += Time.deltaTime;
            if(time < 5.0f)
            {
                this.draw_event();
            }
            else
            {
                this.info_text.text = "";
                gm._GameState.Value = GameState.GameEnd;
            }
        }

        if(gm._GameState.Value == GameState.WinEventStart)
        {
            time += Time.deltaTime;
            if(time < 5.0f)
            {
                this.win_event();
            }
            else
            {
                this.info_text.text = "";
                gm._GameState.Value = GameState.GameEnd;
            }
        }

        //ここをplayerdestroyとobstacledestroyの二つの状態遷移にするのもあり
        if(gm._GameState.Value == GameState.GameEnd)
        {
            this.game_end();
        }

    }

    public void ready_event() {
        //this.info_text = this.GetComponent<Text>();
        if(time < 5.0f)
        {
            this.info_text.text = "Start from " + GetComponent<player_manager>().members[GetComponent<player_manager>().players_id_turn_info[GetComponent<player_manager>().turn_number]] + "!";
        }else if (time < 6.0f)
        {
            this.info_text.text = "3";
        }else if (time < 7.0f)
        {
            this.info_text.text = "2";
        }else if(time < 8.0f)
        {
            this.info_text.text = "1";
        }

    }

    public void draw_event()
    {
        this.info_text.text = "Draw";
    }

    public void win_event()
    {
        this.info_text.text = GetComponent<player_manager>().members[winner_id] +  "is Win!";
    }

    //ここサーバーから叩かれる処理
    public void any_player_goal(string id)
    {
        this.winner_id = id;
        //idとplayersから勝利プレイヤーを取得。勝利モーションを起こさせる？
        gm._GameState.Value = GameState.WinEventStart;
    }

    public void game_end()
    {
        string[] players_id = GetComponent<player_manager>().players_id_turn_info;
        Dictionary<string, GameObject> players = GetComponent<player_manager>().players;
        GameObject[,] obs_list = GetComponent<obstacle_manager>().obs_list;

        for(int i = 0; i < players_id.Length; i++)
        {
            if(players[players_id[0]] != null)
            {
                Destroy(players[players_id[0]]);
            }
        }

        foreach(GameObject obstacle in obs_list)
        {
            if(obstacle != null)
            {
                Destroy(obstacle);
            }
        }

        gm._GameState.Value = GameState.Menu;
    }
}
