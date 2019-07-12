using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle_manager : MonoBehaviour
{
    /// <summary>
    ///・初期位置の情報(マス目)を決定し配置する。以後それらの障害物を監視する*
    ///・サーバから攻撃された障害物の情報(座標と殴られた向き)を受け取る
    ///・動く障害物を検索して、どこまで動くのかを判定する*
    ///・障害物にターゲット座標を渡し、フラグを立てる*
    ///・リストの位置情報を更新する*
    /// </summary>

    public Vector3[][] masu_real_point;
    GameObject[][] obs_list;
    public GameObject obstacle_prefab;
    int masu_x_number = 3;
    int masu_y_number = 3;
    // Start is called before the first frame update
    void Start()
    {
        //マス座標と固定かどうかを決定して、それに対応する実座標にobstacleを生成する。
        //生成したマス座標をkeyとして、それに対応する生成したobstacleと一対一対応するkey-valueを作る
        for(int masu_x = 0; masu_x < masu_x_number; masu_x++)
        {
            for(int masu_y = 0; masu_y < masu_y_number; masu_y++)
            {
                GameObject obstacle = null;
                int set_random_obstacle = Random.Range(0, 3);
                if(set_random_obstacle == 0)
                {
                    obstacle = (GameObject)Instantiate(obstacle_prefab, masu_real_point[masu_x][masu_y], Quaternion.identity);
                    obstacle_function obs_component = obstacle.GetComponent<obstacle_function>();
                    obs_component.set_masu(masu_x, masu_y);
                }
                obs_list[masu_x][masu_y] = obstacle;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        //*server
        //マスの情報が送られてきたら
        int server_masu_x = 1;
        int server_masu_y = 4;
        string attack_direction = "Down";
        List<GameObject> moving_obs_list = new List<GameObject>();
        bool is_null_zone = false;
        int distance = 0;

        //0 < server_masu_x < masu_x_number
        //0 < server_masu_y < masu_y_number
        if (attack_direction.Equals("Up"))
        {
            //x固定、y負方向
            for (int i = server_masu_y; i >= 0; i--)
            {
                if (obs_list[server_masu_x][i] != null)
                {
                    if (is_null_zone)
                    {
                        break;
                    }
                    else
                    {
                        moving_obs_list.Add(obs_list[server_masu_x][i]);
                        obs_list[server_masu_x][i] = null;
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
                obs_component.set_masu(server_masu_x, obs_component.get_masu_y() - distance);
                obs_component.set_target_position(masu_real_point[server_masu_x][obs_component.get_masu_y() - distance]);
                obs_list[server_masu_x][obs_component.get_masu_y() - distance] = obs;
                obs_component.set_is_moving(true);
            }
        }
        else if (attack_direction.Equals("Down"))
        {
            //x固定、y正方向
            for (int i = server_masu_y; i <= masu_y_number; i++)
            {
                if (obs_list[server_masu_x][i] != null)
                {
                    if (is_null_zone)
                    {
                        break;
                    }
                    else
                    {
                        moving_obs_list.Add(obs_list[server_masu_x][i]);
                        obs_list[server_masu_x][i] = null;
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
                obs_component.set_masu(server_masu_x, obs_component.get_masu_y() + distance);
                obs_component.set_target_position(masu_real_point[server_masu_x][obs_component.get_masu_y() + distance]);
                obs_list[server_masu_x][obs_component.get_masu_y() + distance] = obs;
                obs_component.set_is_moving(true);
            }
        }
        else if (attack_direction.Equals("Right"))
        {
            //y固定、x正方向
            for (int i = server_masu_x; i <= masu_x_number; i++)
            {
                if (obs_list[i][server_masu_y] != null)
                {
                    if (is_null_zone)
                    {
                        break;
                    }
                    else
                    {
                        moving_obs_list.Add(obs_list[i][server_masu_y]);
                        obs_list[i][server_masu_y] = null;
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
                obs_component.set_masu(obs_component.get_masu_x() + distance, server_masu_y);
                obs_component.set_target_position(masu_real_point[obs_component.get_masu_x() + distance][server_masu_y]);
                obs_list[obs_component.get_masu_x() + distance][server_masu_y] = obs;
                obs_component.set_is_moving(true);
            }
        }
        else if (attack_direction.Equals("Left")){
            //y固定、x負方向
            for (int i = server_masu_x; i >= 0; i--)
            {
                if (obs_list[i][server_masu_y] != null)
                {
                    if (is_null_zone)
                    {
                        break;
                    }
                    else
                    {
                        moving_obs_list.Add(obs_list[i][server_masu_y]);
                        obs_list[i][server_masu_y] = null;
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
                obs_component.set_masu(obs_component.get_masu_x() - distance, server_masu_y);
                obs_component.set_target_position(masu_real_point[obs_component.get_masu_x() - distance][server_masu_y]);
                obs_list[obs_component.get_masu_x() - distance][server_masu_y] = obs;
                obs_component.set_is_moving(true);
            }
        } 
    }
}
