using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu_Button : MonoBehaviour
{
    GameManager gm;
    private void Start()
    {
        gm = GameManager.Instance;
    }
    public void CreateRoom()
    {
        if (gm._GameState.Value == GameState.Menu)
        {
            Debug.Log("CREATE");
            gm._GameState.Value = GameState.CreateRoom;
        }
    }

    public void SerchRoom()
    {
        if (gm._GameState.Value == GameState.Menu)
        {
            Debug.Log("SERCH");
            gm._GameState.Value = GameState.RoomSerching;
        }
    }

    public void JoinRoom()
    {
        if (gm._GameState.Value == GameState.Menu)
        {
            gm._GameState.Value = GameState.RoomSerching;
            GetComponent<ui_Manager>().RoomList();
        }
    }
}
