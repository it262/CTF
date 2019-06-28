using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeModel : MonoBehaviour {

	static SocketObject so;
	static DataWorker dw;
    static GameManager gm;
	IntermittentChaos IChaos;

    Color color = new Color();

    public float intensity = 0;

    public bool isEnter = false;
    public bool isStart = false;

	float chaos = 0.1f;

	// Use this for initialization
	void Start () {
		so = SocketObject.Instance;
		dw = DataWorker.Instance;
        gm = GameManager.Instance;
		IChaos = new IntermittentChaos ();
		StartCoroutine ("Chaos");
	}
	
	// Update is called once per frame
	void Update () {
		if (!so.connecting) {
			isStart = false;
		}
        if (isEnter || isStart)
        {

			if (!isStart || !(gm._GameState.Value == GameState.RoomSerching)) {
				

				//intensity = Mathf.Lerp(intensity, 1, 0.1f);
				chaos = 1;
				if (intensity >= 0.9) {
					if (isEnter) {
						isEnter = false;
					}

					if (isStart) {
						//色が全付いた瞬間
					}
 
				}
			}
        }
        else
        {
			chaos = 0;
			//intensity = Mathf.Lerp(intensity, 0, 0.1f);
        }

		intensity = Mathf.Lerp(intensity, chaos, 0.1f);


        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor",
			new Color(color.r, color.g, color.b) * intensity);

		if(gm._GameState.Value == GameState.Playing)
			CheckDestroy ();

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChangeColorArea"))
        {
            isEnter = true;
        }

        if (other.CompareTag("ChangeColorArea_Start"))
        {
            isStart = true;
        }
    }

    public void setColorDef()
    {
        color = gameObject.GetComponent<Renderer>().material.GetColor("_EmissionColor");
    }

	void CheckDestroy(){
		if (Vector3.Distance (Vector3.zero, transform.position) > 50f) {
			Destroy (this.gameObject);
		}
	}

	IEnumerator Chaos(){
		while (true) {
			if (gm._GameState.Value == GameState.RoomSerching) {
				chaos = IChaos.getChaos (intensity);
			}
			yield return new WaitForSeconds (0.1f);
		}
	}
}
