using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubesController : MonoBehaviour {

	static SocketObject so;

    public GameObject prefab,cube;

	private GameObject sphire;

    List<GameObject> cubes = new List<GameObject>();

	[SerializeField]int _X=15,_Z=11;

    //public Material[] materials;
	//public Material materials;

    private float intensity = 1;

	// Use this for initialization
	void Start () {

		so = SocketObject.Instance;
		CubeSetting ();
	}
	
	// Update is called once per frame
	void Update () {
		if (so.connecting && GameObject.FindGameObjectsWithTag ("ChangeColorArea_Start").Length == 0) {
			sphire = (GameObject)Instantiate (prefab,
				new Vector3 (0, 0, 0),
				Quaternion.identity);
		} else if(!so.connecting && GameObject.FindGameObjectsWithTag ("ChangeColorArea_Start").Length > 0){
			Destroy (sphire);
		}
	}

	void SettingColor(GameObject obs,int n){
		switch (n) {
		case 0:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 1));
			break;
		case 1:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 0, 0));
			break;
		case 2:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 0, 1));
			break;
		case 3:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (1, 1, 0));
			break;
		case 4:
			obs.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",
				new Color (0, 1, 0));
			break;
		}
	}

	public void CubeSetting(){
		for (int i = 0; i < _X; i++) {
			for (int j = 0; j < _Z; j++) {
				GameObject g = (GameObject)Instantiate(cube, new Vector3 (-(_X/2) + i, 1, (_Z/2) - j),Quaternion.identity);
				g.transform.parent = this.gameObject.transform;
				cubes.Add(g);
				SettingColor (g,Random.Range(0,5));
				g.GetComponent<CubeModel> ().setColorDef();
				g.GetComponent<CubeModel> ().isStart = false;
			}
		}
	}

	public void GameStart(){
		foreach (GameObject c in cubes) {
			Destroy (c);
		}
        Destroy(sphire);
		cubes.Clear ();
		this.gameObject.SetActive (false);
	}
}
