using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ui_Manager : MonoBehaviour
{
    GameManager gm;
    public GameObject Menu01,Menu02,Menu03;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.Instance;
        Menu();
    }

    public void CreateRoom()
    {
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", true);
        Menu03.GetComponent<Animator>().SetBool("On", false);
    }

    public void RoomList()
    {
        Menu01.GetComponent<Animator>().SetBool("On", false);
        Menu02.GetComponent<Animator>().SetBool("On", false);
        Menu03.GetComponent<Animator>().SetBool("On", true);
    }

    public void Menu()
    {
        Menu01.GetComponent<Animator>().SetBool("On", true);
        Menu02.GetComponent<Animator>().SetBool("On", false);
        Menu03.GetComponent<Animator>().SetBool("On", false);
    }
}
