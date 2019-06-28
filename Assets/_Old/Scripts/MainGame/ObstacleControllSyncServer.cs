using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class ObstacleControllSyncServer : SingletonMonoBehavior<ObstacleControllSyncServer> {

	static SocketObjectONE so;

	JsonInJson jj;

	int idCounter = 0;

    public double interval = 0.5;
    public int xSection = 11,zSection = 15;
    public int xWidth = 3, yWidth = 2, zWidth = 3;
    public int colorMAX = 5;

    SerialDisposable disposable;

    // Use this for initialization
    void Start(){
		so = SocketObjectONE.Instance;

        disposable = new SerialDisposable();

        disposable.Disposable = Observable.Interval(TimeSpan.FromSeconds(interval))
            .Subscribe(_ =>
                SendObsData()
            ).AddTo(this);

        this.ObserveEveryValueChanged(x => this.interval)
            .Skip(1)
            .Subscribe(_ => Restart());

    }

    void Restart()
    {
        Debug.Log("Restart");
        disposable.Dispose();
        disposable = new SerialDisposable();
        disposable.Disposable = Observable.Interval(TimeSpan.FromSeconds(interval)).Subscribe(_ =>
                SendObsData()
            ).AddTo(this);
    }

    void SendObsData()
    {
        if (so.id == null)
            return;
        var data = new Dictionary<string, string>();
        var json = new Dictionary<string, string>();
        json["n"] = idCounter.ToString();
        json["xTarget"] = ((int)UnityEngine.Random.Range(0, xSection)).ToString();
        json["zTarget"] = ((int)UnityEngine.Random.Range(0, zSection)).ToString();
        int x_width = (int)UnityEngine.Random.Range(1, xWidth);
        int y_width = (int)UnityEngine.Random.Range(1, yWidth);
        int z_width = (int)UnityEngine.Random.Range(1, zWidth);
        int total = x_width + y_width + z_width;
        json["x_width"] = x_width.ToString();
        json["y_width"] = y_width.ToString();
        json["z_width"] = z_width.ToString();
        int max = x_width * y_width * z_width;
        string s = "";
        for (int i = 0; i < max; i++)
        {
            s += ((int)UnityEngine.Random.Range(0, colorMAX)).ToString();
        }
        json["color"] = s;
        //data["json"] = jj.InJson(json);
        so.EmitMessage("Obs", json);
        Debug.Log("[EMIIT]"+idCounter+"-"+(idCounter+total));
        if (idCounter + total > 2000000000)
        {
            idCounter = 0;
        }
        idCounter += total;
    }
}
