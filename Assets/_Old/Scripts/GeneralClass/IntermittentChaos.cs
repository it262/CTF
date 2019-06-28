using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntermittentChaos
{
	private float n;

	public IntermittentChaos(){
		n = Random.Range (0, 1f);
	}
    
	public float getChaos(float input){
		n = (input > 0.5f) ? n + 2 * n * n : n - 2 * (1 - n) * (1 - n);
		if (n < 0.05f || n > 0.995) {
			n = Random.Range (0.1f, 0.9f);
		}
		return n;
	}
}
