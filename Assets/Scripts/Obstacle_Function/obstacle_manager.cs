﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class obstacle_manager : MonoBehaviour
{
    /// <summary>
    ///・初期位置の情報(マス目)を決定し配置する。以後それらの障害物を監視する*
    ///・サーバから攻撃された障害物の情報(座標と殴られた向き)を受け取る
    ///・動く障害物を検索して、どこまで動くのかを判定する*
    ///・障害物にターゲット座標を渡し、フラグを立てる*
    ///・リストの位置情報を更新する*
    /// </summary>
 
    //ゲームマネージャー
    GameManager gm;
    //outside
    public GameObject obstacle_prefab;
    public GameObject stage_manager;
    Vector3[,] masu_real_point;
    int masu_x_number;
    int masu_y_number;

    //inside
    GameObject[,] obs_list;
    public int set_obstacle_frequency = 3;

    //virtual
    int receive_masu_x;
    int receive_masu_y;
    string receive_attack_direction;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.StageSettingComp)
            .Subscribe(_ => set_obstacle());//処理

    }

    // Update is called once per frame
    void Update()
    {
        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.MoveObstacles)
            .Subscribe(_ => move_obstacles());
    }


    //ここ誰かがまず決めてそれを他のユーザに配る感じにしなくてはダメな感じですかね？
    void set_obstacle()
    {
        getStagePos stage_component = stage_manager.GetComponent<getStagePos>();
        masu_real_point = stage_component.get_data();
        masu_x_number = stage_component.get_x_length();
        masu_y_number = stage_component.get_y_length();
        obs_list = new GameObject[masu_x_number, masu_y_number];
        //マス座標を決定して、それに対応する実座標にobstacleを生成する。
        //生成したマス座標をkeyとして、それに対応する生成したobstacleと一対一対応するkey-valueを作る
        for (int masu_x = 0; masu_x < masu_x_number; masu_x++)
        {
            for (int masu_y = 0; masu_y < masu_y_number; masu_y++)
            {
                GameObject obstacle = null;
                int set_random_obstacle = Random.Range(0, set_obstacle_frequency);
                if (set_random_obstacle == 0)
                {
                    obstacle = (GameObject)Instantiate(obstacle_prefab, masu_real_point[masu_x, masu_y], Quaternion.identity);
                    obstacle_function obs_component = obstacle.GetComponent<obstacle_function>();
                    obs_component.set_masu(masu_x, masu_y);
                }
                obs_list[masu_x, masu_y] = obstacle;
            }
        }

        gm._GameState.Value = GameState.ObstacleSettingComp;
    }

    void move_obstacles()
    {
        //*server
        //マスの情報が送られてきたら
        List<GameObject> moving_obs_list = new List<GameObject>();
        bool is_null_zone = false;
        int distance = 0;
        bool is_search = true, is_move_end = false;

        //0 < server_masu_x < masu_x_number
        //0 < server_masu_y < masu_y_number
        if (is_search)
        {
            if (receive_attack_direction.Equals("Up"))
            {
                //x固定、y負方向
                for (int i = receive_masu_y; i >= 0; i--)
                {
                    if (obs_list[receive_masu_x, i] != null)
                    {
                        if (is_null_zone)
                        {
                            break;
                        }
                        else
                        {
                            moving_obs_list.Add(obs_list[receive_masu_x, i]);
                            obs_list[receive_masu_x, i] = null;
                        }
                    }
                    else
                    {
                        distance += 1;
                        is_null_zone = true;
                    }
                }

                foreach (GameObject obs in moving_obs_list)
                {
                    obstacle_function obs_component = obs.GetComponent<obstacle_function>();
                    obs_component.set_masu(receive_masu_x, obs_component.get_masu_y() - distance);
                    obs_component.set_target_position(masu_real_point[receive_masu_x, obs_component.get_masu_y() - distance]);
                    obs_list[receive_masu_x, obs_component.get_masu_y() - distance] = obs;
                    obs_component.set_is_moving(true);
                }
                is_search = false;
            }
            else if (receive_attack_direction.Equals("Down"))
            {
                //x固定、y正方向
                for (int i = receive_masu_y; i <= masu_y_number; i++)
                {
                    if (obs_list[receive_masu_x, i] != null)
                    {
                        if (is_null_zone)
                        {
                            break;
                        }
                        else
                        {
                            moving_obs_list.Add(obs_list[receive_masu_x, i]);
                            obs_list[receive_masu_x, i] = null;
                        }
                    }
                    else
                    {
                        distance += 1;
                        is_null_zone = true;
                    }
                }

                foreach (GameObject obs in moving_obs_list)
                {
                    obstacle_function obs_component = obs.GetComponent<obstacle_function>();
                    obs_component.set_masu(receive_masu_x, obs_component.get_masu_y() + distance);
                    obs_component.set_target_position(masu_real_point[receive_masu_x, obs_component.get_masu_y() + distance]);
                    obs_list[receive_masu_x, obs_component.get_masu_y() + distance] = obs;
                    obs_component.set_is_moving(true);
                }
                is_search = false;
            }
            else if (receive_attack_direction.Equals("Right"))
            {
                //y固定、x正方向
                for (int i = receive_masu_x; i <= masu_x_number; i++)
                {
                    if (obs_list[i, receive_masu_y] != null)
                    {
                        if (is_null_zone)
                        {
                            break;
                        }
                        else
                        {
                            moving_obs_list.Add(obs_list[i, receive_masu_y]);
                            obs_list[i, receive_masu_y] = null;
                        }
                    }
                    else
                    {
                        distance += 1;
                        is_null_zone = true;
                    }
                }

                foreach (GameObject obs in moving_obs_list)
                {
                    obstacle_function obs_component = obs.GetComponent<obstacle_function>();
                    obs_component.set_masu(obs_component.get_masu_x() + distance, receive_masu_y);
                    obs_component.set_target_position(masu_real_point[obs_component.get_masu_x() + distance, receive_masu_y]);
                    obs_list[obs_component.get_masu_x() + distance, receive_masu_y] = obs;
                    obs_component.set_is_moving(true);
                }
                is_search = false;
            }
            else if (receive_attack_direction.Equals("Left"))
            {
                //y固定、x負方向
                for (int i = receive_masu_x; i >= 0; i--)
                {
                    if (obs_list[i, receive_masu_y] != null)
                    {
                        if (is_null_zone)
                        {
                            break;
                        }
                        else
                        {
                            moving_obs_list.Add(obs_list[i, receive_masu_y]);
                            obs_list[i, receive_masu_y] = null;
                        }
                    }
                    else
                    {
                        distance += 1;
                        is_null_zone = true;
                    }
                }
                foreach (GameObject obs in moving_obs_list)
                {
                    obstacle_function obs_component = obs.GetComponent<obstacle_function>();
                    obs_component.set_masu(obs_component.get_masu_x() - distance, receive_masu_y);
                    obs_component.set_target_position(masu_real_point[obs_component.get_masu_x() - distance, receive_masu_y]);
                    obs_list[obs_component.get_masu_x() - distance, receive_masu_y] = obs;
                    obs_component.set_is_moving(true);
                }
                is_search = false;
            }
        }
        else
        {
            //直接殴ったobstacleのis_movingがfalsuになったなら全体の移動が終わったということ
            if (!moving_obs_list[0].GetComponent<obstacle_function>().get_is_moving())
            {
                is_move_end = true;
            }

            //obstacleの移動が全て完了したら次のターンになる
            if (is_move_end)
            {
                gm._GameState.Value = GameState.ChangeTurn;
            }
        }
    }
}