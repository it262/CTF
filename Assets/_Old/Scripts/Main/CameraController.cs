using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonMonoBehavior<CameraController>
{
    public GameObject lookAt;
    public Vector3 setPos;
    // Start is called before the first frame update
    void Start()
    {
        setPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localPosition != setPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 0.1f);
            if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.1f)
            {
                transform.localPosition = Vector3.zero;
            }
        }
        if (lookAt != null)
        {
            transform.LookAt(lookAt.transform);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
