using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle_function : MonoBehaviour
{
    /// <summary>
    ///・自分の状態が分かる(動いているのか、動かせないのか)
    ///・動くことができる
    /// </summary>

    public float speed;

    bool is_moving;
    Vector3 target_position;
    int masu_x, masu_y;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //もし動く指示があったら
        if (is_moving)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target_position, step);
            if (Vector3.Distance(transform.position, target_position) < 0.01f)
            {
                transform.position = target_position;
                this.set_is_moving(false);
            }
        }
    }

    public Vector3 get_target_position()
    {
        return target_position;
    }

    public void set_target_position(Vector3 target_position)
    {
        this.target_position = target_position;
    }

    public bool get_is_moving()
    {
        return is_moving;
    }

    public void set_is_moving(bool is_moving)
    {
        this.is_moving = is_moving;
    }

    public float get_speed()
    {
        return speed;
    }

    public void set_speed(float speed)
    {
        this.speed = speed;
    }

    public int get_masu_x()
    {
        return masu_x;
    }

    public int get_masu_y()
    {
        return masu_y;
    }

    public void set_masu(int x, int y)
    {
        this.masu_x = x;
        this.masu_y = y;
    }
}
