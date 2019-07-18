using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room_Indicater : MonoBehaviour
{
    [SerializeField] GameObject node;
    [SerializeField] GameObject content;

    List<GameObject> nodes;
    // Start is called before the first frame update
    void Start()
    {
        nodes = new List<GameObject>();
    }

    public void SetNode(string roomName,int cnt)
    {
        GameObject n = Instantiate(node);
        n.transform.SetParent(content.transform);
        n.transform.localScale = new Vector3(1, 1, 1);
        node_Script script = n.GetComponent<node_Script>();
        script.setText(roomName);
        script.setFill(cnt);
        nodes.Add(n);
    }

    public void Clear()
    {
        foreach(GameObject n in nodes)
        {
            Destroy(n);
        }
        nodes.Clear();
    }
}
