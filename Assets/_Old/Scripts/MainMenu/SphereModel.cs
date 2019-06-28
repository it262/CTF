using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereModel : MonoBehaviour
{

    private float r;

    private float minimumR = 0.0f;
    public float maximumR = 3.0f;
    public float speed = 0.8f;
    private float t = 0.0f;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        r = Mathf.Lerp(minimumR, maximumR, t);
        t = speed * Time.deltaTime;

        transform.localScale += new Vector3(r, 0, r);

        if (transform.localScale.x > maximumR)
        {
            Destroy(gameObject);
        }
    }
}
