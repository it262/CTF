using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ObstacleControllSync : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;
    static GameManager gm;

	JsonInJson jj;

	public GameObject obstaclePrefab,SummonPref,DestroyPlane;

	public Material emit;

	public int xTarget;
	public int zTarget;
	public Vector2 targetSection;
	public GameObject stage;

	GameObject[] tagObjects;

	public float speed;

	private int _Y = 30;
	[SerializeField] int FirstObsMax = 10;

	bool send = false;

	public Dictionary<string,string> first = new Dictionary<string,string>();
	public Queue<string> obs = new Queue<string>();
	public Dictionary<string,string> state = new Dictionary<string,string>();//0:完了 1:処理中

	public Queue<Dictionary<string,string>> victim = new Queue<Dictionary<string,string>>();

	public Dictionary<int,GameObject> obstacle = new Dictionary<int,GameObject>();

	int idCounter = 0;

	public float SendObsInterval = 0.5f;

	// Use this for initialization
	void Start(){
		// -> FirstProcessing()
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
        gm = GameManager.Instance;
		jj = new JsonInJson ();
		GameObject g = Instantiate (DestroyPlane, new Vector3 (0, -50, 0), Quaternion.identity);
		g.GetComponent<DestroyPlane> ().ocs = this;
		g.transform.parent = dw.GameInstance.transform;

       gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.DefaultObstacleSetting)
            .Subscribe(_ => SendDefaultObstacle());
    }

	// Update is called once per frame
	void Update () {

		if (stage == null)
			return;

		if (stage.GetComponent<Stage>().debug)
			return;

        
        if (gm._GameState.Value == GameState.DefaultObstacleSetting)
            ReceiveDefaultObstacle();

        //obstacleがフィールド上になかったら新しいobstacleを召喚する。
        if (gm._GameState.Value == GameState.Playing)
		    ObsUpdate();
	}

    void SendDefaultObstacle()
    {
        if (dw.RoomMaster.Equals(dw.me.GetComponent<PlayerScript>().pd.id))
        {
            var data = new Dictionary<string, string>();
            data["TYPE"] = "FirstObs";
            data["max"] = FirstObsMax.ToString();
            if (stage == null)
                stage = GameObject.Find("Stage(Clone)");
            for (int i = 0; i < FirstObsMax; i++)
            {
                xTarget = (int)Random.Range(0, stage.GetComponent<Stage>().xSection);
                zTarget = (int)Random.Range(0, stage.GetComponent<Stage>().zSection);
                targetSection = stage.GetComponent<Stage>().TargetSection(xTarget, zTarget);

                data["x" + i] = targetSection[0].ToString();
                data["z" + i] = targetSection[1].ToString();
                data["color" + i] = ((int)Random.Range(0, 5)).ToString();//0:normal 1:attack 2:diffence 3:speed 4:heal

            }
            so.EmitMessage("ToOwnRoom", data);
            Debug.Log("オブジェクト初期位置送信完了");
            return;
        }
    }


    void ReceiveDefaultObstacle(){
		//obstacleの初期配置
		if(first.Count > 0){
			int max = int.Parse (first ["max"]);
			for (int i = 0; i < max; i++) {
				string x = "x"+i;
				string z = "z"+i;
				GameObject obs = (GameObject)Instantiate (obstaclePrefab,
					new Vector3 (float.Parse(first[x]), stage.transform.position.y+1f, float.Parse(first[z])),
					                Quaternion.identity);
				obs.gameObject.tag = "fallenObstacle";
				obstacle.Add (idCounter, obs);
				obs.GetComponent<ObsUpdate> ().id = idCounter++;
				obs.transform.parent = dw.GameInstance.transform;
				Color color = SettingColor (obs, int.Parse (first ["color" + i]));
				GameObject summon = (GameObject)Instantiate (SummonPref, obs.transform.position, Quaternion.identity);
				summon.GetComponent<ParticleSystem> ().startColor = color;
				summon.transform.parent = dw.GameInstance.transform;
				Destroy (summon, 2f);

			}
			first.Clear ();
			StartCoroutine("SendObsData");
			Debug.Log ("初期オブジェクト設置完了");
            gm._GameState.Value = GameState.Playing;
            CameraController.Instance.transform.parent = dw.me.GetComponent<PlayerScript>().cam.transform;

        }

	}

	void ObsUpdate(){

		if (victim.Count > 0) {
			Debug.Log ("Receive:"+victim.Peek()["n"] + "破壊");
			var v = victim.Dequeue ();
			var trgObs = obstacle [int.Parse (v ["n"])];
			if (trgObs != null) {
                dw.players[v["attacker"]].GetComponent<PlayerScript>().pd.changeState(trgObs.GetComponent<ObsUpdate>().type);
				trgObs.GetComponent<ObsUpdate> ().Destroy ();
			}
		}

		if (obs.Count > 0) {
			//if (isnt_There ("Obstacle")) {
			//stateSync(1);
			var dic = jj.PicJson(obs.Dequeue());
			Debug.Log (dic.Count);

			int n = int.Parse (dic["n"]);
				int xT = int.Parse (dic ["xTarget"]);
				int zT = int.Parse (dic ["zTarget"]);
				int xW = int.Parse (dic ["x_width"]);
				int yW = int.Parse (dic ["y_width"]);
				int zW = int.Parse (dic ["z_width"]);
				string colorData = dic ["color"];

				targetSection = stage.GetComponent<Stage> ().TargetSection (xT, zT);
				int x_width = xW;
				int y_width = yW;
				int z_width = zW;
				char[] c = colorData.ToCharArray ();
				int cnt = 0;//色変更用
				for (int i = 0; i < x_width; i++) {
					for (int j = 0; j < z_width; j++) {
						for (int k = 0; k < y_width; k++) {
						if (!obstacle.ContainsKey (n)) {
							GameObject o = (GameObject)Instantiate (obstaclePrefab,
								               new Vector3 (targetSection.x + i, _Y - j, targetSection.y/*z*/ - k),
								               Quaternion.identity);
							obstacle.Add (n, o);
							o.GetComponent<ObsUpdate> ().id = n;
							o.transform.parent = dw.GameInstance.transform;
							Color color = SettingColor (o, int.Parse (c [cnt++].ToString ()));
							GameObject summon = (GameObject)Instantiate (SummonPref, o.transform.position, Quaternion.identity);
							summon.GetComponent<ParticleSystem> ().startColor = color;
							summon.transform.parent = dw.GameInstance.transform;
							Destroy (summon, 2f);
							n++;
						}
						}
					}
				}
				//obs.Clear ();
				send = false;
				//}
				//return;
			}
		}

	void stateSync(int s){
		var data = new Dictionary<string,string> ();
		data ["TYPE"] = "StateUpdate";
		data ["state"] = s.ToString();
		so.EmitMessage ("ToOwnRoom", data);
	}


	//targetTagが"いない"時にtrueを返す。フィールド上にobstacleが残っているかどうかを判定するのに使う。
    /*
	bool isnt_There(string targetTag) {
		if (!FPComplete)
			return false;
		tagObjects = GameObject.FindGameObjectsWithTag(targetTag);
		if (tagObjects.Length == 0){
			return true;
		}else {
			return false;
		}
	}
    */

	Color SettingColor(GameObject obs,int n){
		switch (n) {
		case 0:
			//ノーマル
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 0, 0));
			obs.GetComponent<ObsUpdate>().type = "normal";
			return new Color (0,0,0);
			break;
		case 1:
			//atk
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 0));
			obs.GetComponent<ObsUpdate>().type = "atk";
			return new Color (1,0,0);
			break;
		case 2:
			//dif
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 0, 1));
			obs.GetComponent<ObsUpdate>().type = "dif";
			return new Color (0,0,1);
			break;
		case 3:
			//spd
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 1, 0));
			obs.GetComponent<ObsUpdate>().type = "spd";
			return new Color (1,1,0);
			break;
		case 4:
			//hp
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 0, 0));
			obs.GetComponent<ObsUpdate>().type = "life";
			return new Color (0,0,0);
			break;
		}
		return new Color (0,0,0);
	}

	public void DestroyAll(){
		foreach (GameObject obj in obstacle.Values) {
			Destroy (obj);
		}
	}

	IEnumerator SendObsData(){
			while (true) {
			if (!dw.watching && dw.RoomMaster.Equals(dw.me.GetComponent<PlayerScript>().pd.id) && gm._GameState.Value == GameState.Playing) {

				var data = new Dictionary<string,string> ();
				data ["TYPE"] = "Obs";

				var json = new Dictionary<string,string> ();
				json ["n"] = idCounter.ToString ();
				json ["xTarget"] = ((int)Random.Range (0, stage.GetComponent<Stage> ().xSection)).ToString ();
				json ["zTarget"] = ((int)Random.Range (0, stage.GetComponent<Stage> ().zSection)).ToString ();
				int x_width = (int)Random.Range (1, 3);
				int y_width = (int)Random.Range (1, 2);
				int z_width = (int)Random.Range (1, 3);
				int total = x_width + y_width + zTarget;
				json ["total"] = total.ToString ();
				json ["x_width"] = x_width.ToString ();
				json ["y_width"] = y_width.ToString ();
				json ["z_width"] = z_width.ToString ();
				int max = x_width * y_width * z_width;
				string s = "";
				for (int i = 0; i < max; i++) {
					s += ((int)Random.Range (0, 5)).ToString ();
				}
				json ["color"] = s;

				data ["json"] = jj.InJson(json);
				so.EmitMessage ("ToOwnRoom", data);
				idCounter += total;

			}
			yield return new WaitForSeconds (SendObsInterval);
		}
	}
}
