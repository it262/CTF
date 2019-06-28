using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerUI : MonoBehaviour
{
    ObstacleControllSyncServer server;
    InputField text;
    // Start is called before the first frame update
    void Start()
    {
        server = ObstacleControllSyncServer.Instance;
        text = GetComponent<InputField>();
        switch (this.gameObject.name)
        {
            case "Interval":
                text.text = server.interval.ToString();
                break;
            case "X_Section":
                text.text = server.xSection.ToString();
                break;
            case "Z_Section":
                text.text = server.zSection.ToString();
                break;
            case "X_Width":
                text.text = server.xWidth.ToString();
                break;
            case "Y_Width":
                text.text = server.yWidth.ToString();
                break;
            case "Z_Width":
                text.text = server.zWidth.ToString();
                break;
            case "ColorMAX":
                text.text = server.colorMAX.ToString();
                break;
        }
    }

    public void valueChanged()
    {
        switch (this.gameObject.name)
        {
            case "Interval":
                server.interval = double.Parse(text.text);
                break;
            case "X_Section":
                server.xSection = int.Parse(text.text);
                break;
            case "Z_Section":
                server.zSection = int.Parse(text.text);
                break;
            case "X_Width":
                server.xWidth = int.Parse(text.text);
                break;
            case "Y_Width":
                server.yWidth = int.Parse(text.text);
                break;
            case "Z_Width":
                server.zWidth = int.Parse(text.text);
                break;
            case "ColorMAX":
                server.colorMAX = int.Parse(text.text);
                break;
        }
    }
}
