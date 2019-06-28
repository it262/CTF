using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMAX : MonoBehaviour
{
	DataWorker dw;
	int _MAX;
    // Start is called before the first frame update
    void Start()
    {
		dw = DataWorker.Instance;
    }

    // Update is called once per frame
    void Update()
    {
		
    }

	public void changeField(){
		if (int.TryParse (GetComponent<Text> ().text, out _MAX)) {
			dw.MAX = (_MAX <= 0) ? 2 : _MAX;
		} else {
			dw.MAX = 2;
		}
	}
}
