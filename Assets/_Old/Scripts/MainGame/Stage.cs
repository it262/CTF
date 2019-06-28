using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stage : MonoBehaviour {

	static DataWorker dw;

	[SerializeField]GameObject Obs;
	public GameObject ObsController;
    public GameObject CamPos;

	public bool debug = false;

    public int half_zSection = 5;
    public int half_xSection = 10;
    public int zSection;
    public int xSection;
	public int half_xHole;
	public int half_zHole;
    float zScale;
    float xScale;
    int[] zGridArray;
    int[] xGridArray;

	public List<Vector2> spawnPoints = new List<Vector2> ();

    // Use this for initialization
    void Start () {

		dw = DataWorker.Instance;

        zSection = half_zSection * 2;
        xSection = half_xSection * 2;
        zScale = zSection / 10.0f;
        xScale = xSection / 10.0f;
        
		float startX = Obs.transform.localScale.x * half_xSection;
		float startZ = Obs.transform.localScale.z * half_zSection;
        //transform.localScale = new Vector3(xScale*10, 1f, zScale*10);
		for (int i = 0; i < xSection; i++) {
			for (int j = 0; j < zSection; j++) {
				if (((half_xSection - half_xHole) > i || (half_zSection - half_zHole) > j) ||
					((half_xSection + half_xHole) < i || (half_zSection + half_zHole) < j)) {
					GameObject g = (GameObject)Instantiate (Obs, new Vector3 (
						              startX - Obs.transform.localScale.x / 2 - Obs.transform.localScale.x * i,
						              0,
						              startZ - Obs.transform.localScale.z / 2 - Obs.transform.localScale.z * j), 
						              Quaternion.identity);
					g.transform.parent = transform;
				}
			}
		}

		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter> ();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int k = 0;
		while(k<meshFilters.Length){
			combine[k].mesh = meshFilters[k].sharedMesh;
			combine[k].transform = meshFilters[k].transform.localToWorldMatrix;
			meshFilters[k].gameObject.SetActive(false);
			k++;
		}
		transform.GetComponent<MeshFilter>().mesh = new Mesh();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
		transform.GetComponent<MeshCollider> ().sharedMesh = transform.GetComponent<MeshFilter> ().mesh;
		transform.gameObject.SetActive(true);

        //targetObstacleを求めるための座標を格納する配列を作る。
        zGridArray = new int[zSection + 1];

        for (int i = 0; i < zGridArray.Length; i++) {
            zGridArray[i] = -1 * half_zSection + i;
        }

        xGridArray = new int[xSection + 1];

        for (int i = 0; i < xGridArray.Length; i++) {
            xGridArray[i] = -1 * half_xSection + i;
        }

		GameObject obs = (GameObject)Instantiate (ObsController, Vector3.zero, Quaternion.identity);
		obs.GetComponent<ObstacleControllSync> ().stage = this.gameObject;
        Debug.Log(obs.GetComponent<ObstacleControllSync>().stage);
        dw.InstanceObsCon = obs;
		obs.transform.parent = dw.GameInstance.transform;

		List<Vector2> spawnPoints = new List<Vector2> ();
		spawnPoints.Add (TargetSection(0,0));
		spawnPoints.Add (TargetSection(xSection-1,zSection-1));
		spawnPoints.Add (TargetSection(0,zSection-1));
		spawnPoints.Add (TargetSection(xSection-1,0));
		dw.PlayerCreate (obs,spawnPoints);

        CamPos = Instantiate(CamPos, new Vector3(transform.position.x + 30f, transform.position.y + 10f, 0), Quaternion.identity);
        CameraController.Instance.transform.parent = CamPos.transform;
	}
	
	// Update is called once per frame
	void Update () {
        CamPos.transform.RotateAround(transform.position, new Vector3(0, 1, 0), 10 * Time.deltaTime);
        CamPos.transform.LookAt(transform);
    }

    //obstacleの召喚の起点となるtargetObstacleのx,yを求めるための関数
	public Vector2 TargetSection(int targetX, int targetZ) {

		Vector2 result;
		result.x = (xGridArray[targetX] + xGridArray[targetX + 1]) / 2.0f;
		result.y = (zGridArray[targetZ] + zGridArray[targetZ + 1]) / 2.0f;

        return result;
    }
}
