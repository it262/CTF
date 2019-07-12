using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class node_Script : MonoBehaviour
{
    GameManager gm;
    [SerializeField] GameObject fillBar;
    [SerializeField] Text text;

    public int fill;

    public int fillWidth;

    // Start is called before the first frame update
    void Start()
    {
        fill = 0;
        fillWidth = 1000;
        gm = GameManager.Instance;
    }

    public void setFill(int cnt)
    {
        fillBar.GetComponent<RectTransform>().sizeDelta = new Vector2((fillWidth/4)*cnt,150);
    }

    public void setText(string str)
    {
        text.text = str;
    }

    public void pressEnter()
    {
        SocketObject.Instance.trgRoom = text.text;
        gm._GameState.Value = GameState.JoinRoom;
    }
}
