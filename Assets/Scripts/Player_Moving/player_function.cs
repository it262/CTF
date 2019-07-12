﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_function : MonoBehaviour
{
    /// <summary>
    ///*playerの機能
    ///・移動と回転ができる
    ///・どの方向がを向いているのかが分かる
    ///・自分のターンの間は攻撃できる
    ///・Goalしたかどうかが分かる
    ///・障害物に潰されたら死亡する
    /// </summary>

    public float move_speed;
    public float rotate_speed;
    public bool is_my_turn;
    public bool is_goal;
    public bool is_dead;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //移動と回転ができる
        //*位置と向きの情報
        Vector3 velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity.z += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0f, -rotate_speed, 0f);
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity.z -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0f, rotate_speed, 0f);
        }

        velocity = velocity.normalized * move_speed * Time.deltaTime;

        if (velocity.magnitude > 0)
        {
            transform.position += gameObject.transform.rotation * velocity;
        }
        else {
            //Don't Move!
        }

        //どの方向を向いているかが分かる
        if (45 <= transform.eulerAngles.y && transform.eulerAngles.y < 135)
        {
            Debug.Log("Right");
        }
        else if(135 <= transform.eulerAngles.y && transform.eulerAngles.y < 225)
        {
            Debug.Log("Down");
        }
        else if(225 <= transform.eulerAngles.y && transform.eulerAngles.y < 315)
        {
            Debug.Log("Left");
        }
        else
        {
            Debug.Log("Up");
        }

        //自分のターンの間は攻撃出来る
        if (is_my_turn)
        {
            Attack();
        }

    }

    //Rayで
    //*殴った相手と自分の向きの情報
    public void Attack()
    {
		//Any Scripts
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Goalしたかどうかが分かる
        //*自分がGoalしたという情報
        if(collision.gameObject.name == "Goal")
        {
            is_goal = true;
        }

        //障害物に潰されたら死亡する
        //*自分が死亡したという情報
        if(collision.gameObject.name == "Obstacle")
        {
            //if (collision.gameObject.get_is_moving())
            //{
            //    is_dead = true;
            //    Destroy(gameObject);
            //}
        }
    }

}