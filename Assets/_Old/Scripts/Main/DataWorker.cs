using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class DataWorker : SingletonMonoBehavior<DataWorker> {

    static GameManager gm;

    CameraController cc;
    [SerializeField] GameObject titleCamerapos;

	public int MAX = 2;

	[SerializeField]GameObject PlayerPrefab,StagePrefab,cubesController,sphereController,TitleCamera,TitleText,GameInstancePrefab;

	public GameObject GameInstance;

	public Dictionary<string,Vector3> posSync = new Dictionary<string,Vector3>();
	public Dictionary<string,Vector2> rotSync = new Dictionary<string,Vector2>();
	public Dictionary<string,bool> heatbeat = new Dictionary<string,bool>();
	public Dictionary<string,bool> pushSwitch = new Dictionary<string,bool>();
	public Queue<Dictionary<string,string>> chatQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> roomQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> hitQue = new Queue<Dictionary<string,string>>();
	public Queue<Dictionary<string,string>> elimQue = new Queue<Dictionary<string,string>>();

    public string RoomMaster;

	public JSONObject roomState;
	public Dictionary<string,GameObject> players = new Dictionary<string,GameObject> ();
	public GameObject me;

	public bool Exping = false;

    public bool watching = false;

	public Room myRoom;

	public int score;

	public GameObject InstanceStage,InstanceObsCon;
	public GameObject Menu,Enhanced;

	// Use this for initialization
	void Start () {
		Object o = (Object)this;
		Debug.Log (o);
        cc = CameraController.Instance;
        cc.transform.parent = titleCamerapos.transform;

        gm = GameManager.Instance;

        gm._GameState
            .DistinctUntilChanged()
            .Where(x => x == GameState.RoomSettingComp)
            .Subscribe(_ =>PlayerListSet());

    }
	
	// Update is called once per frame
	void Update () {

		if (GameManager.Instance._GameState.Value == GameState.Playing) {

			Dictionary<string,string> d;

            if (hitQue.Count > 0)
            {
                d = hitQue.Dequeue();
                var g = players[d["trg"]];
                Vector3 start = new Vector3(float.Parse(d["startX"]), float.Parse(d["startY"]), float.Parse(d["startZ"]));
                Vector3 end = new Vector3(float.Parse(d["endX"]), float.Parse(d["endY"]), float.Parse(d["endZ"]));
                g.GetComponent<PlayerScript>().model.setImpactData(start, end, g);

            }

            if (elimQue.Count > 0) {
				d = elimQue.Dequeue ();
				if (d ["trg"].Equals (GetComponent<SocketObject> ().id)) {
					//死亡
					MenuSetting ();
				} else {
					exclusion (d ["trg"].ToString ());
				}
			}

			Debug.Log (players.Count);
			if (!Exping && (MAX!=1) &&(players.Count == 1)) {
				score += 300;
				MenuSetting ();
			}


		}
	}

    void PlayerListSet()
    {
        Debug.Log("ルームエラー");
        if (myRoom != null && myRoom.cnt == MAX && players.Count == 0)
        {
            Debug.Log("ルームエラー");
            players.Clear();
            foreach (KeyValuePair<string, string> member in myRoom.member)
            {
                players.Add(member.Key, null);
            }
            //playing = true;
            Debug.Log("Start[ルーム名：" + myRoom.roomName + "/人数：" + players.Count + "]");
            GameSettings();
        }
        else
        {
            Debug.Log("ルームエラー");
        }
    }

	void GameSettings(){
		GameInstance = (GameObject)Instantiate (GameInstancePrefab);
		TitleText.SetActive (false);
		//TitleCamera.SetActive (false);
		sphereController.SetActive (false);
		cubesController.GetComponent<CubesController> ().GameStart ();
		InstanceStage = (GameObject)Instantiate (StagePrefab,Vector3.zero,Quaternion.identity);
		InstanceStage.transform.parent = GameInstance.transform;
        Enhanced.SetActive(true);
        Menu.SetActive(false);
    }

	public void PlayerCreate(GameObject obCon,List<Vector2> pos){
		int cnt = 1;
		players.Clear ();
		foreach (KeyValuePair<string,string> data in myRoom.member) {
			obCon.GetComponent<ObstacleControllSync> ().state.Add (data.Key, "0");
			GameObject g = (GameObject)Instantiate (PlayerPrefab,new Vector3(pos[cnt-1].x,5,pos[cnt-1].y),Quaternion.identity);
            PlayerScript ps = g.GetComponent<PlayerScript>();
            ps.pd = new PlayerData();
			if (GetComponent<SocketObject> ().id.Equals (data.Key)) {
				//TitleCamera.SetActive (false);
				ps.pd.isPlayer = true;
				g.tag = "Player";
				me = g;
			} else {
				ps.cam.SetActive(false);
                g.GetComponent<Attack>().enabled = false;
				g.tag = "Others";
			}
			ps.pd.id = data.Key;
			ps.pd.name = data.Value;
			g.transform.parent = GameInstance.transform;
			players.Add (data.Key, g);
            Debug.Log("Player" + (cnt++) + "：" + ps.pd.id + "(" + ps.pd.name + ")");
        }
        gm._GameState.Value = GameState.DefaultObstacleSetting;
    }

	void MenuSetting(){
        Enhanced.SetActive(false);
        cc.transform.parent = titleCamerapos.transform;
        Destroy (InstanceStage);
		InstanceObsCon.GetComponent<ObstacleControllSync> ().DestroyAll ();
		Destroy (InstanceObsCon);
		foreach (GameObject data in players.Values) {
			Destroy (data);
		}
		DataClear ();
		//TitleCamera.SetActive (true);
		TitleText.SetActive (true);
		sphereController.SetActive (true);
		cubesController.SetActive (true);
		cubesController.GetComponent<CubesController> ().CubeSetting ();
		Menu.SetActive (true);
	}

	public void disconnectUser(string id){
		var data = new Dictionary<string,string> ();
		data ["TYPE"] = "Dead";
		data ["id"] = id;
		GetComponent<SocketObject> ().EmitMessage ("ToOwnRoom", data);
		score += (players.Count > 2) ? 150 :(players.Count > 3) ? 50 : 0;
	}

	public void exclusion(string id){
        if (players.ContainsKey(id))
        {
            if (id.Equals(me.GetComponent<PlayerScript>().pd.id)) {
            CameraController.Instance.transform.parent = InstanceStage.GetComponent<Stage>().CamPos.transform;
            watching = true;
        }
            Destroy (players [id]);
			players.Remove (id);
		}
	}

	public void DataClear(){
		//退室処理
		var data = new Dictionary<string,string> ();
		data ["to"] = "LEAVE";
		GetComponent<SocketObject>().EmitMessage ("Quick", data);
		Debug.Log ("[DataWorker]退室しました");

		Destroy (GameInstance);

        gm._GameState.Value = GameState.None;

        RoomMaster = null;

        watching = false;
		Exping = false;
		myRoom = null;
		me = null;
		posSync.Clear ();
		rotSync.Clear ();
		heatbeat.Clear ();
		roomQue.Clear ();
		hitQue.Clear ();
		players.Clear ();

		Cursor.lockState = CursorLockMode.None;
		Cursor.SetCursor (null,Vector2.zero,CursorMode.ForceSoftware);

	}
}


