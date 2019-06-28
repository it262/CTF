using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room{

	public GameObject g;
	public string roomName;
	public int cnt;
	public Dictionary<string,string> member = new Dictionary<string,string>();
	public string master;
	public bool playing;


	public Room(GameObject g,string roomName){
		this.g = g;
		this.roomName = roomName;
		this.cnt = 0;
		playing = false;
		updateState ();
	}

	public Room(string roomName){
		this.roomName = roomName;
		this.cnt = 0;
		playing = false;
		updateState ();
	}


	public void joinRoom(string id,string name){
		member.Add (id,name);
		cnt++;
		updateState ();
	}

	public void leaveRoom(string id){
		member.Remove (id);
		cnt--;
		updateState ();
	}

	public void updateState(){
		/*
		this.g.transform.Find ("RoomState/name").GetComponent<Text> ().text = roomName;
		this.g.transform.Find ("RoomState/member").GetComponent<Text> ().text = cnt.ToString () + "/" + so.GetComponent<DataWorker> ().MAX;
		int n = 0;
		foreach (string val in member.Values) {
			this.g.transform.Find ("Member/player" + (n)).GetComponent<Text> ().color = Color.white;
			if (val == null) {
				this.g.transform.Find ("Member/player" + (n++)).GetComponent<Text> ().text = "[データ受信集...]";
			} else {
				this.g.transform.Find ("Member/player" + (n++)).GetComponent<Text> ().text = val;
			}
		}
		for (int i = n; i < 4; i++) {
			if (i < so.GetComponent<DataWorker> ().MAX) {
				this.g.transform.Find ("Member/player" + i).GetComponent<Text> ().color = Color.green;
				this.g.transform.Find ("Member/player" + i).GetComponent<Text> ().text = "[VACANCY]";
			} else {
				this.g.transform.Find ("Member/player" + i).GetComponent<Text> ().color = Color.red;
				this.g.transform.Find ("Member/player" + i).GetComponent<Text> ().text = "-----";
			}
		}
		*/
	}

}