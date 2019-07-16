using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class players_LeadyUI : MonoBehaviour
{
    public void setPlayerName(string name)
    {
        GetComponentInChildren<Text>().text = name;
    }
}
