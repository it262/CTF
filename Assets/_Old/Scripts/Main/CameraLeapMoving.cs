using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLeapMoving : SingletonMonoBehavior<CameraLeapMoving>
{

	public Vector3 toPos;

	public Vector3 StartPos;

    // Start is called before the first frame update
    void Start()
    {
		StartPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
		transform.position = Vector3.Lerp (transform.position, toPos, 0.8f);
    }
}
