using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class StartSphere : MonoBehaviour {

	static DataWorker dw;

    private float r;

    private float minimumR = 0.0f;
    public float maximumR = 20.0f;
    public float speed = 0.8f;
    private float t = 0.0f;

    // Use this for initialization
    void Start () {
		dw = DataWorker.Instance;
	}
	
	// Update is called once per frame
	void Update () {

        r = Mathf.Lerp(minimumR, maximumR, t);
        t = speed * Time.deltaTime;

        transform.localScale += new Vector3(r, 0, r);

        if (transform.localScale.x > maximumR)
        {
            transform.localScale = new Vector3(maximumR, 0, maximumR);
        }
    }
}
