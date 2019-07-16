using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getStagePos : MonoBehaviour
{
    [SerializeField] int x, z;
    [SerializeField] GameObject debugObj;
    public Vector3[,] data;
    Vector3 pivot;
    // Start is called before the first frame update
    void Start()
    {
        pivot = new Vector3(-1, 1, 1) + transform.position;
        data = new Vector3[x-1,z-1];
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<x-1; i++)
        {
            for(int j=0; j<z-1; j++)
            {
                data[i,j] = pivot + new Vector3(i * -1, 0, j * 1);
                if(debugObj!=null)
                    Instantiate(debugObj, data[i,j], Quaternion.identity);
            }
        }
    }

    public Vector3 getPos(int x,int z)
    {
        return data[x, z];
    }
}
