using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class getStagePos : MonoBehaviour
{
    GameManager gm;
    [SerializeField] int x, z;
    [SerializeField] GameObject debugObj;
    public Vector3[,] data;
    Vector3 pivot;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        pivot = new Vector3(-1, 1, 1) + transform.position;
        data = new Vector3[x-1,z-1];

        for (int i = 0; i < x - 1; i++)
        {
            for (int j = 0; j < z - 1; j++)
            {
                data[i, j] = pivot + new Vector3(i * -1, 0, j * 1);
                if (debugObj != null)
                    Instantiate(debugObj, data[i, j], Quaternion.identity);
            }
        }

        gm._GameState.Value = GameState.StageSettingComp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3[,] get_data()
    {
        return this.data;
    }

    public int get_x_length()
    {
        return data.GetLength(0);
    }

    public int get_y_length()
    {
        return data.GetLength(1);
    }
}
